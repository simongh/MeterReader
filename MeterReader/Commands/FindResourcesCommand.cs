using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MeterReader.Commands
{
    public class FindResourcesCommand : IRequest
    {
        public bool Reset { get; set; }
    }

    internal class FindResourcesCommandHandler : AsyncRequestHandler<FindResourcesCommand>
    {
        private readonly Services.IHttpService _httpService;
        private readonly Services.IDataContext _dataContext;

        public FindResourcesCommandHandler(
            Services.IHttpService httpService,
            Services.IDataContext dataContext)
        {
            _httpService = httpService;
            _dataContext = dataContext;
        }

        protected override async Task Handle(FindResourcesCommand request, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Settings
                .Where(s => s.Name.StartsWith("config:resources"))
                .ToArrayAsync();

            if (existing.Any() && !request.Reset)
            {
                return;
            }

            var veId = await _httpService.GetVirtualEntitiesAsync();

            var settings = (await _httpService.GetResourcesAsync(veId))
                .Select((r, index) => new Entities.Setting
                {
                    Name = $"config:resources:{index}",
                    Value = r,
                });

            foreach (var item in settings)
            {
                var existingItem = existing
                    .FirstOrDefault(s => s.Name == item.Name);

                if (existingItem == null)
                    _dataContext.Settings.Add(item);
                else
                    existingItem.Value = item.Value;
            }

            await _dataContext.SaveChangesAsync();

            //foreach (var item in resources)
            //{
            //    var filter = new Types.ResourceFilter
            //    {
            //        From = request.From,
            //        To = request.To,
            //        Id = item,
            //    };

            //    var data = await _httpService.GetReadingsAsync(filter);
            //}
        }
    }
}