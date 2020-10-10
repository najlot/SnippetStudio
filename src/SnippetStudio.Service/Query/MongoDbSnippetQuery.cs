using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public class MongoDbSnippetQuery : ISnippetQuery
	{
		private readonly IMongoCollection<SnippetModel> _collection;

		public MongoDbSnippetQuery(IMongoDatabase database)
		{
			_collection = database.GetCollection<SnippetModel>(nameof(SnippetModel)[0..^5]);
		}

		public Snippet Get(Guid id)
		{
			var e = _collection.Find(item => item.Id == id).FirstOrDefault();

			if (e == null)
			{
				return null;
			}

			return new Snippet()
			{
				Id = e.Id,
				Name = e.Name,
				Language = e.Language,
				Variables = e.Variables,
				Template = e.Template,
				Code = e.Code,
			};
		}

		public IEnumerable<Snippet> GetAll()
		{
			return _collection.AsQueryable().Select(e => new Snippet()
			{
				Id = e.Id,
				Name = e.Name,
				Language = e.Language,
				Variables = e.Variables,
				Template = e.Template,
				Code = e.Code,
			});
		}

		public IEnumerable<Snippet> GetAll(Expression<Func<Snippet, bool>> predicate)
		{
			var items = _collection.AsQueryable().Select(e => new Snippet()
			{
				Id = e.Id,
				Name = e.Name,
				Language = e.Language,
				Variables = e.Variables,
				Template = e.Template,
				Code = e.Code,
			}).Where(predicate);

			return items;
		}

		public IEnumerable<Snippet> GetAllForUser(string username)
		{
			var items = _collection
				.AsQueryable()
				.Where(m => m.CreatedBy == username)
				.Select(e => new Snippet()
				{
					Id = e.Id,
					Name = e.Name,
					Language = e.Language,
					Variables = e.Variables,
					Template = e.Template,
					Code = e.Code,
				});

			return items;
		}
	}
}
