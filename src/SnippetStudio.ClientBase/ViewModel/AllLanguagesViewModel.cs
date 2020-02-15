using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class AllLanguagesViewModel : AbstractViewModel, IDisposable
	{
		private readonly LanguageService _languageService;
		private readonly INavigationService _navigationService;
		private readonly Messenger _messenger;
		private readonly ErrorService _errorService;

		private bool _isBusy;
		private ObservableCollection<LanguageViewModel> _languages = new ObservableCollection<LanguageViewModel>();

		public bool IsBusy
		{
			get => _isBusy;
			private set => Set(nameof(IsBusy), ref _isBusy, value);
		}

		public ObservableCollection<LanguageViewModel> Languages
		{
			get => _languages;
			private set => Set(nameof(Languages), ref _languages, value);
		}

		public AllLanguagesViewModel(ErrorService errorService,
			LanguageService languageService,
			INavigationService navigationService,
			Messenger messenger)
		{
			_errorService = errorService;
			_languageService = languageService;
			_navigationService = navigationService;
			_messenger = messenger;

			_messenger.Register<SaveLanguage>(this.Handle);
			_messenger.Register<EditLanguage>(this.Handle);
			_messenger.Register<DeleteLanguage>(this.Handle);
		}

		private async void Handle(DeleteLanguage obj)
		{
			try
			{
				var oldItem = Languages.FirstOrDefault(i => i.Item.Id == obj.Id);

				if (oldItem != null)
				{
					var index = Languages.IndexOf(oldItem);

					if (index != -1)
					{
						Languages.RemoveAt(index);
					}
				}

				await _languageService.DeleteItemAsync(obj.Id);
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}

		private async void Handle(EditLanguage obj)
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = await _languageService.GetItemAsync(obj.Id);

				// Prevalidate
				item.SetValidation(new LanguageValidationList(), true);


				var vm = new LanguageViewModel(
					_errorService,
					item,
					_navigationService,
					_messenger);


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

		private async void Handle(SaveLanguage obj)
		{
			try
			{
				int index = -1;
				var oldItem = Languages.FirstOrDefault(i => i.Item.Id == obj.Item.Id);

				if (oldItem != null)
				{
					index = Languages.IndexOf(oldItem);

					if (index != -1)
					{
						Languages.RemoveAt(index);
					}
				}


				if (index == -1)
				{
					Languages.Insert(0, new LanguageViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger));

					await _languageService.AddItemAsync(obj.Item);
				}
				else
				{
					Languages.Insert(index, new LanguageViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger));

					await _languageService.UpdateItemAsync(obj.Item);
				}
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}

		public RelayCommand AddLanguageCommand => new RelayCommand(async () =>
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = _languageService.CreateLanguage();

				// Prevalidate
				item.SetValidation(new LanguageValidationList(), true);


				var itemVm = new LanguageViewModel(
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

		public async Task RefreshLanguagesAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				Languages.Clear();

				var languages = await _languageService.GetItemsAsync(true);

				Languages = new ObservableCollection<LanguageViewModel>(languages
					.Select(item => new LanguageViewModel(
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

		public RelayCommand RefreshLanguagesCommand => new RelayCommand(async () => await RefreshLanguagesAsync());

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