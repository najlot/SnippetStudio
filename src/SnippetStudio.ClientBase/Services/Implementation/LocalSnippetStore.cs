using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services.Implementation
{
	public sealed class LocalSnippetStore : ISnippetStore
	{
		private readonly string _dataPath;
		private readonly ILocalSubscriber _subscriber;
		private List<SnippetModel> _items = null;

		public LocalSnippetStore(string folderName, ILocalSubscriber localSubscriber)
		{
			if (Path.IsPathRooted(folderName))
			{
				_dataPath = folderName;
			}
			else
			{
				var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnippetStudio");
				appdataDir = Path.Combine(appdataDir, folderName);
				Directory.CreateDirectory(appdataDir);

				_dataPath = appdataDir;
			}
			
			_items = GetItems();
			_subscriber = localSubscriber;
		}

		private List<SnippetModel> GetItems()
		{
			List<SnippetModel> items;
			var path = Path.Combine(_dataPath, "Snippets.json");

			if (File.Exists(path))
			{
				var data = File.ReadAllText(path);
				items = JsonSerializer.Deserialize<List<SnippetModel>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			}
			else if (Directory.Exists(_dataPath))
			{
				items = ObsoleteReader.TemplateReader.
					ReadAllTemplates(_dataPath).ToList();
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
				item.Variables,
				item.Template,
				item.Code,
				"Me"));

			return await Task.FromResult(true);
		}

		private void SaveItems()
		{
			var path = Path.Combine(_dataPath, "Snippets.json");
			var text = JsonSerializer.Serialize(_items);
			File.WriteAllText(path, text);
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