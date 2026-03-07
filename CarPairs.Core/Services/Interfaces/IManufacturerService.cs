using System.Threading;

namespace CarPairs.Core.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<List<SimpleLookupDto>> GetLookupAsync(int? organizationId, CancellationToken cancellationToken = default);
        Task<PagedResult<Manufacturer>> GetAllAsync(int? organizationId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Manufacturer?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken);
        Task<int> CreateAsync(int? organizationId, Manufacturer manufacturer, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int? organizationId, Manufacturer manufacturer, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken);
    }
}
