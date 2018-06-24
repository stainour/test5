using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackupService.Integration.Implementation
{
    public class InMemoryBackupServiceFacade : IBackupServiceFacade
    {
        private readonly List<Backup> _backups;

        public InMemoryBackupServiceFacade()
        {
            var now = DateTime.UtcNow;
            _backups = Enumerable.Range(1, 30).Select(i => new Backup(i, now.AddHours(-i * 12))).ToList().OrderBy(backup => Guid.NewGuid()).ToList();
        }

        /// <inheritdoc/>
        public Task DeleteAsync(IEnumerable<IBackup> backupsToDelete)
        {
            if (backupsToDelete == null) throw new ArgumentNullException(nameof(backupsToDelete));
            return Task.Run(() =>
            {
                foreach (var backup in backupsToDelete)
                {
                    _backups.Remove((Backup)backup);
                }
            });
        }

        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IBackup>> GetBackupsAsync(DateTime toDate)
        {
            return Task.FromResult(_backups.Where(backup => backup.CreationDate <= toDate).Cast<IBackup>());
        }
    }
}