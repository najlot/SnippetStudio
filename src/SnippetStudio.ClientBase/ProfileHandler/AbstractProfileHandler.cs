using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;
using Cosei.Client.Base;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public abstract class AbstractProfileHandler : IProfileHandler
	{
		private IProfileHandler _handler = null;

		protected ISubscriber Subscriber { get; set; }

		protected ISnippetService SnippetService { get; set; }
		protected IUserService UserService { get; set; }

		public ISnippetService GetSnippetService() => SnippetService ?? _handler?.GetSnippetService();
		public IUserService GetUserService() => UserService ?? _handler?.GetUserService();

		public IProfileHandler SetNext(IProfileHandler handler) => _handler = handler;

		public async Task SetProfile(ProfileBase profile)
		{
			if (Subscriber != null)
			{
				await Subscriber.DisposeAsync();
				Subscriber = null;
			}

			SnippetService?.Dispose();
			SnippetService = null;
			UserService?.Dispose();
			UserService = null;

			await ApplyProfile(profile);

			if (_handler != null)
			{
				await _handler.SetProfile(profile);
			}
		}

		protected abstract Task ApplyProfile(ProfileBase profile);
	}
}
