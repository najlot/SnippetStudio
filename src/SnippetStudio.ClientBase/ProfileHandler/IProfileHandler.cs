using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public interface IProfileHandler
	{
		SnippetService GetSnippetService();

		IProfileHandler SetNext(IProfileHandler handler);

		void SetProfile(ProfileBase profile);
	}
}
