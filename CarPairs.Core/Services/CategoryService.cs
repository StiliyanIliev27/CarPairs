using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SimpleLookupDto>> GetLookupAsync(int? organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => !organizationId.HasValue || c.OrganizationId == organizationId.Value)
                .OrderBy(m => m.Name)
                .Select(m => new SimpleLookupDto
                {
                    Id = m.Id,
                    Name = m.Name!
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Category>> GetAllAsync(int? organizationId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Categories
                .Where(c => !organizationId.HasValue || c.OrganizationId == organizationId.Value)
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = items
            };
        }

        public async Task<Category?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Where(c => !organizationId.HasValue || c.OrganizationId == organizationId.Value)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<int> CreateAsync(int? organizationId, Category category, CancellationToken cancellationToken)
        {
            category.CreatedAt = DateTime.UtcNow;
            if (organizationId.HasValue)
                category.OrganizationId = organizationId.Value;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
        }

        public async Task<bool> UpdateAsync(int? organizationId, Category category, CancellationToken cancellationToken)
        {
            var existing = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.Id && (!organizationId.HasValue || c.OrganizationId == organizationId.Value), cancellationToken);
            if (existing == null)
                return false;

            category.OrganizationId = existing.OrganizationId;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && (!organizationId.HasValue || c.OrganizationId == organizationId.Value), cancellationToken);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
