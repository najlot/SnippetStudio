using Cosei.Client.RabbitMq;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class RestProfileHandler : AbstractProfileHandler
	{
		private RestProfile _profile;

		private IRequestClient CreateRequestClient()
		{
			return new HttpRequestClient(_profile.ServerName);
		}

		protected override void ApplyProfile(ProfileBase profile)
		{
			if (profile is RestProfile restProfile)
			{
				_profile = restProfile;

				var requestClient = CreateRequestClient();
				var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);

				var snippetStore = new SnippetStore(requestClient, tokenProvider);
				SnippetService = new SnippetService(snippetStore);
			}
		}
	}
}
