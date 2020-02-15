using System;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class MenuViewModel : AbstractViewModel
	{
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;

		private readonly AllLanguagesViewModel _allLanguagesViewModel;
		private readonly AllSnippetsViewModel _allSnippetsViewModel;

		public RelayCommand NavigateToLanguages => new RelayCommand(async () =>
		{
			try
			{
				var refreshTask = _allLanguagesViewModel.RefreshLanguagesAsync();
				_navigationService.NavigateForward(_allLanguagesViewModel);
				await refreshTask;
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Could not load...", ex);
			}
		});
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
			AllLanguagesViewModel allLanguagesViewModel,
			AllSnippetsViewModel allSnippetsViewModel,
			INavigationService navigationService)
		{
			_errorService = errorService;
			_allLanguagesViewModel = allLanguagesViewModel;
			_allSnippetsViewModel = allSnippetsViewModel;
			_navigationService = navigationService;
		}
	}
}