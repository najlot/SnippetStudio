using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.Services
{
	public interface IDataStore<TModel> : IDisposable
	{
		Task<bool> AddItemAsync(TModel item);

		Task<bool> UpdateItemAsync(TModel item);

		Task<bool> DeleteItemAsync(Guid id);

		Task<TModel> GetItemAsync(Guid id);

		Task<IEnumerable<TModel>> GetItemsAsync(bool forceRefresh = false);
	}
}