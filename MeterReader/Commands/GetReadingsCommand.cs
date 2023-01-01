using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MeterReader.Commands
{
    public class GetReadingsCommand : IRequest
    {
        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }
    }

    internal class GetReadingsCommandHandler : AsyncRequestHandler<GetReadingsCommand>
    {
        private readonly Services.IDataContext _dataContext;
        private readonly Services.IHttpService _httpService;
        private readonly ILogger<GetReadingsCommandHandler> _log;
        private readonly Types.Options _options;

        public GetReadingsCommandHandler(
            Services.IDataContext dataContext,
            Services.IHttpService httpService,
            IOptions<Types.Options> options,
            ILogger<GetReadingsCommandHandler> log)
        {
            _dataContext = dataContext;
            _httpService = httpService;
            _log = log;
            _options = options.Value;
        }

        protected override async Task Handle(GetReadingsCommand request, CancellationToken cancellationToken)
        {
            var resources = await _dataContext.Settings
                .Where(s => s.Name.StartsWith("config:resources"))
                .Select(s => s.Value)
                .ToArrayAsync();

            if (!resources.Any())
            {
                throw new ApplicationException("Unable to find any resource IDs. Make sure they have been discovered, or set manually");
            }

            _log.LogInformation("Collecting reading from {startDate:o} to {endDate:o} with {interval}", request.From, request.To, _options.Interval);

            foreach (var item in resources)
            {
                var filter = new Types.ResourceFilter
                {
                    From = request.From,
                    To = request.To,
                    Id = item,
                    Interval = _options.Interval,
                };

                var data = await _httpService.GetReadingsAsync(filter);
                _dataContext.Readings.AddRange(data);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}