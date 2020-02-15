using System;
using SnippetStudio.ClientBase.ViewModel;

namespace SnippetStudio.ClientBase.Services
{
	public class ErrorService
	{
		private readonly INavigationService _navigationService;

		public ErrorService(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public void ShowAlert(string message, Exception ex)
		{
			ShowAlert(message, ex.Message);
		}

		public void ShowAlert(Exception ex)
		{
			ShowAlert(ex.GetType().Name, ex.Message);
		}

		public void ShowAlert(string title, string message)
		{
			var vm = new AlertViewModel()
			{
				Title = title,
				Message = message
			};

			_navigationService.NavigateForward(vm);
		}
	}
}