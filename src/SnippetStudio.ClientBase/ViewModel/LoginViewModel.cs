using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;

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
			
			_messenger.Register<LoginProfile>(Login);
			_messenger.Register<EditProfile>(Edit);
			_messenger.Register<DeleteProfile>(Delete);
			_messenger.Register<SaveProfile>(Save);

			LoadProfiles().ContinueWith(task => Console.WriteLine(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
		}

		private async Task LoadProfiles()
		{
			try
			{
				var profiles = _profilesService.Load();
				LoginProfiles = new ObservableCollection<LoginProfileViewModel>(profiles.Select(p => new LoginProfileViewModel(p, _messenger)));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Could not load profiles...", ex);
			}
		}

		private async Task Delete(DeleteProfile obj)
		{
			var profile = obj.Profile;
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

		private async Task Edit(EditProfile obj)
		{
			var profile = obj.Profile;
			var vm = new ProfileViewModel(profile.Clone(), _messenger);
			await _navigationService.NavigateForward(vm);
		}

		private void Save(SaveProfile obj)
		{
			var profile = obj.Profile;
			var profileVm = LoginProfiles.FirstOrDefault(vm => vm.Profile.Id == profile.Id);

			if (profileVm != null)
			{
				LoginProfiles.Remove(profileVm);
			}

			LoginProfiles.Add(new LoginProfileViewModel(profile, _messenger));

			_profilesService.Save(LoginProfiles.Select(vm => vm.Profile).ToList());
		}

		private async Task Login(LoginProfile obj)
		{
			try
			{
				var profile = obj.Profile;

				_serviceScope?.Dispose();
				_serviceScope = _serviceProvider.CreateScope();

				await _profileHandler.SetProfile(profile);
				var menuViewModel = _serviceScope.ServiceProvider.GetRequiredService<MenuViewModel>();
				await _navigationService.NavigateForward(menuViewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Could not login...", ex);
			}
		}
	}
}