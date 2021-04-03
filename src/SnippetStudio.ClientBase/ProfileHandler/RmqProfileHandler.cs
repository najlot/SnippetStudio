using Cosei.Client.Base;
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
		private RabbitMqModelFactory _rabbitMqModelFactory;
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
			return new RabbitMqClient(_rabbitMqModelFactory, "SnippetStudio.Service");
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (_subscriber != null)
			{
				await _subscriber.DisposeAsync();
				_subscriber = null;
			}

			if (_rabbitMqModelFactory != null)
			{
				_rabbitMqModelFactory.Dispose();
				_rabbitMqModelFactory = null;
			}

			if (profile is RmqProfile rmqProfile)
			{
				_profile = rmqProfile;

				_rabbitMqModelFactory = new RabbitMqModelFactory(
					_profile.RabbitMqHost,
					_profile.RabbitMqVirtualHost,
					_profile.RabbitMqUser,
					_profile.RabbitMqPassword);

				var requestClient = CreateRequestClient();
				var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);
				var subscriber = new RabbitMqSubscriber(
					_rabbitMqModelFactory,
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
