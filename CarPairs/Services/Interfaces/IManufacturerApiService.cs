using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Core;

public interface IManufacturerApiService
{
    Task<PagedResult<ManufacturerReadDto>?> GetAllAsync();
    Task<ManufacturerReadDto?> GetByIdAsync(int id);
    Task<bool> CreateAsync(ManufacturerCreateDto dto);
    Task<bool> UpdateAsync(int id, ManufacturerUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PagedResult<ManufacturerReadDto>?> SearchAsync(string? name, string? country);
}
