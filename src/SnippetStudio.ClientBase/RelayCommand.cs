using System;
using System.Windows.Input;

namespace SnippetStudio.ClientBase
{
	public class RelayCommand : RelayCommand<object>
	{
		public RelayCommand(Action action)
			: base(_ => action())
		{
		}

		public RelayCommand(Action action, Func<bool> canExecute)
			: base(_ => action(), _ => canExecute())
		{
		}
	}

	public class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _action;
		private readonly Func<T, bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public RelayCommand(Action<T> action)
		{
			_action = action;
			_canExecute = _ => true;
		}

		public RelayCommand(Action<T> action, Func<T, bool> canExecute)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			if (CanExecute(parameter))
			{
				_action(parameter == null ? default : (T)parameter);
			}
		}
	}
}