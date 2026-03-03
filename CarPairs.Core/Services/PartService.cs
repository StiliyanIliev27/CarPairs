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
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _context.Parts
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

    public async Task<Part?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Parts
            .Include(p => p.Manufacturer)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CreateAsync(Part part, CancellationToken cancellationToken)
    {
        part.CreatedAt = DateTime.UtcNow;
        _context.Parts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);
        return part.Id;
    }

    public async Task<bool> UpdateAsync(Part part, CancellationToken cancellationToken)
    {
        if (!await _context.Parts.AnyAsync(p => p.Id == part.Id, cancellationToken))
            return false;

        _context.Parts.Update(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var part = await _context.Parts.FindAsync(new object[] { id }, cancellationToken);
        if (part == null)
            return false;

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}