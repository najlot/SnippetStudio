using Cosei.Service.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;

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
			var item = new UserModel()
			{
				Id = command.Id,
				Username = command.Username,
				EMail = command.EMail,
				Password = command.Password,
			};

			_userRepository.Insert(item);

			_publisher.PublishAsync(new UserCreated(
				command.Id,
				command.Username,
				command.EMail,
				command.Password));
		}

		public void UpdateUser(UpdateUser command, string userName)
		{
			var item = _userRepository.Get(command.Id);
			
			item.Username = command.Username;
			item.EMail = command.EMail;
			item.Password = command.Password;

			_userRepository.Update(item);

			_publisher.PublishAsync(new UserUpdated(
				command.Id,
				command.Username,
				command.EMail,
				command.Password));
		}

		public void DeleteUser(Guid id, string userName)
		{
			_userRepository.Delete(id);

			_publisher.PublishAsync(new UserDeleted(id));
		}

		public User GetItem(Guid id)
		{
			return _userQuery.Get(id);
		}

		public List<User> GetItemsForUser(string userName)
		{
			IEnumerable<User> items = _userQuery.GetAll();

			return items.ToList();
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