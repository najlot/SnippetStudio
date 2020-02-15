using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MongoDbLanguageRepository : ILanguageRepository
	{
		private readonly IMongoCollection<LanguageModel> _collection;

		public MongoDbLanguageRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<LanguageModel>(nameof(LanguageModel)[0..^5]);
		}

		public void Delete(Guid id)
		{
			_collection.DeleteOne(item => item.Id == id);
		}

		public LanguageModel Get(Guid id)
		{
			return _collection.Find(item => item.Id == id).FirstOrDefault();
		}

		public IEnumerable<LanguageModel> GetAll()
		{
			return _collection.AsQueryable();
		}

		public IEnumerable<LanguageModel> GetAll(Expression<Func<LanguageModel, bool>> predicate)
		{
			return _collection.Find(predicate).ToEnumerable();
		}

		public void Insert(LanguageModel model)
		{
			_collection.InsertOne(model);
		}

		public void Update(LanguageModel model)
		{
			_collection.FindOneAndReplace(item => item.Id == model.Id, model);
		}
	}
}