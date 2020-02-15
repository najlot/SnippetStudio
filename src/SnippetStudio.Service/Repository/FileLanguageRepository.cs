using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class FileLanguageRepository : ILanguageRepository
	{
		private readonly string _storagePath;

		public FileLanguageRepository(FileConfiguration configuration)
		{
			_storagePath = configuration.LanguagesPath;
			Directory.CreateDirectory(_storagePath);
		}

		public void Delete(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());
			File.Delete(path);
		}

		public LanguageModel Get(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());

			if (!File.Exists(path))
			{
				return null;
			}

			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonConvert.DeserializeObject<LanguageModel>(text);

			return item;
		}

		public void Insert(LanguageModel model)
		{
			Update(model);
		}

		public void Update(LanguageModel model)
		{
			var path = Path.Combine(_storagePath, model.Id.ToString());
			var str = JsonConvert.SerializeObject(model);
			var bytes = Encoding.UTF8.GetBytes(str);
			File.WriteAllBytes(path, bytes);
		}
	}
}