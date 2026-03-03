using CarPairs.API.DTOs.Parts;
using CarPairs.Core;

public interface IPartApiService
{
    Task<PagedResult<PartDto>?> GetAllAsync();
    Task<PartDto?> GetByIdAsync(int id);
    Task<bool> CreateAsync(CreatePartDto dto);
    Task<bool> DeleteAsync(int id);
}