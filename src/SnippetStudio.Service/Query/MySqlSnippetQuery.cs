using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public class MySqlSnippetQuery : ISnippetQuery
	{
		private readonly string _connectionString;

		public MySqlSnippetQuery(MySqlConfiguration _configuration)
		{
			_connectionString = $"server={_configuration.Host};" +
				$"port={_configuration.Port};" +
				$"database={_configuration.Database};" +
				$"uid={_configuration.User};" +
				$"password={_configuration.Password}";
		}

		public async Task<SnippetModel> GetAsync(Guid id)
		{
			using var db = new MySqlConnection(_connectionString);
			var item = await db.QueryFirstOrDefaultAsync<SnippetModel>("SELECT * FROM Snippets WHERE Id=@id", new { id });
			
			if (item == null)
			{
				return null;
			}

			item.Variables = (await db.QueryAsync<Variable>("SELECT * FROM Snippet_Variables WHERE SnippetModelId=@id", new { id })).ToList();

			return item;
		}

		public async IAsyncEnumerable<SnippetModel> GetAllAsync()
		{
			using var db = new MySqlConnection(_connectionString);
			var items = await db.QueryAsync<SnippetModel>("SELECT * FROM Snippets");

			foreach (var item in items)
			{
				yield return item;
			}
		}

		public async IAsyncEnumerable<SnippetModel> GetAllAsync(Expression<Func<SnippetModel, bool>> predicate)
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

		public IEnumerable<Snippet> GetAllForUser(string username)
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<Snippet>("SELECT * FROM Snippets WHERE CreatedBy = @user",
				new { user = username });
		}
	}
}
