using System;
using System.Threading.Tasks;
using SnippetStudio.ClientBase;
using Xamarin.Forms;

namespace SnippetStudio.Mobile
{
	public class DispatcherHelper : IDispatcherHelper
	{
		public void BeginInvokeOnMainThread(Action action)
		{
			Device.BeginInvokeOnMainThread(action);
		}

		public async Task BeginInvokeOnMainThread(Func<Task> action)
		{
			var tcs = new TaskCompletionSource<object>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await action();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            await tcs.Task;
        }
	}
}