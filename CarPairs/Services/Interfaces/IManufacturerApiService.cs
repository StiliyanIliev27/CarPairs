using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Core;

namespace CarPairs.Web.Services.Interfaces
{
    public interface IManufacturerApiService
    {
        Task<PagedResult<ManufacturerReadDto>?> GetAllAsync();
        Task<ManufacturerReadDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ManufacturerCreateDto dto);
        Task<bool> UpdateAsync(int id, ManufacturerUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

