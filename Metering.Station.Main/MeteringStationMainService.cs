using Autofac;
using log4net;
using Metering.Station.Api;
using Metering.Station.Main.Background;
using Nancy.Hosting.Self;
using Quartz;

namespace Metering.Station.Main
{
    public class MeteringStationMainService
    {
        private readonly ILifetimeScope _lifetimeScope;
        private NancyHost _webApp;
        private readonly ILog _logger = LogManager.GetLogger(nameof(MeteringStationMainService));

        public MeteringStationMainService(ILifetimeScope scope)
        {
            _lifetimeScope = scope;
        }

        public void Start()
        {
            StartDataCollection();
            _webApp = new Startup(_lifetimeScope).Initialize();
            _logger.Info("Starting web server...");
            _webApp.Start();
            _logger.Info("Web server started...");
        }

        public void Stop()
        {
            StopDataCollection();
            _logger.Info("Stopping web server...");
            _webApp?.Stop();
            _webApp?.Dispose();
            _logger.Info("Web server stopped...");
        }

        private void StopDataCollection()
        {
            //TODO consider cancelling data collection task when stopping
            _logger.Info("Stopping data collection background service...");
            var scheduler = _lifetimeScope.Resolve<IScheduler>();
            if(scheduler.IsStarted)
                scheduler.Shutdown();
            _logger.Info("Data collection background service stopped...");
        }

        private void StartDataCollection()
        {
            _logger.Info("Starting data collection background service...");
            var scheduler = _lifetimeScope.Resolve<IScheduler>();
            scheduler.JobFactory = new AutofacJobFactory(_lifetimeScope);
            scheduler.Start();

            var job = JobBuilder.Create<CollectDataJob>()
                .WithIdentity(nameof(CollectDataJob))
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{nameof(CollectDataJob)}_Trigger")
                .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInMinutes(1).WithMisfireHandlingInstructionFireNow())
                .StartNow()
                .Build();

            scheduler.ScheduleJob(job, trigger);
            _logger.Info("Data collection background service started...");
        }
    }
}
