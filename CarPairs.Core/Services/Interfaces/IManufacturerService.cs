namespace CarPairs.Core.Interfaces;

public interface IManufacturerService
{
    Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}