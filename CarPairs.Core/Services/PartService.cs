using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core.Services;

public class PartService : IPartService
{
    private readonly ApplicationDbContext _context;

    public PartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Part>> GetAllAsync(
        int? organizationId,
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _context.Parts
            .Where(p => !organizationId.HasValue || p.OrganizationId == organizationId.Value)
            .Include(p => p.Manufacturer)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Part>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = items
        };
    }

    public async Task<Part?> GetByIdAsync(int? organizationId, int id, CancellationToken cancellationToken)
    {
        return await _context.Parts
            .Where(p => !organizationId.HasValue || p.OrganizationId == organizationId.Value)
            .Include(p => p.Manufacturer)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CreateAsync(int? organizationId, Part part, CancellationToken cancellationToken)
    {
        part.CreatedAt = DateTime.UtcNow;
        if (organizationId.HasValue)
            part.OrganizationId = organizationId.Value;
        _context.Parts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);
        return part.Id;
    }

    public async Task<bool> UpdateAsync(int? organizationId, Part part, CancellationToken cancellationToken)
    {
        var existing = await _context.Parts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == part.Id && (!organizationId.HasValue || p.OrganizationId == organizationId.Value), cancellationToken);
        if (existing == null)
            return false;

        part.OrganizationId = existing.OrganizationId;
        _context.Parts.Update(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int? organizationId, int id, CancellationToken cancellationToken)
    {
        var part = await _context.Parts
            .FirstOrDefaultAsync(p => p.Id == id && (!organizationId.HasValue || p.OrganizationId == organizationId.Value), cancellationToken);
        if (part == null)
            return false;

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}