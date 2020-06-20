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

		public User Get(Guid id)
		{
			using var db = new MySqlConnection(_connectionString);
			var item = db.QueryFirst<User>("SELECT * FROM Users WHERE Id=@id", new { id });
			return item;
		}

		public IEnumerable<User> GetAll()
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<User>("SELECT * FROM Users");
		}

		public IEnumerable<User> GetAll(Expression<Func<User, bool>> predicate)
		{
			using var db = new MySqlConnection(_connectionString);
			return db.Query<User>("SELECT * FROM Users").Where(predicate.Compile());
		}
	}
}
