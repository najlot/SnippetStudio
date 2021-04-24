﻿using System;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.ViewModel;

namespace SnippetStudio.ClientBase.Services.Implementation
{
	public class ErrorService : IErrorService
	{
		private readonly INavigationService _navigationService;

		public ErrorService(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public async Task ShowAlertAsync(string message, Exception ex)
		{
			await ShowAlertAsync(message, ex.Message);
		}

		public async Task ShowAlertAsync(Exception ex)
		{
			await ShowAlertAsync(ex.GetType().Name, ex.Message);
		}

		public async Task ShowAlertAsync(string title, string message)
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
