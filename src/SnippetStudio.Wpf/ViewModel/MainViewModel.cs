using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.Services;

namespace SnippetStudio.Wpf.ViewModel
{
	public class MainViewModel : AbstractViewModel, INavigationService
	{
		private AbstractViewModel _viewModel;
		private readonly Stack<AbstractViewModel> _backViewModels = new Stack<AbstractViewModel>();
		private AbstractViewModel _lastViewModel = null;

		private bool _isPopup = false;

		public RelayCommand NavigateBackCommand { get; }

		public AbstractViewModel ViewModel
		{
			get => _viewModel;
			private set
			{
				_isPopup = value is IPopupViewModel;
				Set(nameof(ViewModel), ref _viewModel, value);
			}
		}

		public MainViewModel()
		{
			NavigateBackCommand = new RelayCommand(() => NavigateBack(), () => _backViewModels.Count > 0 && !_isPopup);
		}

		public Task NavigateForward(AbstractViewModel newViewModel)
		{
			if (_lastViewModel != null)
			{
				_backViewModels.Push(_lastViewModel);
			}

			_lastViewModel = newViewModel;

			ViewModel = newViewModel;

			NavigateBackCommand.RaiseCanExecuteChanged();

			return Task.CompletedTask;
		}

		public Task NavigateBack()
		{
			if (_backViewModels.Count < 1)
			{
				return Task.CompletedTask;
			}

			_lastViewModel = _backViewModels.Pop();

			if (_lastViewModel != null)
			{
				ViewModel = _lastViewModel;
			}

			NavigateBackCommand.RaiseCanExecuteChanged();

			return Task.CompletedTask;
		}
	}
}