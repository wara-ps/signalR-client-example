using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace boardgame.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add options of the given type and bind to configuration section.
        /// </summary>
        public static OptionsBuilder<TOptions> AddOptionsAndBindToConfig<TOptions>(this IServiceCollection services, string section)
            where TOptions : class
        {
            var builder = services.AddOptions<TOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    configuration.GetSection(section).Bind(options);
                });

            return builder;
        }

        /// <summary>
        /// Adds an injectable IHostedService registration of the given type.
        /// </summary>
        public static IServiceCollection AddHostedService<ServiceInterface, ServiceImplementation>(this IServiceCollection services)
            where ServiceInterface : class
            where ServiceImplementation : class, IHostedService, ServiceInterface
        {
            services.AddSingleton<ServiceInterface, ServiceImplementation>();
            services.AddHostedService<HostedServiceWrapper<ServiceInterface>>();

            return services;
        }


        private class HostedServiceWrapper<ServiceInterface> : IHostedService
        {
            private readonly IHostedService _service;

            public HostedServiceWrapper(ServiceInterface hostedService)
            {
                _service = (IHostedService)hostedService;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                return _service.StartAsync(cancellationToken);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return _service.StopAsync(cancellationToken);
            }
        }
    }
}
