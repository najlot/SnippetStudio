using Cosei.Client.Base;
using Cosei.Client.RabbitMq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Services.Implementation;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class RmqProfileHandler : AbstractProfileHandler
	{
		private RmqProfile _profile;
		private RabbitMqModelFactory _rabbitMqModelFactory;
		private readonly IMessenger _messenger;
		private readonly IDispatcherHelper _dispatcher;
		private readonly IErrorService _errorService;

		public RmqProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IErrorService errorService)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
			_errorService = errorService;
		}

		private IRequestClient CreateRequestClient()
		{
			return new RabbitMqClient(_rabbitMqModelFactory, "SnippetStudio.Service");
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
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
						_dispatcher.BeginInvokeOnMainThread(async () => await _errorService.ShowAlertAsync(exception));
					});

				var snippetStore = new SnippetStore(requestClient, tokenProvider);
				SnippetService = new SnippetService(snippetStore, _messenger, _dispatcher, subscriber);
				var userStore = new UserStore(requestClient, tokenProvider);
				UserService = new UserService(userStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();

				Subscriber = subscriber;
			}
		}
	}
}
