using LiveFootballScoreBoard.Models;
using LiveFootballScoreBoard.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace LiveFootballScoreBoard.Tests.Services
{
	[TestClass]
	public class MemoryCacheStorageServiceTests
	{
		private readonly MemoryCacheStorageService _service;
		private readonly IMemoryCache _memoryCache;
		private readonly Mock<ILogger<MemoryCacheStorageService>> _loggerMock;

		private const string MatchesKey = Constants.FOOTBALL_MATCHES_KEY;

		public MemoryCacheStorageServiceTests() 
		{ 
			_memoryCache = new MemoryCache(new MemoryCacheOptions());
			_loggerMock = new Mock<ILogger<MemoryCacheStorageService>>();

			_service = new MemoryCacheStorageService(_memoryCache, _loggerMock.Object);
		}

		[TestMethod]
		[DataRow(null)]
		[DataRow("[{ property: value }]")]
		public void GetItem_ReturnsFootballMatchesByKey(string? cache) 
		{
			// Arrange
			var expectedResult = cache;
			_memoryCache.Set(MatchesKey, expectedResult);

			// Act
			var actualResult = _service.GetItem(MatchesKey);

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestMethod]
		public void GetItem_ReturnsNullWhenKeyDoesNotExist()
		{
			// Arrange
			var expectedResult = default(string?);

			// Act
			var actualResult = _service.GetItem(MatchesKey);

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetItem_ThrowsExceptionWhenMemoryCacheIsNotProvided()
		{
			// Arrange
			var emptyMemoryCache = default(MemoryCache);

			// Act
			var service = new MemoryCacheStorageService(emptyMemoryCache, _loggerMock.Object);

			// Assert
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetItem_ThrowsExceptionWhenLoggerIsNull()
		{
			// Arrange
			var emptyLogger = default(ILogger<MemoryCacheStorageService>);

			// Act
			var service = new MemoryCacheStorageService(_memoryCache, emptyLogger);

			// Assert
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetItem_ThrowsExceptionWhenAnyCacheAndLoggerAreNull()
		{
			// Arrange
			var emptyLogger = default(ILogger<MemoryCacheStorageService>);
			var emptyMemoryCache = default(MemoryCache);

			// Act
			var service = new MemoryCacheStorageService(emptyMemoryCache, emptyLogger);

			// Assert
		}

		[TestMethod]
		[DataRow("footballMatches", "[]")]
		[DataRow("hockeyMatches", null)]
		[DataRow("footballMatches", "[1, 2, 4]")]
		public void UpdateItem_AddsNewItemIfItMissingOrUpdateOne(string key, string? value)
		{
			// Arrange
			var expectedResult = new ExecutionResult { Succeeded = true };
			var expectedEntries = _memoryCache.GetCurrentStatistics().CurrentEntryCount + 2;

			// Act
			var actualResult = _service.UpdateItem(key, value);

			// Assert
			Assert.IsNotNull(actualResult);
			Assert.AreEqual(expectedResult.Succeeded, actualResult.Succeeded);
			Assert.AreEqual(_memoryCache.Get<string?>(key), value);
			Assert.AreEqual(_memoryCache.GetCurrentStatistics().CurrentEntryCount, expectedEntries);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_memoryCache.Dispose();
		}
	}
}
