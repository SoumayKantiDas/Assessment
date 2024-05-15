using System.Collections.Generic;
using System.Threading.Tasks;
using Assessment.Models;

namespace Assessment.Services
{
    public interface ICacheService
    {
        Task<IEnumerable<Video>> GetCachedVideosAsync();
        Task CacheVideosAsync(IEnumerable<Video> videos);
        Task ClearCachedVideosAsync();
    }
}
