using Domain;
using Domain.Specificactions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class BackupRetentionSpecificactionTests
    {
        private readonly DateTime _retainBeforeDate = new DateTime(2018, 5, 31);

        [Fact]
        public void ShouldBeRetained_NoBackup_NoRetained()
        {
            var backups = Enumerable.Empty<Backup>().ToList();

            BackupUnits(backups, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), survivedUnits => Assert.Empty(survivedUnits));
        }

        [Fact]
        public void ShouldBeRetained_OneBackupEveryDay_TwelveRetained()
        {
            var backups = Enumerable.Range(1, 31).Reverse().Select(i => new Backup(new DateTime(2018, 5, i))).ToList();

            BackupUnits(backups, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), retainedBackups =>
           {
               Assert.Equal(12, retainedBackups.Count);

               Assert.Equal(retainedBackups[0], backups[0]);
               Assert.Equal(retainedBackups[1], backups[1]);
               Assert.Equal(retainedBackups[2], backups[2]);
               Assert.Equal(retainedBackups[3], backups[3]);
               Assert.Equal(retainedBackups[4], backups[4]);
               Assert.Equal(retainedBackups[5], backups[5]);
               Assert.Equal(retainedBackups[6], backups[6]);
               Assert.Equal(retainedBackups[7], backups[7]);
               Assert.Equal(retainedBackups[8], backups[8]);
               Assert.Equal(retainedBackups[9], backups[9]);
               Assert.Equal(retainedBackups[10], backups[10]);
               Assert.Equal(retainedBackups[11], backups[14]);
           });
        }

        [Fact]
        public void ShouldBeRetained_OneLatestBackup_OneRetained()
        {
            var backups = new List<Backup> { new Backup(new DateTime(2018, 5, 30)) };

            BackupUnits(backups, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), retainedBackups => Assert.Single(retainedBackups));
        }

        [Fact]
        public void ShouldBeRetained_OneOldestBackup_OneRetained()
        {
            var backupUnits = new List<Backup> { new Backup(new DateTime(2018, 5, 10)) };

            BackupUnits(backupUnits, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), retainedBackups => Assert.Single(retainedBackups));
        }

        [Fact]
        public void ShouldBeRetained_TwoBackupsEveryDay_SeventeenRetained()
        {
            var backups = Enumerable.Range(2, 62).Reverse().Select(i => new Backup(new DateTime(2018, 5, i / 2, 5, i / 2, 0))).ToList();

            BackupUnits(backups, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), retainedBackups =>
            {
                Assert.Equal(17, retainedBackups.Count);

                Assert.Equal(retainedBackups[0], backups[0]);
                Assert.Equal(retainedBackups[1], backups[1]);
                Assert.Equal(retainedBackups[2], backups[2]);
                Assert.Equal(retainedBackups[3], backups[3]);
                Assert.Equal(retainedBackups[4], backups[4]);
                Assert.Equal(retainedBackups[5], backups[5]);
                Assert.Equal(retainedBackups[6], backups[6]);
                Assert.Equal(retainedBackups[7], backups[7]);
                Assert.Equal(retainedBackups[8], backups[8]);
                Assert.Equal(retainedBackups[9], backups[9]);
                Assert.Equal(retainedBackups[10], backups[10]);
                Assert.Equal(retainedBackups[11], backups[11]);
                Assert.Equal(retainedBackups[12], backups[16]);
                Assert.Equal(retainedBackups[13], backups[17]);
                Assert.Equal(retainedBackups[14], backups[18]);
                Assert.Equal(retainedBackups[15], backups[19]);
                Assert.Equal(retainedBackups[16], backups[30]);
            });
        }

        [Fact]
        public void ShouldBeRetained_TwoOldestBackup_OneRetained()
        {
            var backups = new List<Backup> { new Backup(new DateTime(2018, 5, 10)), new Backup(new DateTime(2018, 5, 8)) };

            BackupUnits(backups, () => new BackupRetentionSpecificactionBuilder(_retainBeforeDate).AddRule(3, 4).AddRule(7, 4).AddRule(14, 1).Build(), retainedBackups =>
            {
                Assert.Single(retainedBackups);
                Assert.Equal(retainedBackups[0], backups[0]);
            });
        }

        private static void BackupUnits(List<Backup> backups, Func<IBackupRetentionSpecification> specificactionFactory, Action<List<Backup>> assert)
        {
            void FilterBackups(bool useStopCondition)
            {
                var retainedBackups = new List<Backup>();

                var specificaction = specificactionFactory();

                for (int i = 0; i < backups.Count; i++)
                {
                    var backup = backups[i];

                    if (specificaction.ShouldBeRetained(in backup))
                    {
                        retainedBackups.Add(backup);
                    }

                    if (useStopCondition && specificaction.AreAllRetainedBackupsFound)
                    {
                        break;
                    }
                }
                assert(retainedBackups);
            }

            FilterBackups(true);

            FilterBackups(false);
        }
    }
}