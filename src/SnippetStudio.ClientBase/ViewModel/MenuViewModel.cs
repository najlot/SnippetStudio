using System;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class MenuViewModel : AbstractViewModel
	{
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;

		private readonly AllSnippetsViewModel _allSnippetsViewModel;

		public RelayCommand NavigateToSnippets => new RelayCommand(async () =>
		{
			try
			{
				var refreshTask = _allSnippetsViewModel.RefreshSnippetsAsync();
				_navigationService.NavigateForward(_allSnippetsViewModel);
				await refreshTask;
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Could not load...", ex);
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