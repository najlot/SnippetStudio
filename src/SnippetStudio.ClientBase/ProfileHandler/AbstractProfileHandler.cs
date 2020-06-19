using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public abstract class AbstractProfileHandler : IProfileHandler
	{
		private IProfileHandler _handler = null;

		protected SnippetService SnippetService { get; set; }

		public SnippetService GetSnippetService() => SnippetService ?? _handler?.GetSnippetService();

		public IProfileHandler SetNext(IProfileHandler handler) => _handler = handler;

		public async Task SetProfile(ProfileBase profile)
		{
			SnippetService?.Dispose();
			SnippetService = null;

			await ApplyProfile(profile);

			if (_handler != null)
			{
				await _handler.SetProfile(profile);
			}
		}

		protected abstract Task ApplyProfile(ProfileBase profile);
	}
}
