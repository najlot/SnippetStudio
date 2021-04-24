using System;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class MenuViewModel : AbstractViewModel
	{
		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private bool _isBusy = false;

		private readonly AllSnippetsViewModel _allSnippetsViewModel;
		private readonly AllUsersViewModel _allUsersViewModel;

		public AsyncCommand NavigateToSnippets { get; }
		public async Task NavigateToSnippetsAsync()
		{
			if (_isBusy)
			{
				return;
			}

			try
			{
				_isBusy = true;

				var refreshTask = _allSnippetsViewModel.RefreshSnippetsAsync();
				await _navigationService.NavigateForward(_allSnippetsViewModel);
				await refreshTask;
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Could not load...", ex);
			}
			finally
			{
				_isBusy = false;
			}
		}

		public AsyncCommand NavigateToUsers { get; }
		public async Task NavigateToUsersAsync()
		{
			if (_isBusy)
			{
				return;
			}

			try
			{
				_isBusy = true;

				var refreshTask = _allUsersViewModel.RefreshUsersAsync();
				await _navigationService.NavigateForward(_allUsersViewModel);
				await refreshTask;
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Could not load...", ex);
			}
			finally
			{
				_isBusy = false;
			}
		}

		public MenuViewModel(IErrorService errorService,
			AllSnippetsViewModel allSnippetsViewModel,
			AllUsersViewModel allUsersViewModel,
			INavigationService navigationService)
		{
			_errorService = errorService;
			_allSnippetsViewModel = allSnippetsViewModel;
			_allUsersViewModel = allUsersViewModel;
			_navigationService = navigationService;

			NavigateToSnippets = new AsyncCommand(NavigateToSnippetsAsync, DisplayError);
			NavigateToUsers = new AsyncCommand(NavigateToUsersAsync, DisplayError);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync("Error...", task.Exception);
		}
	}
}