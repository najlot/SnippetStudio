using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MongoDbSnippetRepository : ISnippetRepository
	{
		private readonly IMongoCollection<SnippetModel> _collection;

		public MongoDbSnippetRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<SnippetModel>(nameof(SnippetModel)[0..^5]);
		}

		public void Delete(Guid id)
		{
			_collection.DeleteOne(item => item.Id == id);
		}

		public SnippetModel Get(Guid id)
		{
			return _collection.Find(item => item.Id == id).FirstOrDefault();
		}

		public IEnumerable<SnippetModel> GetAll()
		{
			return _collection.AsQueryable();
		}

		public IEnumerable<SnippetModel> GetAll(Expression<Func<SnippetModel, bool>> predicate)
		{
			return _collection.Find(predicate).ToEnumerable();
		}

		public void Insert(SnippetModel model)
		{
			_collection.InsertOne(model);
		}

		public void Update(SnippetModel model)
		{
			_collection.FindOneAndReplace(item => item.Id == model.Id, model);
		}
	}
}