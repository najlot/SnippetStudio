using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public class MongoDbUserQuery : IUserQuery
	{
		private readonly IMongoCollection<UserModel> _collection;

		public MongoDbUserQuery(IMongoDatabase database)
		{
			_collection = database.GetCollection<UserModel>(nameof(UserModel)[0..^5]);
		}

		public User Get(Guid id)
		{
			var e = _collection.Find(item => item.Id == id).FirstOrDefault();

			if (e == null)
			{
				return null;
			}

			return new User()
			{
				Id = e.Id,
				Username = e.Username,
				EMail = e.EMail,
				Password = e.Password,
			};
		}

		public IEnumerable<User> GetAll()
		{
			return _collection.AsQueryable().Select(e => new User()
			{
				Id = e.Id,
				Username = e.Username,
				EMail = e.EMail,
				Password = e.Password,
			});
		}

		public IEnumerable<User> GetAll(Expression<Func<User, bool>> predicate)
		{
			var items = _collection.AsQueryable().Select(e => new User()
			{
				Id = e.Id,
				Username = e.Username,
				EMail = e.EMail,
				Password = e.Password,
			}).Where(predicate);

			return items;
		}
	}
}
