using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeterReader.Commands
{
    public class StartQuery : IRequest<DateTimeOffset>
    {
    }

    internal class StartQueryHandler : IRequestHandler<StartQuery, DateTimeOffset>
    {
        private readonly Services.IDataContext _dataContext;
        private readonly Types.Options _options;

        public StartQueryHandler(
            Services.IDataContext dataContext,
            IOptions<Types.Options> options)
        {
            _dataContext = dataContext;
            _options = options.Value;
        }

        public async Task<DateTimeOffset> Handle(StartQuery request, CancellationToken cancellationToken)
        {
            var latest = await _dataContext.Readings
                .OrderByDescending(r => r.Created)
                .Take(1)
                .Select(r => r.Created)
                .FirstOrDefaultAsync();

            if (latest == default)
                latest = _options.StartDate;

            return latest;
        }
    }
}