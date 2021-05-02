using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
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


		private readonly Func<VariableViewModel> _variableViewModelFactory;
		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly IMessenger _messenger;

		private readonly IDiskSearcher _diskSearcher;

		public SnippetModel Item
		{
			get => _item;
			set
			{
				Set(nameof(Item), ref _item, value);

				if (Item.Variables == null)
				{
					_messenger.Register<SnippetLoaded>(Handle);
					Variables = new ObservableCollection<VariableViewModel>();
					RunCommand = new AsyncCommand(LoadAndRunAsync, DisplayError);
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

						var viewModel = _variableViewModelFactory();
						viewModel.ParentId = Item.Id;
						viewModel.Item = model;
						return viewModel;
					}));
				}
			}
		}

		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }

		public SnippetViewModel(
			Func<VariableViewModel> variableViewModelFactory,
			IErrorService errorService,
			INavigationService navigationService,
			IDiskSearcher diskSearcher,
			IMessenger messenger)
		{
			_variableViewModelFactory = variableViewModelFactory;

			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;

			_diskSearcher = diskSearcher;

			SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
			DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);
			EditSnippetCommand = new AsyncCommand(EditSnippetAsync, DisplayError, () => !IsBusy);
			ExportSnippetCommand = new AsyncCommand(ExportSnippetAsync, DisplayError, () => !IsBusy);
			RunCommand = new AsyncCommand(RunAsync, DisplayError);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync("Error...", task.Exception);
		}

		private async Task LoadAndRunAsync()
		{
			await _messenger.SendAsync(new LoadSnippet(Item.Id));
			await RunAsync();
		}

		public AsyncCommand RunCommand { get; private set; }

		private async Task RunAsync()
		{
			try
			{
				var variables = new Dictionary<string, string>();
				var models = new ObservableCollection<VariableModel>(Variables.Select(v =>
				{
					return new VariableModel
					{
						Id = v.Item.Id,
						DefaultValue = v.Item.DefaultValue,
						Name = v.Item.Name,
						RequestName = v.Item.RequestName
					};
				}));

				if (models.Any())
				{
					var requestOk = await _navigationService.RequestInputAsync(new TextInputViewModel(models));

					if (!requestOk)
					{
						return;
					}
				}

				foreach (var variable in models)
				{
					variables[variable.Name] = variable.DefaultValue;
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
				await _errorService.ShowAlertAsync("Error running...", ex);
			}
		}

		public void Handle(SnippetLoaded obj) => Handle((SnippetUpdated)obj);

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

				var viewModel = _variableViewModelFactory();
				viewModel.ParentId = Item.Id;
				viewModel.Item = model;
				return viewModel;
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
					await _messenger.SendAsync(new DeleteSnippet(Item.Id));
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
				await _errorService.ShowAlertAsync("Error starting edit...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand ExportSnippetCommand { get; }
		public async Task ExportSnippetAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var path = await _diskSearcher.SelectSaveFileAsync(Item.Name, "SnippetStudio-File (*.SnippetStudio)|*.SnippetStudio", ".SnippetStudio");

				if (!string.IsNullOrEmpty(path))
				{
					using (var stream = File.OpenWrite(path))
					{
						using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
						{
							var entries = new List<(string name, string data)>(6);
							entries.Add((nameof(Item.Name), Item.Name));
							entries.Add((nameof(Item.Language), Item.Language));
							entries.Add((nameof(Item.Template), Item.Template));
							entries.Add((nameof(Item.Variables), JsonSerializer.Serialize(Item.Variables)));
							entries.Add((nameof(Item.Code), Item.Code));
							
							foreach (var (name, data) in entries)
							{
								var dataEntry = archive.CreateEntry(name, CompressionLevel.Optimal);

								using (var entryStream = dataEntry.Open())
								{
									var bytes = Encoding.UTF8.GetBytes(data);

									using (var memstr = new MemoryStream(bytes))
									{
										memstr.CopyTo(entryStream);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error exporting...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}