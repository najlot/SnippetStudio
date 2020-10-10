using Cosei.Client.RabbitMq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class RmqProfileHandler : AbstractProfileHandler
	{
		private RmqProfile _profile;
		private RabbitMqSubscriber _subscriber;
		private readonly Messenger _messenger;
		private readonly IDispatcherHelper _dispatcher;
		private readonly ErrorService _errorService;
		private readonly IClipboardService _clipboardService;

		public RmqProfileHandler(Messenger messenger, IDispatcherHelper dispatcher, ErrorService errorService, IClipboardService clipboardService)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
			_errorService = errorService;
			_clipboardService = clipboardService;
		}

		private IRequestClient CreateRequestClient()
		{
			return new RabbitMqClient(
				_profile.RabbitMqHost,
				_profile.RabbitMqVirtualHost,
				_profile.RabbitMqUser,
				_profile.RabbitMqPassword,
				"SnippetStudio.Service");
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (_subscriber != null)
			{
				await _subscriber.DisposeAsync();
				_subscriber = null;
			}

			if (profile is RmqProfile rmqProfile)
			{
				_profile = rmqProfile;

				var requestClient = CreateRequestClient();
				var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);
				var subscriber = new RabbitMqSubscriber(
					_profile.RabbitMqHost,
					_profile.RabbitMqVirtualHost,
					_profile.RabbitMqUser,
					_profile.RabbitMqPassword,
					exception =>
					{
						_dispatcher.BeginInvokeOnMainThread(async () => await _errorService.ShowAlert(exception));
					});

				var snippetStore = new SnippetStore(requestClient, tokenProvider);
				var csScriptRunService = new CsScriptRunService(_clipboardService);
				SnippetService = new SnippetService(snippetStore, _messenger, csScriptRunService, _dispatcher, subscriber, _profile.ServerUser);
				var userStore = new UserStore(requestClient, tokenProvider);
				UserService = new UserService(userStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();

				_subscriber = subscriber;
			}
		}
	}
}
