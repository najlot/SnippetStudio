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

		public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
		{
			_action = action;
			
			if (canExecute != null)
				_canExecute = canExecute;
			else
				_canExecute = ReturnTrue;
		}

		private static bool ReturnTrue(T param) => true;

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
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