using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;

namespace SnippetStudio.Service
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			LogAdminitrator.Instance
				.SetLogLevel(Najlot.Log.LogLevel.Debug)
				.SetExecutionMiddleware<ChannelExecutionMiddleware>()
				.SetQueueMiddleware<ChannelQueueMiddleware, ConsoleLogDestination>()
				.AddConsoleLogDestination(true);

			CreateWebHostBuilder(args).Build().Run();

			LogAdminitrator.Instance.Dispose();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(builder =>
				{
					builder.ClearProviders();
					builder.AddNajlotLog(LogAdminitrator.Instance);
				})
				.UseStartup<Startup>();
	}
}