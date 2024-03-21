using LiveFootballScoreBoard.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LiveFootballScoreBoard.Services
{
	public class MemoryCacheStorageService : IStorageService<string?>
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ILogger<MemoryCacheStorageService> _logger;

		public MemoryCacheStorageService(IMemoryCache memoryCache, ILogger<MemoryCacheStorageService> logger)
        {
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
			_logger = logger;
		}

        public string? GetItem(string id)
		{
			try
			{
				return _memoryCache.Get<string?>(id);
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return null;
			}
		}

		public ExecutionResult RemoveItem(string id)
		{
			throw new NotImplementedException();
		}

		public ExecutionResult UpdateItem(string id, string? item)
		{
			throw new NotImplementedException();
		}
	}
}
