using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core.Services;

public class ManufacturerService : IManufacturerService
{
    private readonly ApplicationDbContext _context;

    public ManufacturerService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<SimpleLookupDto>> GetLookupAsync(int? organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Manufacturers
            .Where(m => !organizationId.HasValue || m.OrganizationId == organizationId.Value)
            .OrderBy(m => m.Name)
            .Select(m => new SimpleLookupDto
             {
                Id = m.Id,
                Name = m.Name!
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Manufacturer>> GetAllAsync(int? organizationId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Manufacturers
            .Where(m => !organizationId.HasValue || m.OrganizationId == organizationId.Value)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Manufacturer>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = items
        };
    }

    public async Task<Manufacturer?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken)
    {
        return await _context.Manufacturers
            .Where(m => !organizationId.HasValue || m.OrganizationId == organizationId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<int> CreateAsync(int? organizationId, Manufacturer manufacturer, CancellationToken cancellationToken)
    {
        manufacturer.CreatedAt = DateTime.UtcNow;
        if (organizationId.HasValue)
            manufacturer.OrganizationId = organizationId.Value;
        _context.Manufacturers.Add(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return manufacturer.Id;
    }

    public async Task<bool> UpdateAsync(int? organizationId, Manufacturer manufacturer, CancellationToken cancellationToken)
    {
        var existing = await _context.Manufacturers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == manufacturer.Id && (!organizationId.HasValue || m.OrganizationId == organizationId.Value), cancellationToken);
        if (existing == null)
            return false;

        manufacturer.OrganizationId = existing.OrganizationId;
        _context.Manufacturers.Update(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken)
    {
        var manufacturer = await _context.Manufacturers
            .FirstOrDefaultAsync(m => m.Id == id && (!organizationId.HasValue || m.OrganizationId == organizationId.Value), cancellationToken);
        if (manufacturer == null)
            return false;

        _context.Manufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}