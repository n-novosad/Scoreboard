using System.Collections.Concurrent;
using System.Text.Json;
using LiveFootballScoreBoard.Models;
using LiveFootballScoreBoard.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LiveFootballScoreBoard.Tests.Services
{
	[TestClass]
	public class FootballScoreBoardTests : BaseTests<FootballScoreBoardService>
	{
		private const ushort MatchDuration = Constants.FOOTBALL_MATCH_DURATION_MINS;
		private const ushort MatchDelay = Constants.FOOTBALL_MATCH_OVERDUE_MINS;
		private const  string NonDeserializableCacheValue = "{}";
		private const string AwayTeam = "Barcelona";
		private const string HomeTeam = "Madrid";
		private readonly Mock<IStorageService<string?>> _storageServiceMock;
		private readonly IFootballScoreBoardService _service;

		private readonly ConcurrentDictionary<int, FootballMatch> _footballMatches = new()
		{
			[0] = new() { AwayTeam = "Mexico", HomeTeam = "Canada", StartTime = DateTime.Now, Scores = new ( 0, 0 ) },
			[1] = new() { AwayTeam = "Germany", HomeTeam = "France", StartTime = DateTime.Now.AddHours(-1), Scores = new (3, 2) },	
		};


		public FootballScoreBoardTests()
		{
			_loggerMock = new Mock<ILogger<FootballScoreBoardService>>();
			_storageServiceMock = new Mock<IStorageService<string?>>();
			var serializedValue = JsonSerializer.Serialize(_footballMatches);
			_storageServiceMock.Setup(t => t.GetItem(Constants.FOOTBALL_MATCHES_KEY))
				.Returns(serializedValue);

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

		[TestMethod]
		public void Constructor_PreloadsMatchesOnInitialization()
		{
			// Arrange

			// Act
			var service = new FootballScoreBoardService(_storageServiceMock.Object, _loggerMock.Object);

			// Assert
			_storageServiceMock.Verify(t => t.GetItem(Constants.FOOTBALL_MATCHES_KEY), Times.Exactly(2));
		}

		[TestMethod]
		[DataRow(null)]
		[DataRow(NonDeserializableCacheValue)]
		public void Constructor_InitializesEmptyContainerIfNothingInMemory(string? cache)
		{
			// Arrange
			_storageServiceMock.Setup(t => t.GetItem(Constants.FOOTBALL_MATCHES_KEY))
				.Returns(cache);
			// Act
			var service = new FootballScoreBoardService(_storageServiceMock.Object, _loggerMock.Object);

			// Assert
			_storageServiceMock.Verify(t => t.GetItem(Constants.FOOTBALL_MATCHES_KEY), Times.Exactly(2));
		}

		[TestMethod]
		public void StartFootballMatch_ReturnsErrorResponseWhenAnyOfTeamIsNotProvided()
		{
			// Arrange
			var homeTeam = default(string?);
			var expectedResult = new ExecutionResult<int> { Succeeded = false };

			// Act
			var actualResult = _service.StartFootballMatch(homeTeam, AwayTeam);

			// Assert
			Assert.IsFalse(actualResult.Succeeded);
			Assert.AreEqual(Constants.TEAM_CANNOT_BE_EMPTY, actualResult.Error.Message);
		}

		[TestMethod]
		public void StartFootballMatch_ReturnsErrorWhenTwoTeamsAreSame()
		{
			// Arrange
			var expectedResult = new ExecutionResult<int> { Succeeded = false };

			// Act
			var actualResult = _service.StartFootballMatch(AwayTeam, AwayTeam);

			// Assert
			Assert.IsFalse(actualResult.Succeeded);
			Assert.AreEqual(Constants.TEAM_CANNOT_COMPETE_AGAINST_THEMSELVES, actualResult.Error.Message);
		}

		[TestMethod]
		public void StartFootballMatch_ReturnsErrorWhenAnyOfTwoTeamsAlreadyPlaying()
		{
			// Arrange
			var expectedResult = new ExecutionResult<int> { Succeeded = false };

			// Act
			var actualResult = _service.StartFootballMatch(_footballMatches[0].HomeTeam, AwayTeam);

			// Assert
			Assert.IsFalse(actualResult.Succeeded);
			Assert.AreEqual(Constants.TEAM_ALREADY_PLAYING, actualResult.Error.Message);
		}

		[TestMethod]
		public void StartFootballMatch_AugmentScoreboardMatchesWithANewOne()
		{
			// Arrange
			var expectedResult = new ExecutionResult<int> { Succeeded = true };
			var pressumableId = DateTime.UtcNow.Ticks;

			// Act
			var actualResult = _service.StartFootballMatch(HomeTeam, AwayTeam);

			// Assert
			Assert.IsTrue(actualResult.Succeeded);
			Assert.IsTrue(pressumableId <= actualResult.Response);
		}

		[TestMethod]
		public void StartFootballMatch_WhenNewMatchStartedUpdateStorageWithLatestMatches()
		{
			// Arrange
			var expectedResult = new ExecutionResult<int> { Succeeded = true };
			var pressumableId = DateTime.UtcNow.Ticks;
			_storageServiceMock.Setup(t => t.UpdateItem(Constants.FOOTBALL_MATCHES_KEY, It.IsAny<string?>()));

			// Act
			var actualResult = _service.StartFootballMatch($"{HomeTeam}-", $"{AwayTeam}-");

			// Assert
			Assert.IsTrue(actualResult.Succeeded);
			Assert.IsTrue(pressumableId <= actualResult.Response);
			_storageServiceMock.Verify(t => t.UpdateItem(Constants.FOOTBALL_MATCHES_KEY, It.IsAny<string?>()));
		}

		[TestMethod]
		public void UpdateMatchScore_RenewsMatchScoresWithNewOne()
		{
			// Arrange
			var matchId = 0;
			ushort homeTeamScore = 1;
			ushort awayTeamScore = 0;

			// Act
			var actualResult = _service.UpdateMatchScore(matchId, homeTeamScore, awayTeamScore);

			// Assert
			Assert.IsTrue(actualResult.Succeeded);
		}
	}
}
