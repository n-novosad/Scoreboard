using System.Collections.Concurrent;
using System.Text.Json;
using LiveFootballScoreBoard.Models;
using Microsoft.Extensions.Logging;

namespace LiveFootballScoreBoard.Services
{
	public class FootballScoreBoardService : IFootballScoreBoardService
	{
		private readonly IStorageService<string?> _storageService;
		private readonly ILogger<FootballScoreBoardService> _logger;
		private ConcurrentDictionary<long, FootballMatch> _liveFootballMatches;

		public FootballScoreBoardService(IStorageService<string?> storageService, ILogger<FootballScoreBoardService> logger)
        {
			_storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			PreloadData();
		}

        public ExecutionResult FinishMatch(long matchId)
		{
			throw new NotImplementedException();
		}

		public ExecutionResult<IList<FootballMatch>> GetMatchesScoreSummary()
		{
			throw new NotImplementedException();
		}

		public ExecutionResult<long> StartFootballMatch(string homeTeam, string awayTeam)
		{
			try
			{
				if (string.IsNullOrEmpty(homeTeam?.Trim()) || string.IsNullOrEmpty(awayTeam?.Trim()))
				{
					return new ExecutionResult<long> { Succeeded = false, Error = new ArgumentException(Constants.TEAM_CANNOT_BE_EMPTY)};
				}

				if (homeTeam.Trim().Equals(awayTeam.Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					return new ExecutionResult<long> { Succeeded = false, Error = new ArgumentException(Constants.TEAM_CANNOT_COMPETE_AGAINST_THEMSELVES) };
				}

				var id = DateTime.UtcNow.Ticks;
				var newMatch = new FootballMatch 
				{ 
					AwayTeam = awayTeam, 
					HomeTeam = homeTeam, 
					Scores = new(0, 0), 
					StartTime = DateTime.UtcNow 
				};

				if (_liveFootballMatches.TryAdd(id, newMatch))
				{
					return new ExecutionResult<long> { Succeeded = true, Response = id };
				}

				return new ExecutionResult<long> { Succeeded = false, Error = new InvalidOperationException() };
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult<long> { Error = e, Succeeded = false };
			}
		}

		public ExecutionResult UpdateMatchScore(long matchId, ushort homeTeamScore, ushort awayTeamScore)
		{
			throw new NotImplementedException();
		}

		private void PreloadData()
		{
			try
			{
				_liveFootballMatches = JsonSerializer.Deserialize<ConcurrentDictionary<long, FootballMatch>>(_storageService.GetItem(Constants.FOOTBALL_MATCHES_KEY)) ?? new();
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				_liveFootballMatches = new();
			}
		}
	}
}
