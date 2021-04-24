using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface IUserService : IDisposable
	{
		UserModel CreateUser();
		Task<bool> AddItemAsync(UserModel item);
		Task<IEnumerable<UserModel>> GetItemsAsync(bool forceRefresh = false);
		Task<UserModel> GetItemAsync(Guid id);
		Task<bool> UpdateItemAsync(UserModel item);
		Task<bool> DeleteItemAsync(Guid id);
	}
}
