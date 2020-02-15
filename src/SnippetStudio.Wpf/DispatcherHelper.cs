using System;
using SnippetStudio.ClientBase;

namespace SnippetStudio.Wpf
{
	public class DispatcherHelper : IDispatcherHelper
	{
		public void BeginInvokeOnMainThread(Action action)
		{
			action();
		}
	}
}