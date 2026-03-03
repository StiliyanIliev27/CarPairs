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
        public async Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .OrderBy(m => m.Name)
                .Select(m => new SimpleLookupDto
                {
                    Id = m.Id,
                    Name = m.Name!
                })
                .ToListAsync(cancellationToken);
        }
    }
}
