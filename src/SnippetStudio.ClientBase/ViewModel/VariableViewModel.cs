using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class VariableViewModel : AbstractViewModel
	{
		private bool _isBusy;
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;
		private readonly Guid _parentId;

		public VariableModel Item { get; }
		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }

		public VariableViewModel(
			ErrorService errorService,
			VariableModel variableModel,
			INavigationService navigationService,
			Messenger messenger,
			Guid parentId)
		{
			Item = variableModel;
			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;
			_parentId = parentId;
		}

		public RelayCommand SaveCommand => new RelayCommand(async () =>
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

				_navigationService.NavigateBack();
				_messenger.Send(new SaveVariable(_parentId, Item));
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		});

		public RelayCommand<bool> DeleteCommand => new RelayCommand<bool>(async navBack =>
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
					if (navBack)
					{
						_navigationService.NavigateBack();
					}

					_messenger.Send(new DeleteVariable(_parentId, Item.Id));
				}
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error deleting...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		});

		public RelayCommand EditVariableCommand => new RelayCommand(() =>
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				_messenger.Send(new EditVariable(_parentId, Item.Id));
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error starting edit...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}, () => !IsBusy);
	}
}