using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.Services
{
	public interface INavigationService
	{
		Task NavigateBack();

		Task NavigateForward(AbstractViewModel newViewModel);
	}
}