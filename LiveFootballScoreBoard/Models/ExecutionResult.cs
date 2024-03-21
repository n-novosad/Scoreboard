namespace LiveFootballScoreBoard.Models
{
	public class ExecutionResult<T> : ExecutionResult
	{
        public T? Response { get; set; }
    }
	public class ExecutionResult
	{
        public bool Succeeded { get; set; }
        public Exception? Error { get; set; }
    }
}
