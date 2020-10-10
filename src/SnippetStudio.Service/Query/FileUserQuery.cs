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
	public class FileUserQuery : IUserQuery
	{
		private readonly string _storagePath;

		public FileUserQuery(FileConfiguration configuration)
		{
			_storagePath = configuration.UsersPath;
			Directory.CreateDirectory(_storagePath);
		}

		public User Get(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());

			if (!File.Exists(path))
			{
				return null;
			}

			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonConvert.DeserializeObject<User>(text);

			return item;
		}

		public IEnumerable<User> GetAll()
		{
			var items = Directory.GetFiles(_storagePath)
				.Select(path => File.ReadAllBytes(path))
				.Select(bytes => Encoding.UTF8.GetString(bytes))
				.Select(text => JsonConvert.DeserializeObject<UserModel>(text))
				.Where(u => u.IsActive)
				.Select(m => new User
				{
					Id = m.Id,
					Username = m.Username,
					EMail = m.EMail
				});

			return items;
		}

		public IEnumerable<User> GetAll(Expression<Func<User, bool>> predicate)
		{
			return GetAll().Where(predicate.Compile());
		}
	}
}
