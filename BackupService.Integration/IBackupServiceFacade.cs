using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackupService.Integration
{
    public interface IBackupServiceFacade : IDisposable
    {
        /// <summary>
        /// Delete backups (atomic operation)
        /// </summary>
        /// <param name="backupsToDelete">The backups to delete.</param>
        /// <exception cref="ArgumentNullException">backupsToDelete is null</exception>
        /// <exception cref="OperationException">internal error</exception>
        /// <returns></returns>
        Task DeleteAsync(IEnumerable<IBackup> backupsToDelete);

        /// <summary>
        /// Gets the backups before <paramref name="toDate"/> value (idempotent operation)
        /// </summary>
        /// <param name="toDate">To date UTC+0</param>
        /// <exception cref="OperationException">internal error</exception>
        /// <returns></returns>
        Task<IEnumerable<IBackup>> GetBackupsAsync(DateTime toDate);
    }
}