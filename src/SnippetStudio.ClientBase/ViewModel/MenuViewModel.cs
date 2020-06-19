using System;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class MenuViewModel : AbstractViewModel
	{
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;
		private bool _isBusy = false;

		private readonly AllSnippetsViewModel _allSnippetsViewModel;

		public RelayCommand NavigateToSnippets => new RelayCommand(async () =>
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
				await _errorService.ShowAlert("Could not load...", ex);
			}
			finally
			{
				_isBusy = false;
			}
		});

		public MenuViewModel(ErrorService errorService,
			AllSnippetsViewModel allSnippetsViewModel,
			INavigationService navigationService)
		{
			_errorService = errorService;
			_allSnippetsViewModel = allSnippetsViewModel;
			_navigationService = navigationService;
		}
	}
}