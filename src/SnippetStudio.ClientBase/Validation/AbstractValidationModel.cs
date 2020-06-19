using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SnippetStudio.ClientBase.Validation
{
	public abstract class AbstractValidationModel : INotifyDataErrorInfo, INotifyPropertyChanged
	{
		private Action _validateAction = () => { };
		private IEnumerable<ValidationResult> _errors = new List<ValidationResult>();
		private IEnumerable<string> _previousProperties = new List<string>();

		protected void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		
		public void SetValidation<T>(ValidationList<T> validations, bool validate = true) where T : AbstractValidationModel
		{
			if (!typeof(T).IsAssignableFrom(GetType()))
			{
				throw new InvalidOperationException($"{typeof(T).FullName} is not assignable from {GetType().FullName}");
			}

			_validateAction = async () =>
			{
				Errors = await validations.ValidateAsync((T)this);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
			};

			if (validate)
			{
				_validateAction();
			}
		}

		public void Set<T>(ref T oldValue, T newValue)
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
			_validateAction();
		}

		private bool _hasErrors = false;
		public bool HasErrors { get => Errors.Any() || _hasErrors; set => _hasErrors = value; }

		public IEnumerable GetErrors(string propertyName)
		{
			foreach (var err in Errors)
			{
				if (string.IsNullOrEmpty(err.PropertyName) || err.PropertyName == propertyName)
				{
					yield return err;
				}
			}
		}

		public IEnumerable<ValidationResult> Errors
		{
			get => _errors;
			set
			{
				_errors = value;

				foreach (var propertyName in _previousProperties)
				{
					RaiseErrorsChanged(propertyName);
				}

				var propertyNames = _errors.Select(err => err.PropertyName).Distinct();

				foreach (var propertyName in propertyNames)
				{
					RaiseErrorsChanged(propertyName);
				}

				_previousProperties = propertyNames;
			}
		}
	}
}
