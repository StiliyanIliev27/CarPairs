using CarPairs.Web.Services.Interfaces;

namespace CarPairs.Web.Services
{
    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient _client;

        public AdminApiService(HttpClient client)
        {
            _client = client;
        }

        // ──── Dashboard ──────────────────────────────────
        public async Task<AdminStatsDto?> GetStatsAsync()
        {
            return await _client.GetFromJsonAsync<AdminStatsDto>("api/admin/stats");
        }

        // ──── Organizations ──────────────────────────────
        public async Task<List<AdminOrganizationDto>?> GetOrganizationsAsync()
        {
            return await _client.GetFromJsonAsync<List<AdminOrganizationDto>>("api/admin/organizations");
        }

        public async Task<AdminOrganizationDto?> GetOrganizationAsync(int id)
        {
            return await _client.GetFromJsonAsync<AdminOrganizationDto>($"api/admin/organizations/{id}");
        }

        public async Task<bool> CreateOrganizationAsync(OrganizationFormDto dto)
        {
            var response = await _client.PostAsJsonAsync("api/admin/organizations", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrganizationAsync(int id, OrganizationFormDto dto)
        {
            var response = await _client.PutAsJsonAsync($"api/admin/organizations/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrganizationAsync(int id)
        {
            var response = await _client.DeleteAsync($"api/admin/organizations/{id}");
            return response.IsSuccessStatusCode;
        }

        // ──── Users ──────────────────────────────────────
        public async Task<List<AdminUserDto>?> GetUsersAsync()
        {
            return await _client.GetFromJsonAsync<List<AdminUserDto>>("api/admin/users");
        }

        public async Task<bool> UpdateUserAsync(string id, UpdateUserFormDto dto)
        {
            var response = await _client.PutAsJsonAsync($"api/admin/users/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var response = await _client.DeleteAsync($"api/admin/users/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
