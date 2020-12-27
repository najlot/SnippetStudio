using Cosei.Service.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;
using System.Text;
using System.Security.Cryptography;

namespace SnippetStudio.Service.Services
{
	public class UserService : IDisposable
	{
		private readonly IUserRepository _userRepository;
		private readonly IUserQuery _userQuery;
		private readonly IPublisher _publisher;

		public UserService(IUserRepository userRepository,
			IUserQuery userQuery,
			IPublisher publisher)
		{
			_userRepository = userRepository;
			_userQuery = userQuery;
			_publisher = publisher;
		}

		public void CreateUser(CreateUser command, string userName)
		{
			if (_userQuery.GetAll(u => u.Username == command.Username).Any())
			{
				throw new InvalidOperationException("User already exists!");
			}

			var item = new UserModel()
			{
				Id = command.Id,
				Username = command.Username,
				EMail = command.EMail,
				PasswordHash = HashPassword(command.Password),
				IsActive = true
			};

			_userRepository.Insert(item);

			_publisher.PublishAsync(new UserCreated(
				command.Id,
				command.Username,
				command.EMail,
				""));
		}

		public void UpdateUser(UpdateUser command, string userName)
		{
			var item = _userRepository.Get(command.Id);

			if (item.Username != userName && userName != "admin")
			{
				throw new InvalidOperationException("You must not modify other users!");
			}

			item.EMail = command.EMail;

			if (!string.IsNullOrWhiteSpace(command.Password))
			{
				item.PasswordHash = HashPassword(command.Password);
			}

			_userRepository.Update(item);

			_publisher.PublishAsync(new UserUpdated(
				command.Id,
				command.Username,
				command.EMail,
				""));
		}

		public void DeleteUser(Guid id, string userName)
		{
			var item = _userRepository.Get(id);

			if (item.Username != userName && userName != "admin")
			{
				throw new InvalidOperationException("You must not delete other user!");
			}

			item.IsActive = false;

			_userRepository.Update(item);

			_publisher.PublishAsync(new UserDeleted(id));
		}

		public async Task<User> GetItemAsync(Guid id)
		{
			var item = await _userQuery.GetAsync(id);

			if (item == null)
			{
				return null;
			}

			return new User
			{
				Id = item.Id,
				Username = item.Username,
				EMail = item.EMail,
				Password = item.Password,
			};
		}

		public async IAsyncEnumerable<User> GetItemsForUserAsync(string userName)
		{
			await foreach (var item in _userQuery.GetAllAsync())
			{
				yield return new User
				{
					Id = item.Id,
					Username = item.Username,
					EMail = item.EMail,
					Password = item.Password,
				};
			}
		}

		public UserModel GetUserModelFromName(string userName)
		{
			var user = _userRepository.Get(userName);

			if (user == null && userName == "admin")
			{
				var id = Guid.NewGuid();

				var command = new CreateUser(
					id,
					userName,
					"",
					userName);

				CreateUser(command, userName);

				user = _userRepository.Get(id);
			}

			return user;
		}

		private byte[] HashPassword(string password)
		{
			var passwordBytes = Encoding.UTF8.GetBytes(password);
			using var sha = SHA256.Create();
			return sha.ComputeHash(passwordBytes);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					(_userRepository as IDisposable)?.Dispose();
					(_userQuery as IDisposable)?.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}