using System;

namespace SnippetStudio.ClientBase
{
	public interface IDispatcherHelper
	{
		void BeginInvokeOnMainThread(Action action);
	}
}