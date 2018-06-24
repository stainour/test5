using System;

namespace Domain.Specificactions
{
    internal abstract class AbstractBackupRetentionSpecification : IBackupRetentionSpecification
    {
        protected internal readonly int MaxBackupAge;
        protected internal readonly DateTime RetainBeforeDate;
        protected internal readonly DateTime ThresholdDate;
        private readonly int _maxRetainedCount;
        private int _currentRetainedCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBackupRetentionSpecification"/> class.
        /// </summary>
        /// <param name="maxBackupAge">The maximum backup age in days</param>
        /// <param name="retainBeforeDate">The retain before date.</param>
        /// <param name="maxRetainedCount">The maximum retained count.</param>
        public AbstractBackupRetentionSpecification(int maxBackupAge, DateTime retainBeforeDate, int maxRetainedCount)
        {
            MaxBackupAge = maxBackupAge;
            RetainBeforeDate = retainBeforeDate;
            _maxRetainedCount = maxRetainedCount;
            ThresholdDate = retainBeforeDate.AddDays(-maxBackupAge);
        }

        public abstract bool AreAllRetainedBackupsFound { get; }
        protected bool IsSpecificactionMaxOut => _currentRetainedCount == _maxRetainedCount;

        public abstract bool ShouldBeRetained(in Backup backup);

        protected void NewBackupIsFound() => ++_currentRetainedCount;
    }
}