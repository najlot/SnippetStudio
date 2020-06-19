using Cosei.Client.RabbitMq;
using System;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class RestProfileHandler : AbstractProfileHandler
	{
		private RestProfile _profile;
		private readonly Messenger _messenger;
		private readonly IDispatcherHelper _dispatcher;
		private readonly ErrorService _errorService;

		public RestProfileHandler(Messenger messenger, IDispatcherHelper dispatcher, ErrorService errorService)
		{
			_messenger = messenger;
			_dispatcher = dispatcher;
			_errorService = errorService;
		}

		private IRequestClient CreateRequestClient()
		{
			return new HttpRequestClient(_profile.ServerName);
		}

		protected override async Task ApplyProfile(ProfileBase profile)
		{
			if (profile is RestProfile restProfile)
			{
				_profile = restProfile;

				var requestClient = CreateRequestClient();
				var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);
				
				var token = await tokenProvider.GetToken();

				var serverUri = new Uri(_profile.ServerName);
				var signalRUri = new Uri(serverUri, "/cosei");

				var subscriber = new SignalRSubscriber(signalRUri.AbsoluteUri, 
					options =>
					{
						options.Headers.Add("Authorization", $"Bearer {token}");
					},
					exception =>
					{
						_dispatcher.BeginInvokeOnMainThread(async () => await _errorService.ShowAlert(exception));
					});

				var snippetStore = new SnippetStore(requestClient, tokenProvider);
				SnippetService = new SnippetService(snippetStore, _messenger, _dispatcher, subscriber);

				await subscriber.StartAsync();
			}
		}
	}
}
