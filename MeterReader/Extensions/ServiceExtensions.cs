using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MeterReader
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Types.Options>(configuration.GetSection("config"));

            services.AddHttpClient<Types.AuthenticationHandler>(HttpClientHelper);
            services.AddHttpClient<Services.IHttpService, Services.HttpService>(HttpClientHelper)
                .AddHttpMessageHandler<Types.AuthenticationHandler>();

            services.AddTransient<Services.IDataContext, Services.DataContext>();
            services.AddDbContext<Services.DataContext>(options =>
             {
                 options.UseSqlite(configuration.GetConnectionString("meterreader"));
             }, contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

            return services;
        }

        private static void HttpClientHelper(IServiceProvider serviceProvider, HttpClient client)
        {
            var options = serviceProvider.GetRequiredService<IOptions<Types.Options>>().Value;

            client.BaseAddress = new Uri(options.Url);
            client.DefaultRequestHeaders.Add("applicationId", options.ApplicationId);
        }
    }
}