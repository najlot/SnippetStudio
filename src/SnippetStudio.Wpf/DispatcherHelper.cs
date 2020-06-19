using System;
using System.Threading.Tasks;
using System.Windows;
using SnippetStudio.ClientBase;

namespace SnippetStudio.Wpf
{
	public class DispatcherHelper : IDispatcherHelper
	{
		public void BeginInvokeOnMainThread(Action action)
		{
			Application.Current.Dispatcher.Invoke(action);
		}

		public async Task BeginInvokeOnMainThread(Func<Task> action)
		{
			await Application.Current.Dispatcher.Invoke(action);
		}
	}
}