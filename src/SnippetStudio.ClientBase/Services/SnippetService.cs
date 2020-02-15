using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class SnippetService : IDisposable
	{
		private IDataStore<SnippetModel> _store;

		public SnippetService(IDataStore<SnippetModel> dataStore)
		{
			_store = dataStore;
		}

		public SnippetModel CreateSnippet()
		{
			return new SnippetModel()
			{
				Id = Guid.NewGuid(),
				Name = "",
				Template = "",
				Code = "",
			};
		}

		public Task<bool> AddItemAsync(SnippetModel item)
		{
			return _store.AddItemAsync(item);
		}

		public Task<bool> DeleteItemAsync(Guid id)
		{
			return _store.DeleteItemAsync(id);
		}

		public Task<SnippetModel> GetItemAsync(Guid id)
		{
			return _store.GetItemAsync(id);
		}

		public Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return _store.GetItemsAsync(forceRefresh);
		}

		public Task<bool> UpdateItemAsync(SnippetModel item)
		{
			return _store.UpdateItemAsync(item);
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