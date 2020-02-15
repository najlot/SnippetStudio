using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Messages
{
	public class DeleteProfile
	{
		public ProfileBase Profile { get; }

		public DeleteProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}