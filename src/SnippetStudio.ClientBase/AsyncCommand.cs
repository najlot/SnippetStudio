using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnippetStudio.ClientBase
{
	public class AsyncCommand : AsyncCommand<object>
	{
		public AsyncCommand(Func<Task> action, Func<Task, Task> errorCallback)
			: base(async _ => await action(), errorCallback)
		{
		}

		public AsyncCommand(Func<Task> action, Func<Task, Task> errorCallback, Func<bool> canExecute)
			: base(async _ => await action(), errorCallback, _ => canExecute())
		{
		}
	}

	public class AsyncCommand<T> : ICommand
	{
		private readonly Func<T, Task> _action;
		private readonly Func<T, bool> _canExecute;
		private readonly Func<Task, Task> _errorCallback;
		
		public event EventHandler CanExecuteChanged;

		public AsyncCommand(Func<T, Task> action, Func<Task, Task> errorCallback, Func<T, bool> canExecute = null)
		{
			_action = action;
			_errorCallback = errorCallback;

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
				ExecuteAsync(parameter).ContinueWith(_errorCallback, TaskContinuationOptions.OnlyOnFaulted);
			}
		}

		public async Task ExecuteAsync(object parameter)
		{
			if (CanExecute(parameter))
			{
				await _action(parameter == null ? default : (T)parameter);
			}
		}
	}
}