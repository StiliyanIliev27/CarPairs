using System.Threading;

namespace CarPairs.Core.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<PagedResult<Manufacturer>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<PagedResult<Manufacturer>> SearchAsync(string? name, string? country, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
