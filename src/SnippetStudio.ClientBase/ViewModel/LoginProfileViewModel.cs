using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using System;
using System.Windows.Input;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LoginProfileViewModel : AbstractViewModel
	{
		private ProfileBase _profile;
		private readonly IErrorService _errorService;
		private readonly IDispatcherHelper _dispatcherHelper;

		public ProfileBase Profile
		{
			get => _profile;
			set => Set(nameof(_profile), ref _profile, value);
		}

		public ICommand LoginCommand { get; }
		public ICommand EditCommand { get; }
		public ICommand DeleteCommand { get; }

		public LoginProfileViewModel(IMessenger messenger, IErrorService errorService, IDispatcherHelper dispatcherHelper)
		{
			LoginCommand = new AsyncCommand(async () => await messenger.SendAsync(new LoginProfile(Profile)), DisplayErrorAsync);
			EditCommand = new AsyncCommand(async () => await messenger.SendAsync(new EditProfile(Profile)), DisplayErrorAsync);
			DeleteCommand = new AsyncCommand(async () => await messenger.SendAsync(new DeleteProfile(Profile)), DisplayErrorAsync);
			_errorService = errorService;
			_dispatcherHelper = dispatcherHelper;
		}

		private async Task DisplayErrorAsync(Task task)
		{
			await _dispatcherHelper.BeginInvokeOnMainThread(async () => await _errorService.ShowAlertAsync(task.Exception));
		}
	}
}