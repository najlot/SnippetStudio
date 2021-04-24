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

		public AllSnippetsViewModel(IErrorService errorService,
			ISnippetService snippetService,
			INavigationService navigationService,
			IMessenger messenger)
		{
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

			Snippets.Insert(0, new SnippetViewModel(
				_errorService,
				new Models.SnippetModel()
				{
					Id = obj.Id,
					Name = obj.Name,
					Language = obj.Language,
					Variables = obj.Variables,
					Template = obj.Template,
					Code = obj.Code,
				},
				_navigationService,
				_messenger));
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


			Snippets.Insert(index, new SnippetViewModel(
				_errorService,
				new Models.SnippetModel()
				{
					Id = obj.Id,
					Name = obj.Name,
					Language = obj.Language,
					Variables = obj.Variables,
					Template = obj.Template,
					Code = obj.Code,
				},
				_navigationService,
				_messenger));
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


				var vm = new SnippetViewModel(
					_errorService,
					item,
					_navigationService,
					_messenger);

				_messenger.Register<EditVariable>(vm.Handle);
				_messenger.Register<DeleteVariable>(vm.Handle);
				_messenger.Register<SaveVariable>(vm.Handle);

				_messenger.Register<SnippetUpdated>(vm.Handle);

				await _navigationService.NavigateForward(vm);
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

				var itemVm = new SnippetViewModel(
					_errorService,
					item,
					_navigationService,
					_messenger);

				_messenger.Register<EditVariable>(itemVm.Handle);
				_messenger.Register<DeleteVariable>(itemVm.Handle);
				_messenger.Register<SaveVariable>(itemVm.Handle);

				await _navigationService.NavigateForward(itemVm);
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
					var vm = new SnippetViewModel(
						_errorService,
						item,
						_navigationService,
						_messenger);

					Snippets.Add(vm);
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