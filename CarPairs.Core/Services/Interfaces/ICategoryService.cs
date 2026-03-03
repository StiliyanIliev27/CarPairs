using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPairs.Core.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    }
}
