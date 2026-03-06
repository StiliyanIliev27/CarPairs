using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;

        public OrganizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Organizations
                .Where(o => o.IsActive)
                .OrderBy(o => o.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CreateAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync(cancellationToken);
            return organization.Id;
        }

        public async Task<bool> UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            _context.Organizations.Update(organization);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var organization = await GetByIdAsync(id, cancellationToken);
            if (organization == null)
                return false;

            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
