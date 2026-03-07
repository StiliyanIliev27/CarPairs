using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPairs.Core.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<SimpleLookupDto>> GetLookupAsync(int? organizationId, CancellationToken cancellationToken = default);
        Task<PagedResult<Category>> GetAllAsync(int? organizationId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Category?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken);
        Task<int> CreateAsync(int? organizationId, Category category, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int? organizationId, Category category, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken);
    }
}
