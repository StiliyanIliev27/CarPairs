namespace CarPairs.Core.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<int> CreateAsync(Organization organization, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Organization organization, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
