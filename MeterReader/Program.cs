using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var d = await mediator.Send(new Commands.StartQuery());

            await mediator.Send(new Commands.GetReadingsCommand
            {
                From = d,
                //From = DateTimeOffset.UtcNow.Date,
                To = DateTimeOffset.UtcNow,
            });

            //await host.RunAsync();
        }

        private static void ApplyMigrations(IServiceProvider app)
        {
            using var db = app.GetRequiredService<Services.DataContext>();
            db.Database.Migrate();
        }
    }
}