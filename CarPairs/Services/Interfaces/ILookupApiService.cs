using CarPairs.Core;

namespace CarPairs.Web.Services.Interfaces
{
    public interface ILookupApiService
    {
        Task<List<SimpleLookupDto>?> GetManufacturersAsync();
        Task<List<SimpleLookupDto>?> GetCategoriesAsync();
    }
}
