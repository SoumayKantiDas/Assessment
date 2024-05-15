using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Assessment.Models;

namespace Assessment.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<Video>> GetCachedVideosAsync()
        {
            var cachedVideos = await _cache.GetStringAsync("Videos");
            return string.IsNullOrEmpty(cachedVideos) ? null : JsonConvert.DeserializeObject<IEnumerable<Video>>(cachedVideos);
        }

        public async Task CacheVideosAsync(IEnumerable<Video> videos)
        {
            var serializedVideos = JsonConvert.SerializeObject(videos);
            await _cache.SetStringAsync("Videos", serializedVideos, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache expires after 5 minutes
            });
        }

        public async Task ClearCachedVideosAsync()
        {
            await _cache.RemoveAsync("Videos");
        }
    }
}
