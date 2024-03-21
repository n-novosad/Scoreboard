using LiveFootballScoreBoard.Models;

namespace LiveFootballScoreBoard.Services
{
	public interface IStorageService<T>
	{
		T GetItem(string id);
		ExecutionResult UpdateItem(string id, T item);
		ExecutionResult RemoveItem(string id);
	}
}
