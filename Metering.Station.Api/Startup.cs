using Autofac;
using Nancy.Hosting.Self;
using System;

namespace Metering.Station.Api
{
    public class Startup
    {
        private readonly ILifetimeScope _lifetimeScope;

        public Startup(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public NancyHost Initialize(int port = 8080)
        {
            var hostConfiguration = new HostConfiguration
            {
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };

            return new NancyHost(new Bootstrapper(_lifetimeScope), hostConfiguration, new Uri($"http://127.0.0.1:{port}"));
        }
    }
}
