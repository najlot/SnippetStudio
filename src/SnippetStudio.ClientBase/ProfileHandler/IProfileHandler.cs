using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public interface IProfileHandler
	{
		ISnippetService GetSnippetService();
		IUserService GetUserService();

		IProfileHandler SetNext(IProfileHandler handler);

		Task SetProfile(ProfileBase profile);
	}
}
