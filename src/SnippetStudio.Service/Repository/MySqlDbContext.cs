using Microsoft.EntityFrameworkCore;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MySqlDbContext : DbContext
	{
		private readonly MySqlConfiguration _configuration;

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

			optionsBuilder.UseMySql(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<SnippetModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.OwnsMany(e => e.Dependencies, e => { e.HasKey(e => e.Id); e.ToTable("Snippet_Dependencies"); });
				entity.OwnsMany(e => e.Variables, e => { e.HasKey(e => e.Id); e.ToTable("Snippet_Variables"); });
			});
		}

		public DbSet<SnippetModel> Snippets { get; set; }
	}
}