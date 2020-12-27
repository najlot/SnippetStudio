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
	public class MongoDbUserQuery : IUserQuery
	{
		private readonly IMongoCollection<UserModel> _collection;

		public MongoDbUserQuery(IMongoDatabase database)
		{
			_collection = database.GetCollection<UserModel>(nameof(UserModel)[0..^5]);
		}

		public async Task<UserModel> GetAsync(Guid id)
		{
			var result = await _collection.FindAsync(item => item.Id == id);
			var item = result.FirstOrDefault();

			if (item == null)
			{
				return null;
			}

			return item;
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync()
		{
			var items = await _collection.FindAsync(FilterDefinition<UserModel>.Empty);
			
			while (await items.MoveNextAsync())
			{
				foreach (var item in items.Current)
				{
					yield return item;
				}
			}
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync(Expression<Func<UserModel, bool>> predicate)
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
