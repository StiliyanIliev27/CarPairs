namespace CarPairs.Core.Services.Interfaces
{
    public interface IPartService
    {
        Task<PagedResult<Part>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

        Task<Part?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> CreateAsync(Part part, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(Part part, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
