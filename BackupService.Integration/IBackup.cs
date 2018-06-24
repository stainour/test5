using System;

namespace BackupService.Integration
{
    public interface IBackup
    {
        /// <summary>
        /// Gets backup UTC+0 creation date.
        /// </summary>
        /// <value>Backup creation UTC+0 date.</value>
        DateTime CreationDate { get; }
    }
}