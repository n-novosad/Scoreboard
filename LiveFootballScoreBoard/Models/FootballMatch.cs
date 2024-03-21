
using System.Diagnostics.CodeAnalysis;

namespace LiveFootballScoreBoard.Models
{
	[ExcludeFromCodeCoverage]
	public class FootballMatch
	{
        public DateTime StartTime { get; set; }
        public ValueTuple<ushort, ushort> Scores { get; set; }
        public required string HomeTeam { get; set; }
        public required string AwayTeam { get; set; }
    }
}
