using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class ProfileViewModel : AbstractViewModel
	{
		private ProfileBase profile;
		private readonly Messenger _messenger;

		public ProfileBase Profile { get => profile; private set => Set(nameof(Profile), ref profile, value); }

		public Source Source
		{
			get => profile.Source;
			set
			{
				if (profile.Source != value)
				{
					ProfileBase newProfile = null;

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

		public ProfileViewModel(Messenger messenger)
		{
			var id = Guid.NewGuid();

			Profile = new LocalProfile
			{
				Id = id,
				Name = "New Profile",
				FolderName = id.ToString()
			};

			_messenger = messenger;
		}

		public ProfileViewModel(ProfileBase profile, Messenger messenger)
		{
			Profile = profile;
			_messenger = messenger;
		}

		public RelayCommand SaveCommand => new RelayCommand(() => _messenger.Send(new SaveProfile(Profile)));
	}
}