using CarPairs.API.DTOs.Parts;
using CarPairs.Core;

public class PartApiService : IPartApiService
{
    private readonly HttpClient _client;

    public PartApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<PagedResult<PartDto>?> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<PagedResult<PartDto>>("api/parts");
    }

    public async Task<PartDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<PartDto>($"api/parts/{id}");
    }

    public async Task<bool> CreateAsync(CreatePartDto dto)
    {
        var response = await _client.PostAsJsonAsync("api/parts", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _client.DeleteAsync($"api/parts/{id}");
        return response.IsSuccessStatusCode;
    }
}