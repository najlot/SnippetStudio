using Cosei.Client.RabbitMq;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class RmqProfileHandler : AbstractProfileHandler
	{
		private RmqProfile _profile;

		private IRequestClient CreateRequestClient()
		{
			return new RabbitMqClient(
				_profile.RabbitMqHost,
				_profile.RabbitMqVirtualHost,
				_profile.RabbitMqUser,
				_profile.RabbitMqPassword,
				"SnippetStudio.Service");
		}

		protected override void ApplyProfile(ProfileBase profile)
		{
			if (profile is RmqProfile rmqProfile)
			{
				_profile = rmqProfile;

				var requestClient = CreateRequestClient();
				var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);

				var snippetStore = new SnippetStore(requestClient, tokenProvider);
				SnippetService = new SnippetService(snippetStore);
			}
		}
	}
}
