using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public interface IProfileHandler
	{
		SnippetService GetSnippetService();
		UserService GetUserService();

		IProfileHandler SetNext(IProfileHandler handler);

		Task SetProfile(ProfileBase profile);
	}
}
