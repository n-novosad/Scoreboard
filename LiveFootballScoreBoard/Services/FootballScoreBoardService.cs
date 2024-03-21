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
		private ConcurrentDictionary<int, FootballMatch> _liveFootballMatches;

		public FootballScoreBoardService(IStorageService<string?> storageService, ILogger<FootballScoreBoardService> logger)
        {
			_storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			PreloadData();
		}

        public ExecutionResult FinishMatch(int matchId)
		{
			throw new NotImplementedException();
		}

		public ExecutionResult<IList<FootballMatch>> GetMatchesScoreSummary()
		{
			throw new NotImplementedException();
		}

		public ExecutionResult<int> StartFootballMatch(string homeTeam, string awayTeam)
		{
			throw new NotImplementedException();
		}

		public ExecutionResult UpdateMatchScore(int matchId, ushort homeTeamScore, ushort awayTeamScore)
		{
			throw new NotImplementedException();
		}

		private void PreloadData()
		{
			try
			{
				_liveFootballMatches = JsonSerializer.Deserialize<ConcurrentDictionary<int, FootballMatch>>(_storageService.GetItem(Constants.FOOTBALL_MATCHES_KEY)) ?? new();
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message, e);

				_liveFootballMatches = new();
			}
		}
	}
}
