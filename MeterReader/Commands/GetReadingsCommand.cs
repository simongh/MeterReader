﻿using MediatR;
using Microsoft.EntityFrameworkCore;

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

        public GetReadingsCommandHandler(
            Services.IDataContext dataContext,
            Services.IHttpService httpService)
        {
            _dataContext = dataContext;
            _httpService = httpService;
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

            foreach (var item in resources)
            {
                var filter = new Types.ResourceFilter
                {
                    From = request.From,
                    To = request.To,
                    Id = item,
                };

                var data = await _httpService.GetReadingsAsync(filter);
                _dataContext.Readings.AddRange(data);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}