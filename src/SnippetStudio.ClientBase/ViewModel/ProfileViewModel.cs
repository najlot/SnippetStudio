using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Localisation;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class ProfileViewModel : AbstractViewModel
	{
		private ProfileBase profile;
		private readonly IMessenger _messenger;
		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;

		public ProfileBase Profile { get => profile; set => Set(nameof(Profile), ref profile, value); }

		public Source Source
		{
			get => profile.Source;
			set
			{
				if (profile.Source != value)
				{
					ProfileBase newProfile;

					switch (value)
					{
						case Source.Local:
							newProfile = new LocalProfile();
							break;

						case Source.RMQ:
							newProfile = new RmqProfile();
							break;

						case Source.REST:
							newProfile = new RestProfile();
							break;

						default:
							throw new NotImplementedException(value.ToString());
					}

					newProfile.Id = Profile.Id;
					newProfile.Name = Profile.Name;
					newProfile.Source = value;

					Profile = newProfile;
				}

				profile.Source = value;
			}
		}

		public List<Source> PossibleSources { get; } = new List<Source>(Enum.GetValues(typeof(Source)) as Source[]);

		public ProfileViewModel(IMessenger messenger, IErrorService errorService, INavigationService navigationService)
		{
			var id = Guid.NewGuid();

			Profile = new LocalProfile
			{
				Id = id,
				Name = ProfileLoc.NewProfile,
				FolderName = id.ToString()
			};

			_messenger = messenger;
			_errorService = errorService;
			_navigationService = navigationService;

			SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
		}

		public AsyncCommand SaveCommand { get; }
		public async Task SaveAsync()
		{
			await _messenger.SendAsync(new SaveProfile(Profile));
			await _navigationService.NavigateBack();
		}
	}
}