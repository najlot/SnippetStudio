using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface ISnippetStore : IDisposable
	{
		Task<bool> AddItemAsync(SnippetModel item);

		Task<bool> UpdateItemAsync(SnippetModel item);

		Task<bool> DeleteItemAsync(Guid id);

		Task<SnippetModel> GetItemAsync(Guid id);

		Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false);
	}
}
