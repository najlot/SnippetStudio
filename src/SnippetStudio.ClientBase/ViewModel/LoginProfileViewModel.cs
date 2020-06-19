using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using System;
using System.Windows.Input;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LoginProfileViewModel : AbstractViewModel
	{
		public ProfileBase Profile { get; }

		public ICommand LoginCommand { get; }
		public ICommand EditCommand { get; }
		public ICommand DeleteCommand { get; }

		public LoginProfileViewModel(ProfileBase profile, Messenger messenger)
		{
			Profile = profile;

			LoginCommand = new AsyncCommand(async () => await messenger.SendAsync(new LoginProfile(profile)));
			EditCommand = new AsyncCommand(async () => await messenger.SendAsync(new EditProfile(profile)));
			DeleteCommand = new AsyncCommand(async () => await messenger.SendAsync(new DeleteProfile(profile)));
		}
	}
}