using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class LanguageService : IDisposable
	{
		private IDataStore<LanguageModel> _store;

		public LanguageService(IDataStore<LanguageModel> dataStore)
		{
			_store = dataStore;
		}

		public LanguageModel CreateLanguage()
		{
			return new LanguageModel()
			{
				Id = Guid.NewGuid(),
				Name = "",
			};
		}

		public Task<bool> AddItemAsync(LanguageModel item)
		{
			return _store.AddItemAsync(item);
		}

		public Task<bool> DeleteItemAsync(Guid id)
		{
			return _store.DeleteItemAsync(id);
		}

		public Task<LanguageModel> GetItemAsync(Guid id)
		{
			return _store.GetItemAsync(id);
		}

		public Task<IEnumerable<LanguageModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return _store.GetItemsAsync(forceRefresh);
		}

		public Task<bool> UpdateItemAsync(LanguageModel item)
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