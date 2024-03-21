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
				awayTeam = awayTeam?.Trim();
				homeTeam = homeTeam?.Trim();

				if (string.IsNullOrEmpty(homeTeam) || string.IsNullOrEmpty(awayTeam))
				{
					return new ExecutionResult<long> { Succeeded = false, Error = new ArgumentException(Constants.TEAM_CANNOT_BE_EMPTY)};
				}

				if (homeTeam.Equals(awayTeam, StringComparison.InvariantCultureIgnoreCase))
				{
					return new ExecutionResult<long> { Succeeded = false, Error = new ArgumentException(Constants.TEAM_CANNOT_COMPETE_AGAINST_THEMSELVES) };
				}

				if (_liveFootballMatches.Any(t => 
						t.Value.HomeTeam.Equals(homeTeam, StringComparison.InvariantCultureIgnoreCase) 
						|| t.Value.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
						|| t.Value.HomeTeam.Equals(awayTeam, StringComparison.InvariantCultureIgnoreCase) 
						|| t.Value.AwayTeam.Equals(awayTeam, StringComparison.OrdinalIgnoreCase)))
				{
					return new ExecutionResult<long> { Succeeded = false, Error = new ArgumentException(Constants.TEAM_ALREADY_PLAYING) };
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
					_storageService.UpdateItem(Constants.FOOTBALL_MATCHES_KEY, JsonSerializer.Serialize(_liveFootballMatches));

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
