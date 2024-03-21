
using System.Diagnostics.CodeAnalysis;

namespace LiveFootballScoreBoard.Models
{
	[ExcludeFromCodeCoverage]
	public static class Constants
	{
		public const ushort FOOTBALL_MATCH_DURATION_MINS = 90;
		public const ushort FOOTBALL_MATCH_OVERDUE_MINS = 30;
		public const string FOOTBALL_MATCHES_KEY = "FootballScoreBoard";
		public const string NULL_VALIDATION_ERROR = "Value cannot be null. (Parameter 'key')";
		public const string TEAM_CANNOT_BE_EMPTY = "Team name should be non empty string";
		public const string TEAM_CANNOT_COMPETE_AGAINST_THEMSELVES = "Teams should have different names";
		public const string TEAM_ALREADY_PLAYING = "Match is going on for at least one of the teams";
		public const string MATCH_SCORES_CAN_BE_ONLY_AUGMENTED = "Match scores cannot be decreased";
	}
}
