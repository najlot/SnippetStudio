using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Localisation;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LoginViewModel : AbstractViewModel
	{
		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly IProfilesService _profilesService;
		private readonly IProfileHandler _profileHandler;
		private readonly IServiceProvider _serviceProvider;
		private readonly IMessenger _messenger;
		private readonly Func<ProfileViewModel> _createProfileViewModel;
		private readonly Func<LoginProfileViewModel> _createLoginProfileViewModel;
		private IServiceScope _serviceScope;
		private ObservableCollection<LoginProfileViewModel> _loginProfiles = new ObservableCollection<LoginProfileViewModel>();

		public ObservableCollection<LoginProfileViewModel> LoginProfiles
		{
			get => _loginProfiles;
			private set => Set(nameof(LoginProfiles), ref _loginProfiles, value);
		}

		public AsyncCommand CreateProfileCommand { get; }
		private async Task CreateProfileAsync()
		{
			var viewModel = _createProfileViewModel();
			await _navigationService.NavigateForward(viewModel);
		}

		public LoginViewModel(IErrorService errorService,
			INavigationService navigationService,
			IProfilesService profilesService,
			IProfileHandler profileHandler,
			IServiceProvider serviceProvider,
			IMessenger messenger,
			Func<ProfileViewModel> createProfileViewModel,
			Func<LoginProfileViewModel> createLoginProfileViewModel)
		{
			_errorService = errorService;
			_navigationService = navigationService;
			_profilesService = profilesService;
			_profileHandler = profileHandler;
			_serviceProvider = serviceProvider;
			_messenger = messenger;
			_createProfileViewModel = createProfileViewModel;
			_createLoginProfileViewModel = createLoginProfileViewModel;

			_messenger.Register<LoginProfile>(HandleLoginAsync);
			_messenger.Register<EditProfile>(HandleEditAsync);
			_messenger.Register<DeleteProfile>(HandleDeleteAsync);
			_messenger.Register<SaveProfile>(HandleSaveAsync);

			CreateProfileCommand = new AsyncCommand(
				CreateProfileAsync,
				async task => await _errorService.ShowAlertAsync(ProfileLoc.ErrorCreatingProfile, task.Exception));

			LoadProfilesAsync()
				.ContinueWith(
					async task => await _errorService.ShowAlertAsync(ProfileLoc.CouldNotLoadProfiles, task.Exception),
					TaskContinuationOptions.OnlyOnFaulted);
		}

		private async Task LoadProfilesAsync()
		{
			var profiles = await _profilesService.LoadAsync();
			var loginProfiles = new ObservableCollection<LoginProfileViewModel>(
				profiles.Select(profile =>
				{
					var vm = _createLoginProfileViewModel();
					vm.Profile = profile;
					return vm;
				}));

			LoginProfiles = loginProfiles;
		}

		private async Task HandleDeleteAsync(DeleteProfile obj)
		{
			var profile = obj.Profile;
			var profileVm = LoginProfiles.FirstOrDefault(viewModel => viewModel.Profile.Id == profile.Id);

			if (profileVm == null)
			{
				return;
			}

			var message = string.Format(ProfileLoc.DeleteProfile, profileVm.Profile.Name);

			var vm = new YesNoPageViewModel()
			{
				Title = ProfileLoc.DeleteQ,
				Message = message
			};

			var selection = await _navigationService.RequestInputAsync(vm);

			if (selection)
			{
				LoginProfiles.Remove(profileVm);
				await _profilesService.RemoveAsync(profile);
			}
		}

		private async Task HandleEditAsync(EditProfile obj)
		{
			var profile = obj.Profile;
			var viewModel = _createProfileViewModel();
			viewModel.Profile = profile.Clone();
			await _navigationService.NavigateForward(viewModel);
		}

		private async Task HandleSaveAsync(SaveProfile obj)
		{
			var profile = obj.Profile;
			var profileVm = LoginProfiles.FirstOrDefault(vm => vm.Profile.Id == profile.Id);

			if (profileVm != null)
			{
				LoginProfiles.Remove(profileVm);
			}

			profileVm = _createLoginProfileViewModel();
			profileVm.Profile = profile;
			LoginProfiles.Add(profileVm);

			await _profilesService.SaveAsync(LoginProfiles.Select(vm => vm.Profile).ToList());
		}

		private async Task HandleLoginAsync(LoginProfile obj)
		{
			try
			{
				_serviceScope?.Dispose();
				_serviceScope = _serviceProvider.CreateScope();

				await _profileHandler.SetProfile(obj.Profile);
				var viewModel = _serviceScope.ServiceProvider.GetRequiredService<MenuViewModel>();
				await _navigationService.NavigateForward(viewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync(ProfileLoc.CouldNotLogin, ex);
			}
		}
	}
}