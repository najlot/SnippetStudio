using System.Collections.Generic;
using System.ComponentModel;

namespace SnippetStudio.ClientBase
{
	public abstract class AbstractViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Set<T>(string propertyName, ref T oldValue, T newValue)
		{
			if (EqualityComparer<T>.Default.Equals(oldValue, default) &&
				EqualityComparer<T>.Default.Equals(newValue, default))
			{
				return;
			}

			if (oldValue?.Equals(newValue) ?? false)
			{
				return;
			}

			oldValue = newValue;
			RaisePropertyChanged(propertyName);
		}
	}
}