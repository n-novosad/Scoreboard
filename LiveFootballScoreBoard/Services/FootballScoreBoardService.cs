using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
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
			try
			{
				if (_liveFootballMatches.ContainsKey(matchId))
				{
					var target = _liveFootballMatches[matchId];

					if ((DateTime.UtcNow - target.StartTime).TotalMinutes <= Constants.FOOTBALL_MATCH_DURATION_MINS + Constants.FOOTBALL_MATCH_OVERDUE_MINS) 
					{ 
						return new ExecutionResult { Succeeded = false, Error = new ArgumentException(Constants.MATCH_CANNOT_BE_INTERRUPTED) };
					}

					if (_liveFootballMatches.TryRemove(matchId, out var match))
					{
						return new ExecutionResult { Succeeded = true };
					}
				}

				return new ExecutionResult { Succeeded = false, Error = new ArgumentException(Constants.MATCH_WITH_SPECIFIED_ID_NOT_FOUND) };
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult<long> { Error = e, Succeeded = false };
			}
		}

		public ExecutionResult<IList<FootballMatch>> GetMatchesScoreSummary()
		{
			try
			{
				return new ExecutionResult<IList<FootballMatch>> 
				{ 
					Succeeded = true, 
					Response = _liveFootballMatches
									.Values
									.OrderByDescending(t => t.Scores.Item1 + t.Scores.Item2)
									.ThenByDescending(t => t.StartTime)
									.ToList() 
				};
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult<IList<FootballMatch>> { Error = e, Succeeded = false };
			}
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
			try
			{
				if (_liveFootballMatches.TryGetValue(matchId, out var match))
				{
					if ( match.Scores.Item1 > homeTeamScore || match.Scores.Item2 > awayTeamScore)
					{
						return new ExecutionResult { Succeeded = false, Error = new ArgumentException(Constants.MATCH_SCORES_CAN_BE_ONLY_AUGMENTED) };
					}

					if (homeTeamScore > Constants.SCORE_MAX_THRESHOLD || awayTeamScore > Constants.SCORE_MAX_THRESHOLD)
					{
						return new ExecutionResult { Succeeded = false, Error = new ArgumentException(Constants.SCORES_CANNOT_EXCEED_SETTLED_THRESHOLD) };
					}

					match.Scores = new(homeTeamScore, awayTeamScore);

					_storageService.UpdateItem(Constants.FOOTBALL_MATCHES_KEY, JsonSerializer.Serialize(_liveFootballMatches));

					return new ExecutionResult { Succeeded = true };
				}

				return new ExecutionResult { Succeeded = false, Error = new InvalidOperationException() };
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				return new ExecutionResult<long> { Error = e, Succeeded = false };
			}
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
