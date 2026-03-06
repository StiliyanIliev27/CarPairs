using CarPairs.Web.Services.Interfaces;

namespace CarPairs.Web.Services
{
    public class AccountApiService : IAccountApiService
    {
        private readonly HttpClient _client;

        public AccountApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _client.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
