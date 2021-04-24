using Microsoft.Extensions.DependencyInjection;
using System;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.Services.Implementation;
using SnippetStudio.ClientBase.ViewModel;
using SnippetStudio.Wpf.Services;

namespace SnippetStudio.Wpf.ViewModel
{
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			var messenger = new Messenger();
			var dispatcher = new DispatcherHelper();
			var serviceCollection = new ServiceCollection();
			Main = new MainViewModel();
			var errorService = new ErrorService(Main);

			serviceCollection.AddSingleton<IDiskSearcher, DiskSearcher>();
			serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

			// Register services
			serviceCollection.AddSingleton<IErrorService>(errorService);
			serviceCollection.AddSingleton<IProfilesService, ProfilesService>();
			serviceCollection.AddSingleton<IMessenger>(messenger);

			var clipboardService = new ClipboardService();

			var profileHandler = new LocalProfileHandler(messenger, dispatcher, clipboardService);
			profileHandler
				.SetNext(new RestProfileHandler(messenger, dispatcher, errorService, clipboardService))
				.SetNext(new RmqProfileHandler(messenger, dispatcher, errorService, clipboardService));

			serviceCollection.AddSingleton<IProfileHandler>(profileHandler);
			serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetSnippetService());
			serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());

			// Register viewmodels
			serviceCollection.AddSingleton<LoginViewModel>();
			serviceCollection.AddTransient<LoginProfileViewModel>();
			serviceCollection.AddSingleton<Func<LoginProfileViewModel>>(c => () => c.GetRequiredService<LoginProfileViewModel>());
			serviceCollection.AddScoped<MenuViewModel>();

			serviceCollection.AddScoped<AllSnippetsViewModel>();
			serviceCollection.AddScoped<AllUsersViewModel>();

			serviceCollection.AddSingleton<INavigationService>(Main);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			Main.NavigateForward(serviceProvider.GetRequiredService<LoginViewModel>());
		}

		public MainViewModel Main { get; }
	}
}
