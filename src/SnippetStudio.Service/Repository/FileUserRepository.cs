using System;
using System.IO;
using System.Text;
using System.Text.Json;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class FileUserRepository : IUserRepository
	{
		private readonly string _storagePath;

		public FileUserRepository(FileConfiguration configuration)
		{
			_storagePath = configuration.UsersPath;
			Directory.CreateDirectory(_storagePath);
		}

		public void Delete(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());
			File.Delete(path);
		}

		public UserModel Get(Guid id)
		{
			var path = Path.Combine(_storagePath, id.ToString());

			if (!File.Exists(path))
			{
				return null;
			}

			var bytes = File.ReadAllBytes(path);
			var item = JsonSerializer.Deserialize<UserModel>(bytes, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

			return item;
		}

		public void Insert(UserModel model)
		{
			Update(model);
		}

		public void Update(UserModel model)
		{
			var path = Path.Combine(_storagePath, model.Id.ToString());
			var bytes = JsonSerializer.SerializeToUtf8Bytes(model);
			File.WriteAllBytes(path, bytes);
		}
	}
}