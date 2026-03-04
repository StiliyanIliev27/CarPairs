using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Core;
using System.Net.Http.Json;

public class ManufacturerApiService : IManufacturerApiService
{
    private readonly HttpClient _client;

    public ManufacturerApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<PagedResult<ManufacturerReadDto>?> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<PagedResult<ManufacturerReadDto>>("api/manufacturers");
    }

    public async Task<ManufacturerReadDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<ManufacturerReadDto>($"api/manufacturers/{id}");
    }

    public async Task<bool> CreateAsync(ManufacturerCreateDto dto)
    {
        var response = await _client.PostAsJsonAsync("api/manufacturers", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(int id, ManufacturerUpdateDto dto)
    {
        var response = await _client.PutAsJsonAsync($"api/manufacturers/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _client.DeleteAsync($"api/manufacturers/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<PagedResult<ManufacturerReadDto>?> SearchAsync(string? name, string? country)
    {
        var url = $"api/manufacturers/search?name={System.Uri.EscapeDataString(name ?? string.Empty)}&country={System.Uri.EscapeDataString(country ?? string.Empty)}";
        return await _client.GetFromJsonAsync<PagedResult<ManufacturerReadDto>>(url);
    }
}
