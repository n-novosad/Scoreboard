using LiveFootballScoreBoard.Models;
using LiveFootballScoreBoard.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LiveFootballScoreBoard.Tests.Services
{
	[TestClass]
	public class FootballScoreBoardTests
	{
		private const string MatchesKey = Constants.FOOTBALL_MATCHES_KEY;
		private const ushort MatchDuration = Constants.FOOTBALL_MATCH_DURATION_MINS;
		private const ushort MatchDelay = Constants.FOOTBALL_MATCH_OVERDUE_MINS;

		private readonly Mock<IStorageService<string?>> _storageServiceMock;
		private readonly Mock<ILogger<FootballScoreBoardService>> _loggerMock;
		private readonly IFootballScoreBoardService _service;

		public FootballScoreBoardTests()
		{
			_loggerMock = new Mock<ILogger<FootballScoreBoardService>>();
			_storageServiceMock = new Mock<IStorageService<string?>>();

			_service = new FootballScoreBoardService(_storageServiceMock.Object, _loggerMock.Object);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_ThrowsExceptionWhenStorageServiceIsNull()
		{
			// Arrange
			var emptyStorage = default(IStorageService<string?>);

			// Act
			var service = new FootballScoreBoardService(emptyStorage, _loggerMock.Object);

			// Assert
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_ThrowsExceptionWhenLoggerIsNull()
		{
			// Arrange
			var emptyLogger = default(ILogger<FootballScoreBoardService>);

			// Act
			var service = new FootballScoreBoardService(_storageServiceMock.Object, emptyLogger);

			// Assert
		}
	}
}
