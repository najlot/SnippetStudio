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
		private ObservableCollection<DependencyViewModel> _dependencies = new ObservableCollection<DependencyViewModel>();
		public ObservableCollection<DependencyViewModel> Dependencies { get => _dependencies; set => Set(nameof(Dependencies), ref _dependencies, value); }

		public RelayCommand AddDependencyCommand => new RelayCommand(() =>
		{
			var max = 0;

			if (Dependencies.Count > 0)
			{
				max = Dependencies.Max(e => e.Item.Id) + 1;
			}

			var model = new DependencyModel() { Id = max };

			var vm = new DependencyViewModel(_errorService, model, _navigationService, _messenger, Item.Id);

			Dependencies.Add(vm);
		});

		public async Task Handle(DeleteDependency obj)
		{
			if (Item.Id != obj.ParentId)
			{
				return;
			}

			try
			{
				var oldItem = Dependencies.FirstOrDefault(i => i.Item.Id == obj.Id);

				if (oldItem != null)
				{
					var index = Dependencies.IndexOf(oldItem);

					if (index != -1)
					{
						Dependencies.RemoveAt(index);
					}
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error saving...", ex);
			}
		}

		public async Task Handle(EditDependency obj)
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

				var vm = Dependencies.FirstOrDefault(e => e.Item.Id == obj.Id);

				// Prevalidate
				vm.Item.SetValidation(new DependencyValidationList(), true);

				vm = new DependencyViewModel(
					_errorService,
					vm.Item,
					_navigationService,
					_messenger,
					Item.Id);

				await _navigationService.NavigateForward(vm);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error loading...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task Handle(SaveDependency obj)
		{
			if (Item.Id != obj.ParentId)
			{
				return;
			}

			try
			{
				int index = -1;
				var oldItem = Dependencies.FirstOrDefault(i => i.Item.Id == obj.Item.Id);

				if (oldItem != null)
				{
					index = Dependencies.IndexOf(oldItem);

					if (index != -1)
					{
						Dependencies.RemoveAt(index);
					}
				}

				if (index == -1)
				{
					Dependencies.Insert(0, new DependencyViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger,
						Item.Id));
				}
				else
				{
					Dependencies.Insert(index, new DependencyViewModel(
						_errorService,
						obj.Item,
						_navigationService,
						_messenger,
						Item.Id));
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlert("Error saving...", ex);
			}
		}
	}
}