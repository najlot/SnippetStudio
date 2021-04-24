using System;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase
{
	public interface IMessenger
	{
		void Register<T>(Action<T> handler) where T : class;
		void Register<T>(Func<T, Task> handler) where T : class;
		Task SendAsync<T>(T message) where T : class;
		void Unregister<T>(T obj) where T : class;
	}
}