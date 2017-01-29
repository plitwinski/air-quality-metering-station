using Autofac;
using Quartz;
using Quartz.Impl;
using System.Linq;
using System.Reflection;

namespace Metering.Station.Main.Background
{
    public class QuartzModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new StdSchedulerFactory().GetScheduler()).As<IScheduler>().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));
        }
    }
}
