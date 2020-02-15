using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public class MongoDbLanguageQuery : ILanguageQuery
	{
		private readonly IMongoCollection<LanguageModel> _collection;

		public MongoDbLanguageQuery(IMongoDatabase database)
		{
			_collection = database.GetCollection<LanguageModel>(nameof(LanguageModel)[0..^5]);
		}

		public Language Get(Guid id)
		{
			var e = _collection.Find(item => item.Id == id).FirstOrDefault();

			if (e == null)
			{
				return null;
			}

			return new Language()
			{
				Id = e.Id,
				Name = e.Name,
			};
		}

		public IEnumerable<Language> GetAll()
		{
			return _collection.AsQueryable().Select(e => new Language()
			{
				Id = e.Id,
				Name = e.Name,
			});
		}

		public IEnumerable<Language> GetAll(Expression<Func<Language, bool>> predicate)
		{
			var items = _collection.AsQueryable().Select(e => new Language()
			{
				Id = e.Id,
				Name = e.Name,
			}).Where(predicate);

			return items;
		}
	}
}
