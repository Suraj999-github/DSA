using LRU.LFU.Caching.Configuration;
using LRU.LFU.Caching.Implementations;
using LRU.LFU.Caching.Interfaces;
using LRU.LFU.Caching.Services;

namespace LRU.LFU.Caching.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCachingServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind cache settings
            var cacheSettings = new CacheSettings();
            configuration.GetSection("CacheSettings").Bind(cacheSettings);
            services.AddSingleton(cacheSettings);

            // Register cache implementations
            services.AddSingleton<ILruCache<string, object>>(provider =>
                new LruCache<string, object>(cacheSettings.LruCacheCapacity));

            services.AddSingleton<ILfuCache<string, object>>(provider =>
                new LfuCache<string, object>(cacheSettings.LfuCacheCapacity));

            // Register generic cache factory
            services.AddTransient<ICacheFactory, CacheFactory>();

            return services;
        }
    }
}
