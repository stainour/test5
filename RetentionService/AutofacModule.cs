using Autofac;
using Autofac.Extras.Quartz;
using BackupService.Integration;
using BackupService.Integration.Implementation;
using System.Collections.Specialized;

namespace RetentionService
{
    internal class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var schedulerConfig = new NameValueCollection {
                {"quartz.threadPool.threadCount", "1"},
                {"quartz.scheduler.threadName", "Scheduler"}
            };

            builder.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = c => schedulerConfig
            });
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(RetentionJob).Assembly));

            builder.RegisterType<RetentionJob>().AsSelf();
            builder.RegisterType<InMemoryBackupServiceFacade>().As<IBackupServiceFacade>().InstancePerDependency();
            base.Load(builder);
        }
    }
}