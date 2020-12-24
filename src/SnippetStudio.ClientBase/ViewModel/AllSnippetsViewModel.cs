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
		private readonly SnippetService _snippetService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;
		private readonly ErrorService _errorService;

		private bool _isBusy;
		private ObservableCollection<SnippetViewModel> _snippets = new ObservableCollection<SnippetViewModel>();

		public bool IsBusy
		{
			get => _isBusy;
			private set => Set(nameof(IsBusy), ref _isBusy, value);
		}

		public ObservableCollection<SnippetViewModel> Snippets
		{
			get => _snippets;
			private set => Set(nameof(Snippets), ref _snippets, value);
		}

		public AllSnippetsViewModel(ErrorService errorService,
			SnippetService snippetService,
			INavigationService navigationService,
			Messenger messenger)
		{
			_errorService = errorService;
			_snippetService = snippetService;
			_navigationService = navigationService;
			_messenger = messenger;

			_messenger.Register<SaveSnippet>(Handle);
			_messenger.Register<EditSnippet>(Handle);
			_messenger.Register<DeleteSnippet>(Handle);

			_messenger.Register<SnippetCreated>(Handle);
			_messenger.Register<SnippetUpdated>(Handle);
			_messenger.Register<SnippetDeleted>(Handle);

			AddSnippetCommand = new AsyncCommand(AddSnippetAsync, DisplayError);
			RefreshSnippetsCommand = new AsyncCommand(RefreshSnippetsAsync, DisplayError);
			_messenger.Register<RunSnippet>(Handle);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlert("Error...", task.Exception);
		}

		private async Task Handle(RunSnippet obj)
		{
			var result = await _snippetService.Run(obj.Language, obj.Code, obj.Template, obj.Variables);
			await _messenger.SendAsync(new SnippetRun()
			{
				Id = obj.Id,
				Result = result
			});
		}

		private void Handle(SnippetCreated obj)
		{
			if (_snippetService.GetMyName() != obj.CreatedBy)
			{
				return;
			}

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
				await _errorService.ShowAlert("Error saving...", ex);
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
				_messenger.Register<SnippetRun>(vm.Handle);

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
				await _errorService.ShowAlert("Error saving...", ex);
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
				_messenger.Register<SnippetRun>(itemVm.Handle);

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
				Snippets.Clear();

				var snippets = await _snippetService.GetItemsAsync(true);

				Snippets = new ObservableCollection<SnippetViewModel>(snippets
					.Select(item => new SnippetViewModel(
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