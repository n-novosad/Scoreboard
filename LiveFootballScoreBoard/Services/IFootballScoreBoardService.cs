using LiveFootballScoreBoard.Models;

namespace LiveFootballScoreBoard.Services
{
	public interface IFootballScoreBoardService
	{
		ExecutionResult<long> StartFootballMatch(string homeTeam, string awayTeam);
		ExecutionResult UpdateMatchScore(long matchId, ushort homeTeamScore, ushort awayTeamScore);
		ExecutionResult FinishMatch(long matchId);
		ExecutionResult<IList<FootballMatch>> GetMatchesScoreSummary();
	}
}
