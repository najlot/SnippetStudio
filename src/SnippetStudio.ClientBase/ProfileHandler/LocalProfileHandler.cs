using Cosei.Client.RabbitMq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class LocalProfileHandler : AbstractProfileHandler
	{
		private readonly Messenger _messenger;
		private readonly IDispatcherHelper _dispatcher;

		public LocalProfileHandler(Messenger messenger, IDispatcherHelper dispatcher)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (profile is LocalProfile localProfile)
			{
				var subscriber = new LocalSubscriber();
				var snippetStore = new LocalSnippetStore(localProfile.FolderName, subscriber);
				SnippetService = new SnippetService(snippetStore, _messenger, _dispatcher, subscriber);
				var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
				UserService = new UserService(userStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();
			}
		}
	}
}
