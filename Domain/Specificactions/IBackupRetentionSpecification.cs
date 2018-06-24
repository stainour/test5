namespace Domain.Specificactions
{
    public interface IBackupRetentionSpecification
    {
        bool AreAllRetainedBackupsFound { get; }

        bool ShouldBeRetained(in Backup backup);
    }
}