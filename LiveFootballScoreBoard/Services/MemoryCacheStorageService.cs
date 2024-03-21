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
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

				throw;
			}
		}

		public ExecutionResult RemoveItem(string id)
		{
			try
			{
				_memoryCache.Remove(id);

				return new ExecutionResult { Succeeded = true };
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult { Error = e, Succeeded = false };
			}
		}

		public ExecutionResult UpdateItem(string id, string? item)
		{
			try
			{
				_memoryCache.Set<string?>(id, item);

				return new ExecutionResult { Succeeded = true };
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult { Error = e, Succeeded = false };
			}
		}
	}
}
