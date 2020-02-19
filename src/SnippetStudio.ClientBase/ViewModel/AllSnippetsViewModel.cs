using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

			_messenger.Register<SaveSnippet>(this.Handle);
			_messenger.Register<EditSnippet>(this.Handle);
			_messenger.Register<DeleteSnippet>(this.Handle);
		}

		private async void Handle(DeleteSnippet obj)
		{
			try
			{
				var oldItem = Snippets.FirstOrDefault(i => i.Item.Id == obj.Id);

				if (oldItem != null)
				{
					var index = Snippets.IndexOf(oldItem);

					if (index != -1)
					{
						Snippets.RemoveAt(index);
					}
				}

				await _snippetService.DeleteItemAsync(obj.Id);
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}

		private async void Handle(EditSnippet obj)
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

				_messenger.Register<EditDependency>(vm.Handle);
				_messenger.Register<DeleteDependency>(vm.Handle);
				_messenger.Register<SaveDependency>(vm.Handle);
				_messenger.Register<EditVariable>(vm.Handle);
				_messenger.Register<DeleteVariable>(vm.Handle);
				_messenger.Register<SaveVariable>(vm.Handle);

				_navigationService.NavigateForward(vm);
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error loading...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async void Handle(SaveSnippet obj)
		{
			try
			{
				int index = -1;
				var oldItem = Snippets.FirstOrDefault(i => i.Item.Id == obj.Item.Id);

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
					Snippets.Insert(0, new SnippetViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger));

					await _snippetService.AddItemAsync(obj.Item);
				}
				else
				{
					Snippets.Insert(index, new SnippetViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger));

					await _snippetService.UpdateItemAsync(obj.Item);
				}
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}

		public RelayCommand AddSnippetCommand => new RelayCommand(async () =>
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

				_navigationService.NavigateForward(itemVm);
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error adding...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		});

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
				_errorService.ShowAlert("Error loading data...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public RelayCommand RefreshSnippetsCommand => new RelayCommand(async () => await RefreshSnippetsAsync());

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
		}

		#endregion IDisposable Support
	}
}