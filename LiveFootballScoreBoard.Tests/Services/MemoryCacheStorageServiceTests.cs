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
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetItem_InnerExceptionThrownIfKeyIsNull()
		{
			// Arrange
			string? key = null;

			// Act
			var actualResult = _service.GetItem(key);

			// Assert
			_loggerMock.Verify(x => x.Log(
						LogLevel.Error,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((o, t) => string.Equals("Value cannot be null. (Parameter 'key')", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
						It.IsAny<Exception>(),
						(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
						Times.Once);
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

			// Act
			var actualResult = _service.UpdateItem(key, value);

			// Assert
			Assert.IsNotNull(actualResult);
			Assert.AreEqual(expectedResult.Succeeded, actualResult.Succeeded);
			Assert.AreEqual(_memoryCache.Get<string?>(key), value);
		}

		[TestMethod]
		[DataRow(null, "value")]
		public void UpdateItem_ReturnsFailureWhenKeyIsNull(string key, string? value)
		{
			// Arrange
			var succeeded = false;

			// Act
			var actualResult = _service.UpdateItem(key, value);

			// Assert
			Assert.AreEqual(succeeded, actualResult.Succeeded);
			Assert.AreEqual(typeof(ArgumentNullException), actualResult.Error.GetType());
			_loggerMock.Verify(x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((o, t) => string.Equals("Value cannot be null. (Parameter 'key')", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
				Times.Once);
		}

		[TestMethod]
		[DataRow("footballMatches")]
		[DataRow("hockeyMatches")]
		public void RemoveItem_SuccessfullyCleanedUp(string key)
		{
			// Arrange
			var succeeded = true;

			// Act
			var actualResult = _service.RemoveItem(key);

			// Assert
			Assert.AreEqual(succeeded, actualResult.Succeeded);
		}

		[TestMethod]
		public void RemoveItem_FailureOnAccessingItemByNullKey()
		{
			// Arrange
			var succeeded = false;

			// Act
			var actualResult = _service.RemoveItem(null);

			// Assert
			Assert.AreEqual(succeeded, actualResult.Succeeded);
			Assert.AreEqual(typeof(ArgumentNullException), actualResult.Error.GetType());
			_loggerMock.Verify(x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((o, t) => string.Equals("Value cannot be null. (Parameter 'key')", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
				Times.Once);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_memoryCache.Dispose();
		}
	}
}
