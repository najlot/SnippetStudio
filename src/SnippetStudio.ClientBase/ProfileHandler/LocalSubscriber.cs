using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class LocalSubscriber : Messenger, ILocalSubscriber
	{
		public LocalSubscriber()
		{
		}

		public Task DisposeAsync()
		{
			return Task.CompletedTask;
		}

		public Task StartAsync()
		{
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			// Nothing to do
		}
	}
}
