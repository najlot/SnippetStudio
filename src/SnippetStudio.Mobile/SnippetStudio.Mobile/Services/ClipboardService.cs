using SnippetStudio.ClientBase.Services;
using System;
using Xamarin.Essentials;

namespace SnippetStudio.Mobile.Services
{
	public class ClipboardService : IClipboardService
	{
		public string GetText()
		{
			if (!Clipboard.HasText)
			{
				return "";
			}

			return Clipboard.GetTextAsync().Result;
		}

		public void SetText(string text)
		{
			Clipboard.SetTextAsync(text).Wait();
		}
	}
}
