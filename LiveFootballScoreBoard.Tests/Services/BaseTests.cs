
using LiveFootballScoreBoard.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LiveFootballScoreBoard.Tests.Services
{
	[TestClass]
	public class BaseTests<T>
	{
		protected static Mock<ILogger<T>> _loggerMock;

		protected Action<string, Times> VerifyLogger = (string errorMessage, Times times) => _loggerMock.Verify(x => x.Log(
						LogLevel.Error,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((o, t) => string.Equals(errorMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
						It.IsAny<Exception>(),
						(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
						times);
	}
}
