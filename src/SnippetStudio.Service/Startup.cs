using Cosei.Service.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;
using SnippetStudio.Service.Services;

namespace SnippetStudio.Service
{
	public class Startup
	{
		private bool _useCoseiRabbitMq = false;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var rmqConfig = ConfigurationReader.ReadConfiguration<RabbitMqConfiguration>();
			var fileConfig = ConfigurationReader.ReadConfiguration<FileConfiguration>();
			var mysqlConfig = ConfigurationReader.ReadConfiguration<MySqlConfiguration>();
			var mongoDbConfig = ConfigurationReader.ReadConfiguration<MongoDbConfiguration>();
			var serviceConfig = ConfigurationReader.ReadConfiguration<ServiceConfiguration>();

			if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
			{
				throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
			}

			services.AddSingleton(serviceConfig);

			if (mongoDbConfig != null)
			{
				var connectionString = $"mongodb://{mongoDbConfig.User}:{mongoDbConfig.Password}" +
					$"@{mongoDbConfig.Host}:{mongoDbConfig.Port}/{mongoDbConfig.Database}";

				var client = new MongoDB.Driver.MongoClient(connectionString);
				var db = client.GetDatabase(mongoDbConfig.Database);

				services.AddSingleton(db);
				services.AddScoped<ISnippetRepository, MongoDbSnippetRepository>();
				services.AddScoped<ISnippetQuery, MongoDbSnippetQuery>();
			}
			else if (mysqlConfig != null)
			{
				services.AddSingleton(mysqlConfig);
				services.AddScoped<ISnippetRepository, MySqlSnippetRepository>();
				services.AddScoped<ISnippetQuery, MySqlSnippetQuery>();
				services.AddScoped<MySqlDbContext>();
			}
			else
			{
				if (fileConfig == null) fileConfig = new FileConfiguration();

				services.AddSingleton(fileConfig);
				services.AddScoped<ISnippetRepository, FileSnippetRepository>();
				services.AddScoped<ISnippetQuery, FileSnippetQuery>();
			}

			if (rmqConfig != null)
			{
				rmqConfig.QueueName = "SnippetStudio.Service";
				services.AddCosei(rmqConfig);
				_useCoseiRabbitMq = true;
			}

			services.AddScoped<SnippetService>();
			services.AddScoped<TokenService>();
			services.AddSignalR();

			var validationParameters = TokenService.GetValidationParameters(serviceConfig.Secret);

			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.TokenValidationParameters = validationParameters;
			});

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseAuthentication();
			// app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<CoseiHub>("/cosei");
			});

			if (_useCoseiRabbitMq)
			{
				app.UseCosei();
			}

			using var scope = app.ApplicationServices.CreateScope();
			scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();
		}
	}
}
