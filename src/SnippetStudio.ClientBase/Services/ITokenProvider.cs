using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.Services
{
	public interface ITokenProvider
	{
		Task<string> GetToken();
	}
}
