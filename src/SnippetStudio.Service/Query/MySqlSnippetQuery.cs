using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Configuration;

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

		public Snippet Get(Guid id)
		{
			using var db = new MySqlConnection(_connectionString);
			var item = db.QueryFirst<Snippet>("SELECT * FROM Snippets WHERE Id=@id", new { id });
			item.Dependencies = db.Query<Dependency>("SELECT * FROM Snippet_Dependencies WHERE SnippetModelId=@id", new { id }).ToList();
			item.Variables = db.Query<Variable>("SELECT * FROM Snippet_Variables WHERE SnippetModelId=@id", new { id }).ToList();
			return item;
		}

		public IEnumerable<Snippet> GetAll()
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<Snippet>("SELECT * FROM Snippets");
		}

		public IEnumerable<Snippet> GetAll(Expression<Func<Snippet, bool>> predicate)
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<Snippet>("SELECT * FROM Snippets").Where(predicate.Compile());
		}
	}
}
