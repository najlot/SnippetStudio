using Microsoft.Extensions.DependencyInjection;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.ViewModel;

namespace SnippetStudio.Wpf.ViewModel
{
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

			// Register services
			serviceCollection.AddSingleton<ErrorService>();
			serviceCollection.AddSingleton<ProfilesService>();
			serviceCollection.AddSingleton<Messenger>();

			var profileHandler = new LocalProfileHandler();
			profileHandler.SetNext(new RestProfileHandler()).SetNext(new RmqProfileHandler());

			serviceCollection.AddSingleton<IProfileHandler>(profileHandler);
			serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetLanguageService());
			serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetSnippetService());

			// Register viewmodels
			serviceCollection.AddSingleton<LoginViewModel>();
			serviceCollection.AddScoped<MenuViewModel>();

			serviceCollection.AddScoped<AllLanguagesViewModel>();
			serviceCollection.AddScoped<AllSnippetsViewModel>();

			Main = new MainViewModel();

			serviceCollection.AddSingleton<INavigationService>(Main);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			Main.NavigateForward(serviceProvider.GetRequiredService<LoginViewModel>());
		}

		public MainViewModel Main { get; }
	}
}
