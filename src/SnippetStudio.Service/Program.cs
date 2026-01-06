using Cosei.Service.Base;
using Cosei.Service.Http;
using Cosei.Service.RabbitMq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using SnippetStudio.Service.Configuration;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;
using SnippetStudio.Service.Services;
using System;

namespace SnippetStudio.Service
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			LogAdministrator.Instance
				.SetLogLevel(Najlot.Log.LogLevel.Debug)
				.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
				.AddConsoleDestination(true);

			var builder = WebApplication.CreateBuilder(args);
			builder.Logging.ClearProviders();
			builder.Logging.AddNajlotLog(LogAdministrator.Instance);

			var services = builder.Services;
			var Configuration = builder.Configuration;

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
				string connectionString;

				if (mongoDbConfig.UseDnsSrv)
				{
					connectionString = $"mongodb+srv://{mongoDbConfig.User}:{mongoDbConfig.Password}" +
						$"@{mongoDbConfig.Host}/{mongoDbConfig.Database}";
				}
				else
				{
					connectionString = $"mongodb://{mongoDbConfig.User}:{mongoDbConfig.Password}" +
						$"@{mongoDbConfig.Host}:{mongoDbConfig.Port}/{mongoDbConfig.Database}";
				}

				var client = new MongoDB.Driver.MongoClient(connectionString);
				var db = client.GetDatabase(mongoDbConfig.Database);

				services.AddSingleton(db);
				services.AddScoped<ISnippetRepository, MongoDbSnippetRepository>();
				services.AddScoped<IUserRepository, MongoDbUserRepository>();
				services.AddScoped<ISnippetQuery, MongoDbSnippetQuery>();
				services.AddScoped<IUserQuery, MongoDbUserQuery>();
			}
			else if (mysqlConfig != null)
			{
				services.AddSingleton(mysqlConfig);
				services.AddScoped<ISnippetRepository, MySqlSnippetRepository>();
				services.AddScoped<IUserRepository, MySqlUserRepository>();
				services.AddScoped<ISnippetQuery, MySqlSnippetQuery>();
				services.AddScoped<IUserQuery, MySqlUserQuery>();
				services.AddScoped<MySqlDbContext>();
			}
			else
			{
				if (fileConfig == null) fileConfig = new FileConfiguration();

				services.AddSingleton(fileConfig);
				services.AddScoped<ISnippetRepository, FileSnippetRepository>();
				services.AddScoped<IUserRepository, FileUserRepository>();
				services.AddScoped<ISnippetQuery, FileSnippetQuery>();
				services.AddScoped<IUserQuery, FileUserQuery>();
			}

			if (rmqConfig != null)
			{
				rmqConfig.QueueName = "SnippetStudio.Service";
				services.AddCoseiRabbitMq(rmqConfig);
			}

			services.AddCoseiHttp();

			services.AddScoped<SnippetService>();
			services.AddScoped<UserService>();
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

			var app = builder.Build();

			app.UseCors(c =>
			{
				c.AllowAnyOrigin();
				c.AllowAnyMethod();
				c.AllowAnyHeader();
			});

			app.UseAuthentication();
			// app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();

			app.MapControllers();
			app.MapHub<CoseiHub>("/cosei");

			app.UseCosei();

			using var scope = app.Services.CreateScope();
			scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();

			app.Run();

			LogAdministrator.Instance.Dispose();
		}
	}
}