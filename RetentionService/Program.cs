using Autofac;
using Quartz;
using System;
using System.Threading;

namespace RetentionService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacModule>();

            var container = containerBuilder.Build();

            var job = JobBuilder.Create<RetentionJob>().WithIdentity("Heartbeat", "Maintenance").Build();
            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInHours(6).StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(1, 0)))
                .Build();

            var cts = new CancellationTokenSource();

            var scheduler = container.Resolve<IScheduler>();
            scheduler.ScheduleJob(job, trigger, cts.Token);

            scheduler.Start().Wait();

            Console.WriteLine("======================");
            Console.WriteLine("Press Enter to exit...");
            Console.WriteLine("======================");
            Console.ReadLine();

            cts.Cancel();
            scheduler.Shutdown().Wait();
        }
    }
}