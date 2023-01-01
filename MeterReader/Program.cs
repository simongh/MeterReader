using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeterReader
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureApplication(context.Configuration);
                    services.AddMediatR(typeof(Program).Assembly);
                })
                .Build();

            ApplyMigrations(host.Services);

            var mediator = host.Services.GetRequiredService<IMediator>();
            await mediator.Send(new Commands.FindResourcesCommand());
            var startDate = await mediator.Send(new Commands.StartQuery());

            var config = host.Services.GetRequiredService<IConfiguration>();
            var endDate = config.GetValue<DateTimeOffset?>("enddate") ?? DateTimeOffset.UtcNow;

            await mediator.Send(new Commands.GetReadingsCommand
            {
                From = startDate,
                To = endDate,
            });
        }

        private static void ApplyMigrations(IServiceProvider app)
        {
            using var db = app.GetRequiredService<Services.DataContext>();
            db.Database.Migrate();
        }
    }
}