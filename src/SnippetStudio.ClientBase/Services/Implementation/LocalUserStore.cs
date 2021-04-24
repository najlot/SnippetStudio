using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services.Implementation
{
	public sealed class LocalUserStore : IUserStore
	{
		private readonly string _dataPath;
		private readonly ILocalSubscriber _subscriber;
		private List<UserModel> _items = null;

		public LocalUserStore(string folderName, ILocalSubscriber localSubscriber)
		{
			var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnippetStudio");
			appdataDir = Path.Combine(appdataDir, folderName);
			Directory.CreateDirectory(appdataDir);

			_dataPath = Path.Combine(appdataDir, "Users.json");
			_items = GetItems();
			_subscriber = localSubscriber;
		}

		private List<UserModel> GetItems()
		{
			List<UserModel> items;
			if (File.Exists(_dataPath))
			{
				var data = File.ReadAllText(_dataPath);
				items = JsonSerializer.Deserialize<List<UserModel>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			}
			else
			{
				items = new List<UserModel>();
			}

			return items;
		}

		public async Task<bool> AddItemAsync(UserModel item)
		{
			_items.Insert(0, item);

			SaveItems();

			await _subscriber.SendAsync(new UserCreated(
				item.Id,
				item.Username,
				item.EMail,
				item.Password));

			return await Task.FromResult(true);
		}

		private void SaveItems()
		{
			var text = JsonSerializer.Serialize(_items);
			File.WriteAllText(_dataPath, text);
		}

		public async Task<bool> UpdateItemAsync(UserModel item)
		{
			int index = 0;
			var oldItem = _items.FirstOrDefault(i => i.Id == item.Id);

			if (oldItem != null)
			{
				index = _items.IndexOf(oldItem);

				if (index != -1)
				{
					_items.RemoveAt(index);
				}
				else
				{
					index = 0;
				}
			}

			_items.Insert(index, item);

			SaveItems();

			await _subscriber.SendAsync(new UserUpdated(
				item.Id,
				item.Username,
				item.EMail,
				item.Password));

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
			_items.Remove(oldItem);

			SaveItems();

			await _subscriber.SendAsync(new UserDeleted(id));

			return await Task.FromResult(true);
		}

		public async Task<UserModel> GetItemAsync(Guid id)
		{
			return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
		}

		public async Task<IEnumerable<UserModel>> GetItemsAsync(bool forceRefresh = false)
		{
			_items = GetItems();

			return await Task.FromResult(_items);
		}

		public void Dispose()
		{
			// Nothing to do
		}
	}
}