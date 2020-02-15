namespace SnippetStudio.ClientBase.Services
{
	public interface INavigationService
	{
		void NavigateBack();

		void NavigateForward(AbstractViewModel newViewModel);
	}
}