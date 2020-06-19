using SnippetStudio.ClientBase.Services;
using System.Threading;
using System.Windows;

namespace SnippetStudio.Wpf.Services
{
	public class ClipboardService : IClipboardService
	{
		public string GetText()
		{
			var text = "";

			var thread = new Thread(() =>
			{
				text = Clipboard.GetText();
			});

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();

			return text;
		}

		public void SetText(string text)
		{
			var thread = new Thread(() =>
			{
				Clipboard.SetText(text);
			});

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}
	}
}
