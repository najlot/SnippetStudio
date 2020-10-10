using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public class FileSnippetQuery : ISnippetQuery
	{
		private readonly string _storagePath;

		public FileSnippetQuery(FileConfiguration configuration)
		{
			_storagePath = configuration.SnippetsPath;
			Directory.CreateDirectory(_storagePath);
		}

		public Snippet Get(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());

			if (!File.Exists(path))
			{
				return null;
			}

			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonConvert.DeserializeObject<Snippet>(text);

			return item;
		}

		public IEnumerable<Snippet> GetAll()
		{
			var items = Directory.GetFiles(_storagePath)
				.Select(path => File.ReadAllBytes(path))
				.Select(bytes => Encoding.UTF8.GetString(bytes))
				.Select(text => JsonConvert.DeserializeObject<Snippet>(text));

			return items;
		}

		public IEnumerable<Snippet> GetAll(Expression<Func<Snippet, bool>> predicate)
		{
			return GetAll().Where(predicate.Compile());
		}

		public IEnumerable<Snippet> GetAllForUser(string username)
		{
			var items = Directory.GetFiles(_storagePath)
				.Select(path => File.ReadAllBytes(path))
				.Select(bytes => Encoding.UTF8.GetString(bytes))
				.Select(text => JsonConvert.DeserializeObject<SnippetModel>(text))
				.Where(m => m.CreatedBy == username)
				.Select(e => new Snippet
				{
					Id = e.Id,
					Name = e.Name,
					Language = e.Language,
					Variables = e.Variables,
					Template = e.Template,
					Code = e.Code,
				});

			return items;
		}
	}
}
