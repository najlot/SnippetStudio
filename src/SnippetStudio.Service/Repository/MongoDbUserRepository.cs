using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MongoDbUserRepository : IUserRepository
	{
		private readonly IMongoCollection<UserModel> _collection;

		public MongoDbUserRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<UserModel>(nameof(UserModel)[0..^5]);
		}

		public void Delete(Guid id)
		{
			_collection.DeleteOne(item => item.Id == id);
		}

		public UserModel Get(Guid id)
		{
			return _collection.Find(item => item.Id == id).FirstOrDefault();
		}

		public UserModel Get(string username)
		{
			return _collection.Find(item => item.IsActive && item.Username == username).FirstOrDefault();
		}

		public IEnumerable<UserModel> GetAll()
		{
			return _collection.AsQueryable();
		}

		public IEnumerable<UserModel> GetAll(Expression<Func<UserModel, bool>> predicate)
		{
			return _collection.Find(predicate).ToEnumerable();
		}

		public void Insert(UserModel model)
		{
			_collection.InsertOne(model);
		}

		public void Update(UserModel model)
		{
			_collection.FindOneAndReplace(item => item.Id == model.Id, model);
		}
	}
}