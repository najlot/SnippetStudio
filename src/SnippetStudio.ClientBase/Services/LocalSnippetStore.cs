using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public sealed class LocalSnippetStore : IDataStore<SnippetModel>
	{
		private readonly string _dataPath;
		private readonly LocalSubscriber _subscriber;
		private List<SnippetModel> _items = null;

		public LocalSnippetStore(string folderName, LocalSubscriber localSubscriber)
		{
			var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnippetStudio");
			appdataDir = Path.Combine(appdataDir, folderName);
			Directory.CreateDirectory(appdataDir);

			_dataPath = Path.Combine(appdataDir, "Snippets.json");
			_items = GetItems();
			_subscriber = localSubscriber;
		}

		private List<SnippetModel> GetItems()
		{
			List<SnippetModel> items;
			if (File.Exists(_dataPath))
			{
				var data = File.ReadAllText(_dataPath);
				items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SnippetModel>>(data);
			}
			else
			{
				items = new List<SnippetModel>();
			}

			return items;
		}

		public async Task<bool> AddItemAsync(SnippetModel item)
		{
			_items.Insert(0, item);

			SaveItems();

			await _subscriber.SendAsync(new SnippetCreated(
				item.Id,
				item.Name,
				item.Language,
				item.Dependencies,
				item.Variables,
				item.Template,
				item.Code));

			return await Task.FromResult(true);
		}

		private void SaveItems()
		{
			var text = Newtonsoft.Json.JsonConvert.SerializeObject(_items);
			File.WriteAllText(_dataPath, text);
		}

		public async Task<bool> UpdateItemAsync(SnippetModel item)
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

			await _subscriber.SendAsync(new SnippetUpdated(
				item.Id,
				item.Name,
				item.Language,
				item.Dependencies,
				item.Variables,
				item.Template,
				item.Code));

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
			_items.Remove(oldItem);

			SaveItems();

			await _subscriber.SendAsync(new SnippetDeleted(id));

			return await Task.FromResult(true);
		}

		public async Task<SnippetModel> GetItemAsync(Guid id)
		{
			return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
		}

		public async Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false)
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