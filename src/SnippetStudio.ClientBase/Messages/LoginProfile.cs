using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Messages
{
	public class LoginProfile
	{
		public ProfileBase Profile { get; }

		public LoginProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}