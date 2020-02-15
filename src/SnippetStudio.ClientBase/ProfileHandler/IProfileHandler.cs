using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public interface IProfileHandler
	{
		LanguageService GetLanguageService();
		SnippetService GetSnippetService();

		IProfileHandler SetNext(IProfileHandler handler);

		void SetProfile(ProfileBase profile);
	}
}
