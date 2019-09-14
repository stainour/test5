using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Specificactions
{
    public class BackupRetentionSpecificactionBuilder
    {
        private readonly DateTime _retainBeforeDate;
        private readonly List<(uint ageInDays, uint maxRetainedCount)> _specSettings = new List<(uint ageInDays, uint maxCount)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupRetentionSpecificactionBuilder"/> class.
        /// </summary>
        /// <param name="retainBeforeDate">
        /// Retain backups before <paramref name="retainBeforeDate"/> date
        /// </param>
        public BackupRetentionSpecificactionBuilder(DateTime retainBeforeDate)
        {
            _retainBeforeDate = retainBeforeDate;
        }

        private BackupRetentionSpecificactionBuilder()
        {
        }

        /// <summary>
        /// Adds the rule.
        /// </summary>
        /// <param name="ageInDays">The age in days.</param>
        /// <param name="maxRetainedCount">The maximum retained count.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxRetainedCount"/> less-than or equal to 0 or <paramref
        /// name="ageInDays"/> - All ageInDays should be in ascending order
        /// </exception>
        public BackupRetentionSpecificactionBuilder AddRule(uint ageInDays, uint maxRetainedCount)
        {
            if (maxRetainedCount == 0) throw new ArgumentOutOfRangeException(nameof(maxRetainedCount));
            if (_specSettings.Any())
            {
                var previousSpecSetting = _specSettings.Last();

                if (previousSpecSetting.ageInDays > ageInDays)
                {
                    throw new ArgumentOutOfRangeException(nameof(ageInDays), "All ageInDays should be in ascending order");
                }
            }

            _specSettings.Add((ageInDays, maxRetainedCount));

            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// AddRule should be invoked at least once
        /// </exception>
        public IBackupRetentionSpecification Build()
        {
            if (_specSettings.Any())
            {
                var specSettingsCount = _specSettings.Count;

                if (specSettingsCount == 1)
                {
                    var specSetting = _specSettings.First();
                    return new InitialBackupRetentionSpecification(new FinishingBackupRetentionSpecification(specSetting.ageInDays, _retainBeforeDate, specSetting.maxRetainedCount));
                }

                InnerBackupRetentionSpecification prevSpecification = default;
                InnerBackupRetentionSpecification firstSpecification = default;

                for (int i = 0; i < specSettingsCount - 1; i++)
                {
                    var specSetting = _specSettings[i];

                    var specification = new InnerBackupRetentionSpecification(specSetting.ageInDays, _retainBeforeDate, specSetting.maxRetainedCount);

                    prevSpecification?.SetNextSpecification(specification);

                    prevSpecification = specification;

                    if (firstSpecification == default)
                    {
                        firstSpecification = prevSpecification;
                    }
                }

                var lastSpec = _specSettings.Last();

                prevSpecification.SetNextSpecification(new FinishingBackupRetentionSpecification(lastSpec.ageInDays, _retainBeforeDate, lastSpec.maxRetainedCount));

                return new InitialBackupRetentionSpecification(firstSpecification);
            }

            throw new InvalidOperationException($"{nameof(AddRule)} should be invoked at least once");
        }
    }
}