using System;

namespace Domain.Specificactions
{
    internal class FinishingBackupRetentionSpecification : AbstractBackupRetentionSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishingBackupRetentionSpecification"/> class.
        /// </summary>
        /// <param name="maxBackupAge">The maximum backup age in days</param>
        /// <param name="retainBeforeDate">The retain before date.</param>
        /// <param name="maxRetainedCount">The maximum retained count.</param>
        public FinishingBackupRetentionSpecification(int maxBackupAge, DateTime retainBeforeDate, int maxRetainedCount) : base(maxBackupAge, retainBeforeDate, maxRetainedCount)
        {
        }

        public override bool AreAllRetainedBackupsFound => IsSpecificactionMaxOut;

        public override bool ShouldBeRetained(in Backup backup)
        {
            if (AreAllRetainedBackupsFound)
            {
                return false;
            }

            if (backup.CreationDate <= ThresholdDate)
            {
                NewBackupIsFound();
                return true;
            }

            return false;
        }
    }
}