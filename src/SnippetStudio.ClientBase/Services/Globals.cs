using System.Collections.Generic;

namespace SnippetStudio.ClientBase.Services
{
	public class Globals
	{
		private readonly IClipboardService _clipboardService;

		public Globals(IClipboardService clipboardService)
		{
			_clipboardService = clipboardService;
		}

		public string Template { get; set; }

		public Dictionary<string, string> Variables { get; set; }

		public string ClipboardText
		{
			get => _clipboardService.GetText();
			set => _clipboardService.SetText(value);
		}
	}
}