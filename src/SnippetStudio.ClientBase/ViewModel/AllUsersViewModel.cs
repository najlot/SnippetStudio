using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class AllUsersViewModel : AbstractViewModel, IDisposable
	{
		private readonly UserService _userService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;
		private readonly ErrorService _errorService;

		private bool _isBusy;
		private ObservableCollection<UserViewModel> _users = new ObservableCollection<UserViewModel>();

		public bool IsBusy
		{
			get => _isBusy;
			private set => Set(nameof(IsBusy), ref _isBusy, value);
		}

		public ObservableCollection<UserViewModel> Users
		{
			get => _users;
			private set => Set(nameof(Users), ref _users, value);
		}

		public AllUsersViewModel(ErrorService errorService,
			UserService userService,
			INavigationService navigationService,
			Messenger messenger)
		{
			_errorService = errorService;
			_userService = userService;
			_navigationService = navigationService;
			_messenger = messenger;

			_messenger.Register<SaveUser>(Handle);
			_messenger.Register<EditUser>(Handle);
			_messenger.Register<DeleteUser>(Handle);

			_messenger.Register<UserCreated>(Handle);
			_messenger.Register<UserUpdated>(Handle);
			_messenger.Register<UserDeleted>(Handle);
		}

		private void Handle(UserCreated obj)
		{

			Users.Insert(0, new UserViewModel(
				_errorService,
				new Models.UserModel()
				{
					Id = obj.Id,
					Username = obj.Username,
					EMail = obj.EMail,
					Password = obj.Password,
				},
				_navigationService,
				_messenger));
		}

		private void Handle(UserUpdated obj)
		{
			var oldItem = Users.FirstOrDefault(i => i.Item.Id == obj.Id);
			var index = -1;

			if (oldItem != null)
			{
				index = Users.IndexOf(oldItem);

				if (index != -1)
				{
					Users.RemoveAt(index);
				}
			}

			if (index == -1)
			{
				index = 0;
			}


			Users.Insert(index, new UserViewModel(
				_errorService,
				new Models.UserModel()
				{
					Id = obj.Id,
					Username = obj.Username,
					EMail = obj.EMail,
					Password = obj.Password,
				},
				_navigationService,
				_messenger));
		}

		private void Handle(UserDeleted obj)
		{
			var oldItem = Users.FirstOrDefault(i => i.Item.Id == obj.Id);

			if (oldItem != null)
			{
				Users.Remove(oldItem);
			}
		}

		private async Task Handle(DeleteUser obj)
		{
			try
			{
				await _userService.DeleteItemAsync(obj.Id);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error saving...", ex);
			}
		}

		private async Task Handle(EditUser obj)
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = await _userService.GetItemAsync(obj.Id);

				// Prevalidate
				item.SetValidation(new UserValidationList(), true);


				var vm = new UserViewModel(
					_errorService,
					item,
					_navigationService,
					_messenger);


				_messenger.Register<UserUpdated>(vm.Handle);

				await _navigationService.NavigateForward(vm);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error loading...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task Handle(SaveUser obj)
		{
			try
			{
				if (Users.Any(i => i.Item.Id == obj.Item.Id))
				{
					await _userService.UpdateItemAsync(obj.Item);
				}
				else
				{
					await _userService.AddItemAsync(obj.Item);
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error saving...", ex);
			}
		}

		public RelayCommand AddUserCommand => new RelayCommand(async () =>
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = _userService.CreateUser();

				// Prevalidate
				item.SetValidation(new UserValidationList(), true);

				var itemVm = new UserViewModel(
					_errorService,
					item,
					_navigationService,
					_messenger);


				await _navigationService.NavigateForward(itemVm);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error adding...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		});

		public async Task RefreshUsersAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				Users.Clear();

				var users = await _userService.GetItemsAsync(true);

				Users = new ObservableCollection<UserViewModel>(users
					.Select(item => new UserViewModel(
						_errorService,
						item,
						_navigationService,
						_messenger)));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error loading data...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public RelayCommand RefreshUsersCommand => new RelayCommand(async () => await RefreshUsersAsync());

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_messenger.Unregister(this);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}