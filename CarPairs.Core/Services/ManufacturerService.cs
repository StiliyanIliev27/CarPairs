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

    public async Task<PagedResult<Manufacturer>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Manufacturers.AsQueryable();

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

    public async Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Manufacturers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken)
    {
        manufacturer.CreatedAt = DateTime.UtcNow;
        _context.Manufacturers.Add(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return manufacturer.Id;
    }

    public async Task<bool> UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken)
    {
        if (!await _context.Manufacturers.AnyAsync(m => m.Id == manufacturer.Id, cancellationToken))
            return false;

        _context.Manufacturers.Update(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var manufacturer = await _context.Manufacturers.FindAsync(new object[] { id }, cancellationToken);
        if (manufacturer == null)
            return false;

        _context.Manufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<Manufacturer>> SearchAsync(string? name, string? country, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Manufacturers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(m => m.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(m => m.Country.Contains(country));

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
}
