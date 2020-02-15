using System;
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
	}
}