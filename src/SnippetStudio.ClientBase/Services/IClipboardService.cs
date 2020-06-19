namespace SnippetStudio.ClientBase.Services
{
	public interface IClipboardService
	{
		string GetText();
		void SetText(string text);
	}
}