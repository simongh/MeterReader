using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MeterReader.Commands
{
    public class StartQuery : IRequest<DateTimeOffset>
    {
    }

    internal class StartQueryHandler : IRequestHandler<StartQuery, DateTimeOffset>
    {
        private readonly Services.IDataContext _dataContext;
        private readonly IConfiguration _configuration;

        public StartQueryHandler(
            Services.IDataContext dataContext,
            IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task<DateTimeOffset> Handle(StartQuery request, CancellationToken cancellationToken)
        {
            var startDate = _configuration.GetValue<DateTimeOffset?>("startdate");

            if (startDate.HasValue)
                return startDate.Value;

            var latest = await _dataContext.Readings
                .OrderByDescending(r => r.Created)
                .Take(1)
                .Select(r => r.Created)
                .FirstOrDefaultAsync();

            if (latest == default)
                latest = DateTimeOffset.UtcNow.AddDays(-2);

            return latest;
        }
    }
}