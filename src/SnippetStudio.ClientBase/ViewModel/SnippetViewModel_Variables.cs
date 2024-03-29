﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Messages;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.ViewModel
{
	public partial class SnippetViewModel
	{
		private ObservableCollection<VariableViewModel> _variables = new ObservableCollection<VariableViewModel>();
		public ObservableCollection<VariableViewModel> Variables { get => _variables; set => Set(nameof(Variables), ref _variables, value); }

		public RelayCommand AddVariableCommand => new RelayCommand(() =>
		{
			var max = 0;

			if (Variables.Count > 0)
			{
				max = Variables.Max(e => e.Item.Id) + 1;
			}

			var model = new VariableModel() { Id = max };

			var viewModel = _variableViewModelFactory();
			viewModel.ParentId = Item.Id;
			viewModel.Item = model;

			Variables.Add(viewModel);
		});

		public async Task Handle(DeleteVariable obj)
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
				await _errorService.ShowAlertAsync("Error saving...", ex);
			}
		}

		public async Task Handle(EditVariable obj)
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

				var viewModel = _variableViewModelFactory();
				viewModel.ParentId = Item.Id;
				viewModel.Item = vm.Item;

				await _navigationService.NavigateForward(viewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error loading...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task Handle(SaveVariable obj)
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

				var viewModel = _variableViewModelFactory();
				viewModel.ParentId = Item.Id;
				viewModel.Item = obj.Item;

				if (index == -1)
				{
					Variables.Insert(0, viewModel);
				}
				else
				{
					Variables.Insert(index, viewModel);
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error saving...", ex);
			}
		}
	}
}
