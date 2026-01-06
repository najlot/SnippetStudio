using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;
using MySqlConnector;

namespace SnippetStudio.Service.Query
{
	public class MySqlUserQuery : IUserQuery
	{
		private readonly string _connectionString;

		public MySqlUserQuery(MySqlConfiguration _configuration)
		{
			_connectionString = $"server={_configuration.Host};" +
				$"port={_configuration.Port};" +
				$"database={_configuration.Database};" +
				$"uid={_configuration.User};" +
				$"password={_configuration.Password}";
		}

		public async Task<UserModel> GetAsync(Guid id)
		{
			using var db = new MySqlConnection(_connectionString);
			var item = await db.QueryFirstOrDefaultAsync<UserModel>("SELECT * FROM Users WHERE Id=@id", new { id });
			
			if (item == null)
			{
				return null;
			}


			return item;
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync()
		{
			using var db = new MySqlConnection(_connectionString);
			var items = await db.QueryAsync<UserModel>("SELECT * FROM Users");

			foreach (var item in items)
			{
				yield return item;
			}
		}

		public async IAsyncEnumerable<UserModel> GetAllAsync(Expression<Func<UserModel, bool>> predicate)
		{
			var check = predicate.Compile();
			
			await foreach (var item in GetAllAsync())
			{
				if (check(item))
				{
					yield return item;
				}
			}
		}
	}
}
