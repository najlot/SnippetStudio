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
		private LocalSubscriber _subscriber;
		private readonly IClipboardService _clipboardService;

		public LocalProfileHandler(Messenger messenger, IDispatcherHelper dispatcher, IClipboardService clipboardService)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
			_clipboardService = clipboardService;
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (_subscriber != null)
			{
				await _subscriber.DisposeAsync();
				_subscriber = null;
			}

			if (profile is LocalProfile localProfile)
			{
				var subscriber = new LocalSubscriber();
				var snippetStore = new LocalSnippetStore(localProfile.FolderName, subscriber);
				var csScriptRunService = new CsScriptRunService(_clipboardService);
				SnippetService = new SnippetService(snippetStore, _messenger, csScriptRunService, _dispatcher, subscriber, "Me");
				var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
				UserService = new UserService(userStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();

				_subscriber = subscriber;
			}
		}
	}
}
