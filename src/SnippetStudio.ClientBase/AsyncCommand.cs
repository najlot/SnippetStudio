using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnippetStudio.ClientBase
{
	public class AsyncCommand : AsyncCommand<object>
	{
		public AsyncCommand(Func<Task> action)
			: base(async _ => await action())
		{
		}

		public AsyncCommand(Func<Task> action, Func<bool> canExecute)
			: base(async _ => await action(), _ => canExecute())
		{
		}
	}

	public class AsyncCommand<T> : ICommand
	{
		private readonly Func<T, Task> _action;
		private readonly Func<T, bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public AsyncCommand(Func<T, Task> action)
		{
			_action = action;
			_canExecute = _ => true;
		}

		public AsyncCommand(Func<T, Task> action, Func<T, bool> canExecute)
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
			ExecuteAsync(parameter).ContinueWith(task => Console.WriteLine(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
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