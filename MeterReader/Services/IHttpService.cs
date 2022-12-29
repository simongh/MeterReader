using MeterReader.Types;

namespace MeterReader.Services
{
    public interface IHttpService
    {
        Task<IEnumerable<Entities.Reading>> GetReadingsAsync(ResourceFilter filter);

        Task<IEnumerable<string>> GetResourcesAsync(string virtualEntityId);

        Task<string> GetVirtualEntitiesAsync();
    }
}