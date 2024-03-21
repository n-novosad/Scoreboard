
using System.Diagnostics.CodeAnalysis;

namespace LiveFootballScoreBoard.Models
{
	[ExcludeFromCodeCoverage]
	public static class Constants
	{
		public const ushort FOOTBALL_MATCH_DURATION_MINS = 90;
		public const ushort FOOTBALL_MATCH_OVERDUE_MINS = 30;
		public const string FOOTBALL_MATCHES_KEY = "FootballScoreBoard";
	}
}
