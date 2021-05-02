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
	public class AllSnippetsViewModel : AbstractViewModel, IDisposable
	{
		private readonly Func<SnippetViewModel> _snippetViewModelFactory;
		private readonly ISnippetService _snippetService;
		private readonly INavigationService _navigationService;
		private readonly IMessenger _messenger;
		private readonly IErrorService _errorService;

		private bool _isBusy;
		private string _filter;

		public bool IsBusy
		{
			get => _isBusy;
			private set => Set(nameof(IsBusy), ref _isBusy, value);
		}

		public string Filter
		{
			get => _filter;
			set
			{
				Set(nameof(Filter), ref _filter, value);
				SnippetsView.Refresh();
			}
		}

		public ObservableCollectionView<SnippetViewModel> SnippetsView { get; }
		public ObservableCollection<SnippetViewModel> Snippets { get; } = new ObservableCollection<SnippetViewModel>();

		public AllSnippetsViewModel(
			Func<SnippetViewModel> snippetViewModelFactory,
			IErrorService errorService,
			ISnippetService snippetService,
			INavigationService navigationService,
			IMessenger messenger)
		{
			_snippetViewModelFactory = snippetViewModelFactory;
			_errorService = errorService;
			_snippetService = snippetService;
			_navigationService = navigationService;
			_messenger = messenger;

			SnippetsView = new ObservableCollectionView<SnippetViewModel>(Snippets, FilterSnippet);

			_messenger.Register<SaveSnippet>(Handle);
			_messenger.Register<EditSnippet>(Handle);
			_messenger.Register<DeleteSnippet>(Handle);

			_messenger.Register<SnippetCreated>(Handle);
			_messenger.Register<SnippetUpdated>(Handle);
			_messenger.Register<SnippetDeleted>(Handle);

			AddSnippetCommand = new AsyncCommand(AddSnippetAsync, DisplayError);
			RefreshSnippetsCommand = new AsyncCommand(RefreshSnippetsAsync, DisplayError);
		}

		private bool FilterSnippet(SnippetViewModel arg)
		{
			if (string.IsNullOrEmpty(Filter))
			{
				return true;
			}

			var item = arg.Item;

			var name = item.Name;
			if (!string.IsNullOrEmpty(name) && name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			var language = item.Language;
			if (!string.IsNullOrEmpty(language) && language.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			var template = item.Template;
			if (!string.IsNullOrEmpty(template) && template.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			var code = item.Code;
			if (!string.IsNullOrEmpty(code) && code.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			return false;
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync("Error...", task.Exception);
		}

		private void Handle(SnippetCreated obj)
		{
			var viewModel = _snippetViewModelFactory();


			viewModel.Item = new Models.SnippetModel()
			{
				Id = obj.Id,
				Name = obj.Name,
				Language = obj.Language,
				Variables = obj.Variables,
				Template = obj.Template,
				Code = obj.Code,
			};

			Snippets.Insert(0, viewModel);
		}

		private void Handle(SnippetUpdated obj)
		{
			var oldItem = Snippets.FirstOrDefault(i => i.Item.Id == obj.Id);
			var index = -1;

			if (oldItem != null)
			{
				index = Snippets.IndexOf(oldItem);

				if (index != -1)
				{
					Snippets.RemoveAt(index);
				}
			}

			if (index == -1)
			{
				index = 0;
			}

			var viewModel = _snippetViewModelFactory();


			viewModel.Item = new Models.SnippetModel()
			{
				Id = obj.Id,
				Name = obj.Name,
				Language = obj.Language,
				Variables = obj.Variables,
				Template = obj.Template,
				Code = obj.Code,
			};

			Snippets.Insert(index, viewModel);
		}

		private void Handle(SnippetDeleted obj)
		{
			var oldItem = Snippets.FirstOrDefault(i => i.Item.Id == obj.Id);

			if (oldItem != null)
			{
				Snippets.Remove(oldItem);
			}
		}

		private async Task Handle(DeleteSnippet obj)
		{
			try
			{
				await _snippetService.DeleteItemAsync(obj.Id);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error saving...", ex);
			}
		}

		private async Task Handle(EditSnippet obj)
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = await _snippetService.GetItemAsync(obj.Id);

				// Prevalidate
				item.SetValidation(new SnippetValidationList(), true);

				var viewModel = _snippetViewModelFactory();


				viewModel.Item = item;

				_messenger.Register<EditVariable>(viewModel.Handle);
				_messenger.Register<DeleteVariable>(viewModel.Handle);
				_messenger.Register<SaveVariable>(viewModel.Handle);

				_messenger.Register<SnippetUpdated>(viewModel.Handle);

				await _navigationService.NavigateForward(viewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error loading...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task Handle(SaveSnippet obj)
		{
			try
			{
				if (Snippets.Any(i => i.Item.Id == obj.Item.Id))
				{
					await _snippetService.UpdateItemAsync(obj.Item);
				}
				else
				{
					await _snippetService.AddItemAsync(obj.Item);
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error saving...", ex);
			}
		}

		public AsyncCommand AddSnippetCommand { get; }
		public async Task AddSnippetAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = _snippetService.CreateSnippet();

				// Prevalidate
				item.SetValidation(new SnippetValidationList(), true);

				var viewModel = _snippetViewModelFactory();


				viewModel.Item = item;

				_messenger.Register<EditVariable>(viewModel.Handle);
				_messenger.Register<DeleteVariable>(viewModel.Handle);
				_messenger.Register<SaveVariable>(viewModel.Handle);

				await _navigationService.NavigateForward(viewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error adding...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand RefreshSnippetsCommand { get; }
		public async Task RefreshSnippetsAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				SnippetsView.Disable();
				Filter = "";

				Snippets.Clear();

				var snippets = await _snippetService.GetItemsAsync(true);

				foreach (var item in snippets)
				{
					var viewModel = _snippetViewModelFactory();


					viewModel.Item = item;

					Snippets.Add(viewModel);
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error loading data...", ex);
			}
			finally
			{
				SnippetsView.Enable();
				IsBusy = false;
			}
		}

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