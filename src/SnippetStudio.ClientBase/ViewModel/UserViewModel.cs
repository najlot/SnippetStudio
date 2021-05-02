using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class UserViewModel : AbstractViewModel
	{
		private bool _isBusy;
		private UserModel _item;

		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly IMessenger _messenger;

		public UserModel Item
		{
			get => _item;
			set
			{
				Set(nameof(Item), ref _item, value);
			}
		}

		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }

		public UserViewModel(
			IErrorService errorService,
			INavigationService navigationService,
			IMessenger messenger)
		{
			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;

			SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
			DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);
			EditUserCommand = new AsyncCommand(EditUserAsync, DisplayError, () => !IsBusy);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync("Error...", task.Exception);
		}

		public void Handle(UserUpdated obj)
		{
			if (Item.Id != obj.Id)
			{
				return;
			}

			Item = new UserModel()
			{
				Id = obj.Id,
				Username = obj.Username,
				EMail = obj.EMail,
				Password = obj.Password,
			};
		}

		public AsyncCommand SaveCommand { get; }
		public async Task SaveAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var errors = Item.Errors
					.Where(err => err.Severity > ValidationSeverity.Info)
					.Select(e => e.Text);

				if (errors.Any())
				{
					var message = "There are some validation errors:";
					message += Environment.NewLine + Environment.NewLine;
					message += string.Join(Environment.NewLine, errors);
					message += Environment.NewLine + Environment.NewLine;
					message += "Do you want to continue?";

					var vm = new YesNoPageViewModel()
					{
						Title = "Validation",
						Message = message
					};

					var selection = await _navigationService.RequestInputAsync(vm);

					if (!selection)
					{
						return;
					}
				}

				await _navigationService.NavigateBack();
				await _messenger.SendAsync(new SaveUser(Item));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error saving...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand DeleteCommand { get; }
		public async Task DeleteAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var vm = new YesNoPageViewModel()
				{
					Title = "Delete?",
					Message = "Should the item be deleted?"
				};

				var selection = await _navigationService.RequestInputAsync(vm);

				if (selection)
				{
					await _navigationService.NavigateBack();
					await _messenger.SendAsync(new DeleteUser(Item.Id));
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error deleting...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand EditUserCommand { get; }
		public async Task EditUserAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				await _messenger.SendAsync(new EditUser(Item.Id));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error starting edit...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}