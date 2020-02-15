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
	public partial class SnippetViewModel : AbstractViewModel
	{
		private bool _isBusy;
		public IEnumerable<ILanguage> AvailableLanguages { get; }
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;

		public SnippetModel Item { get; }
		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }

		public SnippetViewModel(
			IEnumerable<ILanguage> languages,
			ErrorService errorService,
			SnippetModel snippetModel,
			INavigationService navigationService,
			Messenger messenger)
		{
			Item = snippetModel;
			AvailableLanguages = languages;
			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;

			if (Item.Dependencies == null)
			{
				Dependencies = new ObservableCollection<DependencyViewModel>();
			}
			else
			{
				Dependencies = new ObservableCollection<DependencyViewModel>(Item.Dependencies.Select(e =>
				{
					var model =  new DependencyModel()
					{
						Id = e.Id,
						Name = e.Name,
					};

					return new DependencyViewModel(_errorService, model, _navigationService, _messenger, Item.Id);
				}));
			}

			if (Item.Variables == null)
			{
				Variables = new ObservableCollection<VariableViewModel>();
			}
			else
			{
				Variables = new ObservableCollection<VariableViewModel>(Item.Variables.Select(e =>
				{
					var model =  new VariableModel()
					{
						Id = e.Id,
						Name = e.Name,
						RequestName = e.RequestName,
						DefaultValue = e.DefaultValue,
					};

					return new VariableViewModel(_errorService, model, _navigationService, _messenger, Item.Id);
				}));
			}
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

				Item.Dependencies = Dependencies.Select(e => new Dependency()
				{
					Id = e.Item.Id,
					Name = e.Item.Name,
				}).ToList();

				Item.Variables = Variables.Select(e => new Variable()
				{
					Id = e.Item.Id,
					Name = e.Item.Name,
					RequestName = e.Item.RequestName,
					DefaultValue = e.Item.DefaultValue,
				}).ToList();

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
				_messenger.Send(new SaveSnippet(Item));
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

		public RelayCommand DeleteCommand => new RelayCommand(async () =>
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
					_navigationService.NavigateBack();
					_messenger.Send(new DeleteSnippet(Item.Id));
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

		public RelayCommand EditSnippetCommand => new RelayCommand(() =>
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				_messenger.Send(new EditSnippet(Item.Id));
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