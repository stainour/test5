using System;

namespace Domain.Specificactions
{
    internal class InnerBackupRetentionSpecification : AbstractBackupRetentionSpecification
    {
        private AbstractBackupRetentionSpecification _nextSpecification;

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerBackupRetentionSpecification"/> class.
        /// </summary>
        /// <param name="maxBackupAge">The maximum backup age in days</param>
        /// <param name="retainBeforeDate">The retain before date.</param>
        /// <param name="maxRetainedCount">The maximum retained count.</param>
        /// <exception cref="ArgumentOutOfRangeException">maxBackupAge or maxRetainedCount</exception>
        internal InnerBackupRetentionSpecification(uint maxBackupAge, DateTime retainBeforeDate, uint maxRetainedCount) : base(maxBackupAge, retainBeforeDate, maxRetainedCount)
        {
            if (maxBackupAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxBackupAge));
            if (maxRetainedCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxRetainedCount));
        }

        public override bool AreAllRetainedBackupsFound => _nextSpecification.AreAllRetainedBackupsFound;

        public override bool ShouldBeRetained(in Backup backup)
        {
            if (backup.CreationDate <= ThresholdDate && backup.CreationDate > _nextSpecification.ThresholdDate && !IsSpecificactionMaxOut)
            {
                NewBackupIsFound();
                return true;
            }

            return _nextSpecification.ShouldBeRetained(in backup);
        }

        internal void SetNextSpecification(AbstractBackupRetentionSpecification nextSpecification)
        {
            if (nextSpecification == null) throw new ArgumentNullException(nameof(nextSpecification));
            if (RetainBeforeDate != nextSpecification.RetainBeforeDate)
            {
                throw new ArgumentOutOfRangeException(nameof(nextSpecification.RetainBeforeDate), $"All {nameof(nextSpecification.RetainBeforeDate)} should be the same");
            }
            if (MaxBackupAge > nextSpecification.MaxBackupAge)
            {
                throw new ArgumentOutOfRangeException(nameof(nextSpecification.MaxBackupAge), $"All {nameof(nextSpecification.MaxBackupAge)} should be in ascending order");
            }

            _nextSpecification = nextSpecification;
        }
    }
}