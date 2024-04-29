using TechBlog.Domain.Gateways.Logger;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Domain.Gateways.MemoryCache;

namespace TechBlog.Infrastructure.MemoryCache
{
    [ExcludeFromCodeCoverage]
    public sealed class MicrosoftMemoryManager : IMemoryCacheManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _persistenceOptions;
        private readonly ILoggerManager _logger;

        public MicrosoftMemoryManager(IMemoryCache memoryCache, MemoryCacheEntryOptions persistenceOptions, ILoggerManager logger)
        {
            _memoryCache = memoryCache;
            _persistenceOptions = persistenceOptions;
            _logger = logger;
        }

        public async Task CreateOrUpdate(Guid requestId, object data, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _logger.Log("Creating or updating request on memory cache", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.RequestEntity, data),
                    (LoggingConstants.RequestId, requestId));

                _memoryCache.Set(requestId, data, _persistenceOptions);

                _logger.Log("Request created or updated on memory cache", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.RequestEntity, data),
                    (LoggingConstants.RequestId, requestId));
            },
            cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid requestId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _memoryCache.TryGetValue<object>(requestId, out _), cancellationToken);
        }

        public async Task<T> GetAsync<T>(Guid requestId, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                if (_memoryCache.TryGetValue<T>(requestId, out var value))
                    return value;

                return default;
            },
            cancellationToken);
        }
    }
}
