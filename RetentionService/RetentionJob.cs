using BackupService.Integration;
using Domain;
using Domain.Specificactions;
using Polly;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetentionService
{
    internal class RetentionJob : IJob
    {
        private readonly IBackupServiceFacade _backupServiceFacade;

        public RetentionJob(IBackupServiceFacade backupServiceFacade)
        {
            _backupServiceFacade = backupServiceFacade;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            try
            {
                var allBackups = (await Policy.Handle<OperationException>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt)).ExecuteAsync(() => _backupServiceFacade.GetBackupsAsync(now)))
                    .OrderByDescending(backup => backup.CreationDate);

                var specification = GetSpecification(now);

                var backupsToRemove = allBackups.Where(b =>
                {
                    var backup = new Backup(b.CreationDate);
                    return !specification.ShouldBeRetained(in backup);
                });

                await Policy.Handle<OperationException>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt)).ExecuteAsync(() => _backupServiceFacade.DeleteAsync(backupsToRemove));
            }
            catch (OperationException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static IBackupRetentionSpecification GetSpecification(DateTime retainBeforeDate) => new BackupRetentionSpecificactionBuilder(retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build();
    }
}