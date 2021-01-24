using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

		public async Task<UserModel> GetAsync(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());

			if (!File.Exists(path))
			{
				return null;
			}

			var bytes = await File.ReadAllBytesAsync(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<UserModel>(text, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

			return item;
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync()
		{
			foreach (var path in Directory.GetFiles(_storagePath))
			{
				var bytes = await File.ReadAllBytesAsync(path);
				var text = Encoding.UTF8.GetString(bytes);
				var item = JsonSerializer.Deserialize<UserModel>(text, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
				yield return item;
			}
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync(Expression<Func<UserModel, bool>> predicate)
		{
			var check = predicate.Compile();

			await foreach (var item in GetAllAsync())
			{
				if (check(item))
				{
					yield return item;
				}
			}
		}
	}
}
