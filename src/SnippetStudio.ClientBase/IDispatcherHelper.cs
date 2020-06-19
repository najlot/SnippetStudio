using System;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase
{
	public interface IDispatcherHelper
	{
		void BeginInvokeOnMainThread(Action action);
		Task BeginInvokeOnMainThread(Func<Task> action);
	}
}