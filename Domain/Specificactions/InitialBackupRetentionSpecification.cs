namespace Domain.Specificactions
{
    internal class InitialBackupRetentionSpecification : IBackupRetentionSpecification
    {
        protected readonly AbstractBackupRetentionSpecification NextSpecification;
        private int _currentSatisfied;

        public InitialBackupRetentionSpecification(AbstractBackupRetentionSpecification nextSpecification)
        {
            NextSpecification = nextSpecification;
        }

        private InitialBackupRetentionSpecification()
        {
        }

        public bool AreAllRetainedBackupsFound => NextSpecification.AreAllRetainedBackupsFound;

        public bool ShouldBeRetained(in Backup backup)
        {
            if (backup.CreationDate > NextSpecification.ThresholdDate)
            {
                _currentSatisfied++;
                return true;
            }

            return NextSpecification.ShouldBeRetained(in backup);
        }
    }
}