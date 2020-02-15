﻿using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.Services
{
	public static class NavigationServiceExtentions
	{
		public static async Task<T> RequestInputAsync<T>(this INavigationService service, AbstractPopupViewModel<T> requestViewModel)
		{
			var taskCompletionSource = new TaskCompletionSource<T>();
			requestViewModel.SetResult = taskCompletionSource.SetResult;
			service.NavigateForward(requestViewModel);
			var result = await taskCompletionSource.Task;
			service.NavigateBack();
			return result;
		}
	}
}
