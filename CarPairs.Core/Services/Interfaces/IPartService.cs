namespace CarPairs.Core.Services.Interfaces
{
    public interface IPartService
    {
        Task<PagedResult<Part>> GetAllAsync(
            int? organizationId,
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken);

        Task<Part?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken);

        Task<int> CreateAsync(int? organizationId, Part part, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(int? organizationId, Part part, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken);
    }
}
