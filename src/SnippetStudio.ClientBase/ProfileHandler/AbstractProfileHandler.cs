using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public abstract class AbstractProfileHandler : IProfileHandler
	{
		private IProfileHandler _next = null;

		protected SnippetService SnippetService { get; set; }

		public SnippetService GetSnippetService() => SnippetService ?? _next?.GetSnippetService();

		public IProfileHandler SetNext(IProfileHandler next) => _next = next;

		public void SetProfile(ProfileBase profile)
		{
			SnippetService?.Dispose();
			SnippetService = null;

			ApplyProfile(profile);

			_next?.SetProfile(profile);
		}

		protected abstract void ApplyProfile(ProfileBase profile);
	}
}
