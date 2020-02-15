using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LoginProfileViewModel : AbstractViewModel
	{
		private readonly Messenger _messenger;

		public ProfileBase Profile { get; }
		public RelayCommand LoginCommand => new RelayCommand(() => _messenger.Send(new LoginProfile(Profile)));
		public RelayCommand EditCommand => new RelayCommand(() => _messenger.Send(new EditProfile(Profile)));
		public RelayCommand DeleteCommand => new RelayCommand(() => _messenger.Send(new DeleteProfile(Profile)));

		public LoginProfileViewModel(ProfileBase profile, Messenger messenger)
		{
			Profile = profile;
			_messenger = messenger;
		}
	}
}