using System.Diagnostics.CodeAnalysis;

namespace LiveFootballScoreBoard.Models
{
	[ExcludeFromCodeCoverage]
	public class ExecutionResult<T> : ExecutionResult
	{
        public T? Response { get; set; }
    }

	[ExcludeFromCodeCoverage]
	public class ExecutionResult
	{
        public bool Succeeded { get; set; }
        public Exception? Error { get; set; }
    }
}
