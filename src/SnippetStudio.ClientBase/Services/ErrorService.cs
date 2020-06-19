using System;
using System.Threading.Tasks;
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

		public async Task ShowAlert(string message, Exception ex)
		{
			await ShowAlert(message, ex.Message);
		}

		public async Task ShowAlert(Exception ex)
		{
			await ShowAlert(ex.GetType().Name, ex.Message);
		}

		public async Task ShowAlert(string title, string message)
		{
			var vm = new AlertViewModel()
			{
				Title = title,
				Message = message
			};

			await _navigationService.NavigateForward(vm);
		}
	}
}