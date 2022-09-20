using EventBudDemo;
using System.Reflection;

namespace TestApiDemo
{
    public static class WebApplicatinBuilderExtensions
    {
        public static void ConfigureExtraServices(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            var assemblies = Assembly.Load(new AssemblyName("TestService"));
            services.Configure<IntegrationEventRabbitMQOptions>(configuration.GetSection("Rabbit"));
            services.AddEventBus("TestService", assemblies);
        }
    }
}
