using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.ViewModel
{
	public partial class SnippetViewModel
	{
		public ObservableCollection<VariableViewModel> Variables { get; set; } = new ObservableCollection<VariableViewModel>();

		public RelayCommand AddVariableCommand => new RelayCommand(() =>
		{
			var max = 0;

			if (Variables.Count > 0)
			{
				max = Variables.Max(e => e.Item.Id) + 1;
			}

			var model = new VariableModel() { Id = max };

			var vm = new VariableViewModel(_errorService, model, _navigationService, _messenger, Item.Id);

			Variables.Add(vm);
		});

		public void Handle(DeleteVariable obj)
		{
			if (Item.Id != obj.ParentId)
			{
				return;
			}

			try
			{
				var oldItem = Variables.FirstOrDefault(i => i.Item.Id == obj.Id);

				if (oldItem != null)
				{
					var index = Variables.IndexOf(oldItem);

					if (index != -1)
					{
						Variables.RemoveAt(index);
					}
				}
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}

		public void Handle(EditVariable obj)
		{
			if (IsBusy)
			{
				return;
			}

			if (Item.Id != obj.ParentId)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var vm = Variables.FirstOrDefault(e => e.Item.Id == obj.Id);

				// Prevalidate
				vm.Item.SetValidation(new VariableValidationList(), true);

				vm = new VariableViewModel(
					_errorService,
					vm.Item,
					_navigationService,
					_messenger,
					Item.Id);

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

		public void Handle(SaveVariable obj)
		{
			if (Item.Id != obj.ParentId)
			{
				return;
			}

			try
			{
				int index = -1;
				var oldItem = Variables.FirstOrDefault(i => i.Item.Id == obj.Item.Id);

				if (oldItem != null)
				{
					index = Variables.IndexOf(oldItem);

					if (index != -1)
					{
						Variables.RemoveAt(index);
					}
				}

				if (index == -1)
				{
					Variables.Insert(0, new VariableViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger,
						Item.Id));
				}
				else
				{
					Variables.Insert(index, new VariableViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger,
						Item.Id));
				}
			}
			catch (Exception ex)
			{
				_errorService.ShowAlert("Error saving...", ex);
			}
		}
	}
}
