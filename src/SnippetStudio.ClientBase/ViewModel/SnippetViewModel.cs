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
	public partial class SnippetViewModel : AbstractViewModel
	{
		private bool _isBusy;
		private SnippetModel _item;
		private string result;
		private readonly ErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;

		public SnippetModel Item { get => _item; private set => Set(nameof(Item), ref _item, value); }
		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }

		public AsyncCommand RunCommand { get; }

		public SnippetViewModel(
			ErrorService errorService,
			SnippetModel snippetModel,
			INavigationService navigationService,
			Messenger messenger)
		{
			Item = snippetModel;
			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;

			if (Item.Variables == null)
			{
				Variables = new ObservableCollection<VariableViewModel>();
			}
			else
			{
				Variables = new ObservableCollection<VariableViewModel>(Item.Variables.Select(e =>
				{
					var model = new VariableModel()
					{
						Id = e.Id,
						Name = e.Name,
						RequestName = e.RequestName,
						DefaultValue = e.DefaultValue,
					};

					return new VariableViewModel(_errorService, model, _navigationService, _messenger, Item.Id);
				}));
			}

			RunCommand = new AsyncCommand(RunAsync, DisplayError);

			SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
			DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);
			EditSnippetCommand = new AsyncCommand(EditSnippetAsync, DisplayError, () => !IsBusy);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlert("Error...", task.Exception);
		}

		public string Result
		{
			get => result;
			set => Set(nameof(Result), ref result, value);
		}

		private async Task RunAsync()
		{
			try
			{
				Dictionary<string, string> variables = new Dictionary<string, string>();

				Result = "...";

				foreach (var variable in Variables)
				{
					var (shouldCancel, input) = await _navigationService.RequestInputAsync(new TextInputViewModel()
					{
						Description = variable.Item.RequestName,
						Input = variable.Item.DefaultValue
					});

					if (shouldCancel)
					{
						return;
					}

					variables[variable.Item.Name] = input;
				}

				await _messenger.SendAsync(new RunSnippet(
					Item.Id,
					Item.Language,
					new List<string>(),
					variables,
					Item.Template,
					Item.Code));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error running...", ex);
				Result = "";
			}
		}

		public void Handle(SnippetRun obj)
		{
			if (Item.Id == obj.Id)
			{
				Result = obj.Result;
			}
		}

		public void Handle(SnippetUpdated obj)
		{
			if (Item.Id != obj.Id)
			{
				return;
			}

			Item = new SnippetModel()
			{
				Id = obj.Id,
				Name = obj.Name,
				Language = obj.Language,
				Variables = obj.Variables,
				Template = obj.Template,
				Code = obj.Code,
			};

			Variables = new ObservableCollection<VariableViewModel>(Item.Variables.Select(e =>
			{
				var model = new VariableModel()
				{
					Id = e.Id,
					Name = e.Name,
					RequestName = e.RequestName,
					DefaultValue = e.DefaultValue,
				};

				return new VariableViewModel(_errorService, model, _navigationService, _messenger, Item.Id);
			}));
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

				await _navigationService.NavigateBack();
				await _messenger.SendAsync(new SaveSnippet(Item));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error saving...", ex);
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
					await _messenger.SendAsync(new DeleteSnippet(Item.Id));
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error deleting...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand EditSnippetCommand { get; }
		public async Task EditSnippetAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				await _messenger.SendAsync(new EditSnippet(Item.Id));
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error starting edit...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}