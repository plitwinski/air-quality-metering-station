using log4net.Config;
using Metering.Station.Main.IoC;
using System.IO;
using Topshelf;
using Topshelf.Autofac;

namespace Metering.Station.Main
{
    public class Program
    {
        private const string Log4NetConfigFile = "log4net.config";

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo(Log4NetConfigFile));
            var container = AutofacContainer.Build();
            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.UseAutofacContainer(container);
                x.Service<MeteringStationMainService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(p => p.Start());
                    s.WhenStopped(p => p.Stop());
                });
            });
        }
    }
}
