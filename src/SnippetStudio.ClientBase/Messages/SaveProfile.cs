using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveProfile
	{
		public ProfileBase Profile { get; }

		public SaveProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}