using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LoginViewModel : AbstractViewModel
	{
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly ProfilesService _profilesService;
		private readonly IProfileHandler _profileHandler;
		private readonly IServiceProvider _serviceProvider;
		private readonly Messenger _messenger;
		private IServiceScope _serviceScope;
		private ObservableCollection<LoginProfileViewModel> _loginProfiles = new ObservableCollection<LoginProfileViewModel>();

		public ObservableCollection<LoginProfileViewModel> LoginProfiles
		{
			get => _loginProfiles;
			private set => Set(nameof(LoginProfiles), ref _loginProfiles, value);
		}

		public RelayCommand CreateProfileCommand => new RelayCommand(() => _navigationService.NavigateForward(new ProfileViewModel(_messenger)));

		public LoginViewModel(ErrorService errorService,
			INavigationService navigationService,
			ProfilesService profilesService,
			IProfileHandler profileHandler,
			IServiceProvider serviceProvider,
			Messenger messenger)
		{
			_errorService = errorService;
			_navigationService = navigationService;
			_profilesService = profilesService;
			_profileHandler = profileHandler;
			_serviceProvider = serviceProvider;
			_messenger = messenger;
			_messenger.Register<LoginProfile>(message => Login(message.Profile));
			_messenger.Register<EditProfile>(message => _navigationService.NavigateForward(new ProfileViewModel(message.Profile.Clone(), _messenger)));
			_messenger.Register<DeleteProfile>(message => Delete(message.Profile));
			_messenger.Register<SaveProfile>(message => Save(message.Profile));

			LoadProfiles();
		}

		private void LoadProfiles()
		{
			try
			{
				var profiles = _profilesService.Load();
				LoginProfiles = new ObservableCollection<LoginProfileViewModel>(profiles.Select(p => new LoginProfileViewModel(p, _messenger)));
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Could not load profiles...", ex);
			}
		}

		private async void Delete(ProfileBase profile)
		{
			var profileVm = LoginProfiles.FirstOrDefault(v => v.Profile.Id == profile.Id);

			if (profileVm == null)
			{
				return;
			}

			var message = "Do you really want to delete profile '" + profileVm.Profile.Name + "'?";

			var vm = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = message
			};

			var selection = await _navigationService.RequestInputAsync(vm);

			if (selection)
			{
				LoginProfiles.Remove(profileVm);
				_profilesService.Remove(profile);
			}
		}

		private void Save(ProfileBase profile)
		{
			var profileVm = LoginProfiles.FirstOrDefault(vm => vm.Profile.Id == profile.Id);

			if (profileVm != null)
			{
				LoginProfiles.Remove(profileVm);
			}

			LoginProfiles.Add(new LoginProfileViewModel(profile, _messenger));

			_profilesService.Save(LoginProfiles.Select(vm => vm.Profile).ToList());
		}

		private void Login(ProfileBase profile)
		{
			try
			{
				_serviceScope?.Dispose();
				_serviceScope = _serviceProvider.CreateScope();

				_profileHandler.SetProfile(profile);
				var menuViewModel = _serviceScope.ServiceProvider.GetRequiredService<MenuViewModel>();
				_navigationService.NavigateForward(menuViewModel);
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Could not login...", ex);
			}
		}
	}
}