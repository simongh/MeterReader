using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
             }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

            return services;
        }

        private static void HttpClientHelper(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.glowmarkt.com/api/v0-1/");
            client.DefaultRequestHeaders.Add("applicationId", "b0f1b774-a586-4f72-9edd-27ead8aa7a8d");
        }
    }
}