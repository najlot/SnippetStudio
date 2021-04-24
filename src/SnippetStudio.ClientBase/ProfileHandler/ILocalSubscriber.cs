using Cosei.Client.Base;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public interface ILocalSubscriber : ISubscriber
	{
		Task SendAsync<T>(T message) where T : class;
	}
}
