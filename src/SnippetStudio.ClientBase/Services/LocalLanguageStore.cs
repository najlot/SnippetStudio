using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class LocalLanguageStore : IDataStore<LanguageModel>
	{
		private readonly string _dataPath;
		private List<LanguageModel> _items = null;

		public LocalLanguageStore(string folderName)
		{
			var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnippetStudio");
			appdataDir = Path.Combine(appdataDir, folderName);
			Directory.CreateDirectory(appdataDir);

			_dataPath = Path.Combine(appdataDir, "Languages.json");
			_items = GetItems();
		}

		private List<LanguageModel> GetItems()
		{
			List<LanguageModel> items;
			if (File.Exists(_dataPath))
			{
				var data = File.ReadAllText(_dataPath);
				items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LanguageModel>>(data);
			}
			else
			{
				items = new List<LanguageModel>();
			}

			return items;
		}

		public async Task<bool> AddItemAsync(LanguageModel item)
		{
			_items.Insert(0, item);

			SaveItems();

			return await Task.FromResult(true);
		}

		private void SaveItems()
		{
			var text = Newtonsoft.Json.JsonConvert.SerializeObject(_items);
			File.WriteAllText(_dataPath, text);
		}

		public async Task<bool> UpdateItemAsync(LanguageModel item)
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

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			var oldItem = _items.Where(arg => arg.Id == id).FirstOrDefault();
			_items.Remove(oldItem);

			SaveItems();

			return await Task.FromResult(true);
		}

		public async Task<LanguageModel> GetItemAsync(Guid id)
		{
			return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
		}

		public async Task<IEnumerable<LanguageModel>> GetItemsAsync(bool forceRefresh = false)
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