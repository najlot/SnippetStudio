using Microsoft.EntityFrameworkCore;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;
using System.Collections.Concurrent;

namespace SnippetStudio.Service.Repository
{
	public class MySqlDbContext : DbContext
	{
		private readonly MySqlConfiguration _configuration;

        private static readonly ConcurrentDictionary<string, ServerVersion> _knownVersions = new();

        public MySqlDbContext(MySqlConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = $"server={_configuration.Host};" +
				$"port={_configuration.Port};" +
				$"database={_configuration.Database};" +
				$"uid={_configuration.User};" +
				$"password={_configuration.Password}";

            if (!_knownVersions.TryGetValue(connectionString, out var version))
            {
                version = ServerVersion.AutoDetect(connectionString);
                _knownVersions[connectionString] = version;
            }

            optionsBuilder.UseMySql(connectionString, version);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<SnippetModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.OwnsMany(e => e.Variables, e => { e.HasKey(e => e.Id); e.ToTable("Snippet_Variables"); });
			});
			modelBuilder.Entity<UserModel>(entity =>
			{
				entity.HasKey(e => e.Id);
			});
		}

		public DbSet<SnippetModel> Snippets { get; set; }
		public DbSet<UserModel> Users { get; set; }
	}
}