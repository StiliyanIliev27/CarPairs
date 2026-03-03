using CarPairs.Core;
using CarPairs.Web.Services.Interfaces;

namespace CarPairs.Web.Services
{
    public class LookupApiService : ILookupApiService
    {
        private readonly HttpClient _client;

        public LookupApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<SimpleLookupDto>?> GetManufacturersAsync()
        {
            return await _client.GetFromJsonAsync<List<SimpleLookupDto>>("api/manufacturers/lookup");
        }

        public async Task<List<SimpleLookupDto>?> GetCategoriesAsync()
        {
            return await _client.GetFromJsonAsync<List<SimpleLookupDto>>("api/categories/lookup");
        }
    }
}
