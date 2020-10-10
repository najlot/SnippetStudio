using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;
using System.Linq;

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
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonConvert.DeserializeObject<UserModel>(text);

			return item;
		}

		public UserModel Get(string username)
		{
			return Directory.GetFiles(_storagePath)
				.Select(path => File.ReadAllBytes(path))
				.Select(bytes => Encoding.UTF8.GetString(bytes))
				.Select(text => JsonConvert.DeserializeObject<UserModel>(text))
				.FirstOrDefault(u => u.IsActive && u.Username == username);
		}

		public void Insert(UserModel model)
		{
			Update(model);
		}

		public void Update(UserModel model)
		{
			var path = Path.Combine(_storagePath, model.Id.ToString());
			var str = JsonConvert.SerializeObject(model);
			var bytes = Encoding.UTF8.GetBytes(str);
			File.WriteAllBytes(path, bytes);
		}
	}
}