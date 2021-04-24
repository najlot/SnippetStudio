using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface ISnippetService : IDisposable
	{
		SnippetModel CreateSnippet();
		Task<bool> AddItemAsync(SnippetModel item);
		Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false);
		Task<SnippetModel> GetItemAsync(Guid id);
		Task<bool> UpdateItemAsync(SnippetModel item);
		Task<bool> DeleteItemAsync(Guid id);
	}
}
