using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Messages
{
	public class EditProfile
	{
		public ProfileBase Profile { get; }

		public EditProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}