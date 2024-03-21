﻿using LiveFootballScoreBoard.Models;

namespace LiveFootballScoreBoard.Services
{
	public interface IFootballScoreBoardService
	{
		ExecutionResult<int> StartFootballMatch(string homeTeam, string awayTeam);
		ExecutionResult UpdateMatchScore(int matchId, ushort homeTeamScore, ushort awayTeamScore);
		ExecutionResult FinishMatch(int matchId);
		ExecutionResult<IList<FootballMatch>> GetMatchesScoreSummary();
	}
}