using CarPairs.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPairs.Core.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Manufacturers
                .OrderBy(m => m.Name)
                .Select(m => new SimpleLookupDto
                {
                    Id = m.Id,
                    Name = m.Name!
                })
                .ToListAsync(cancellationToken);
        }

        public Task<bool> UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
