using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Services.Implementation;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class LocalProfileHandler : AbstractProfileHandler
	{
		private readonly IMessenger _messenger;
		private readonly IDispatcherHelper _dispatcher;
		private readonly IClipboardService _clipboardService;

		public LocalProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IClipboardService clipboardService)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
			_clipboardService = clipboardService;
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (profile is LocalProfile localProfile)
			{
				var subscriber = new LocalSubscriber();
				var snippetStore = new LocalSnippetStore(localProfile.FolderName, subscriber);
				var csScriptRunService = new CsScriptRunService(_clipboardService);
				var pyScriptRunService = new PyScriptRunService(_clipboardService);
				SnippetService = new SnippetService(snippetStore, _messenger, csScriptRunService, pyScriptRunService, _dispatcher, subscriber, "Me");
				var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
				UserService = new UserService(userStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();

				Subscriber = subscriber;
			}
		}
	}
}
