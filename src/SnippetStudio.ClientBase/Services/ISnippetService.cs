using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface ISnippetService : IDisposable
	{
		SnippetModel CreateSnippet(string language);
		Task<bool> AddItemAsync(SnippetModel item);
		Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false);
		Task<SnippetModel> GetItemAsync(Guid id);
		Task<bool> UpdateItemAsync(SnippetModel item);
		Task<bool> DeleteItemAsync(Guid id);

		Task<string> Run(string language, string code, string template, Dictionary<string, string> variables);
		string GetMyName();
	}
}
