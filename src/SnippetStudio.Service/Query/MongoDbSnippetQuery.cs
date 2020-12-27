using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

		public async Task<SnippetModel> GetAsync(Guid id)
		{
			var result = await _collection.FindAsync(item => item.Id == id);
			var item = result.FirstOrDefault();

			if (item == null)
			{
				return null;
			}

			return item;
		}

		public async IAsyncEnumerable<SnippetModel> GetAllAsync()
		{
			var items = await _collection.FindAsync(FilterDefinition<SnippetModel>.Empty);
			
			while (await items.MoveNextAsync())
			{
				foreach (var item in items.Current)
				{
					yield return item;
				}
			}
		}

		public async IAsyncEnumerable<SnippetModel> GetAllAsync(Expression<Func<SnippetModel, bool>> predicate)
		{
			var items = await _collection.FindAsync(predicate);

			while (await items.MoveNextAsync())
			{
				foreach (var item in items.Current)
				{
					yield return item;
				}
			}
		}
	}
}
