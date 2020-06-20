﻿using Cosei.Client.RabbitMq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class UserService : IDisposable
	{
		private IDataStore<UserModel> _store;
		private readonly Messenger _messenger;
		private readonly IDispatcherHelper _dispatcher;

		public UserService(
			IDataStore<UserModel> dataStore,
			Messenger messenger,
			IDispatcherHelper dispatcher,
			ISubscriber subscriber)
		{
			_store = dataStore;
			_messenger = messenger;
			_dispatcher = dispatcher;
			
			subscriber.Register<UserCreated>(Handle);
			subscriber.Register<UserUpdated>(Handle);
			subscriber.Register<UserDeleted>(Handle);
		}

		private async Task Handle(UserCreated message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		private async Task Handle(UserUpdated message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		private async Task Handle(UserDeleted message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		public UserModel CreateUser()
		{
			return new UserModel()
			{
				Id = Guid.NewGuid(),
				Username = "",
				EMail = "",
				Password = "",
			};
		}

		public async Task<bool> AddItemAsync(UserModel item)
		{
			return await _store.AddItemAsync(item);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			return await _store.DeleteItemAsync(id);
		}

		public async Task<UserModel> GetItemAsync(Guid id)
		{
			return await _store.GetItemAsync(id);
		}

		public async Task<IEnumerable<UserModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return await _store.GetItemsAsync(forceRefresh);
		}

		public async Task<bool> UpdateItemAsync(UserModel item)
		{
			return await _store.UpdateItemAsync(item);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_store?.Dispose();
					_store = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}