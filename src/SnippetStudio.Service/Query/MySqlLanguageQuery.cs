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
	public class MySqlLanguageQuery : ILanguageQuery
	{
		private readonly string _connectionString;

		public MySqlLanguageQuery(MySqlConfiguration _configuration)
		{
			_connectionString = $"server={_configuration.Host};" +
				$"port={_configuration.Port};" +
				$"database={_configuration.Database};" +
				$"uid={_configuration.User};" +
				$"password={_configuration.Password}";
		}

		public Language Get(Guid id)
		{
			using var db = new MySqlConnection(_connectionString);
			var item = db.QueryFirst<Language>("SELECT * FROM Languages WHERE Id=@id", new { id });
			return item;
		}

		public IEnumerable<Language> GetAll()
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<Language>("SELECT * FROM Languages");
		}

		public IEnumerable<Language> GetAll(Expression<Func<Language, bool>> predicate)
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<Language>("SELECT * FROM Languages").Where(predicate.Compile());
		}
	}
}
