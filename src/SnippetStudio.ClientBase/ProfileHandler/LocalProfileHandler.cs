using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ProfileHandler
{
	public sealed class LocalProfileHandler : AbstractProfileHandler
	{
		protected override void ApplyProfile(ProfileBase profile)
		{
			if (profile is LocalProfile localProfile)
			{
				var languageStore = new LocalLanguageStore(localProfile.FolderName);
				LanguageService = new LanguageService(languageStore);
				var snippetStore = new LocalSnippetStore(localProfile.FolderName);
				SnippetService = new SnippetService(snippetStore);
			}
		}
	}
}
