﻿using Microsoft.Extensions.DependencyInjection;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.ViewModel;
using SnippetStudio.Mobile.Services;
using SnippetStudio.Mobile.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SnippetStudio.Mobile
{
	// // //
	// Does not work on Android because of https://github.com/dotnet/roslyn/issues/39501
	// // //

	public partial class App : Application
	{
		private static NavigationServicePage _navigationPage;

		public App()
		{
			if (_navigationPage == null)
			{
				var loginView = new LoginView();
				_navigationPage = new NavigationServicePage(loginView);

				var messenger = new Messenger();
				var dispatcher = new DispatcherHelper();
				var serviceCollection = new ServiceCollection();
				var errorService = new ErrorService(_navigationPage);

				serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

				// Register services
				serviceCollection.AddSingleton(errorService);
				serviceCollection.AddSingleton<ProfilesService>();
				serviceCollection.AddSingleton(messenger);

				var clipboardService = new ClipboardService();

				var profileHandler = new LocalProfileHandler(messenger, dispatcher, clipboardService);
				profileHandler
					.SetNext(new RestProfileHandler(messenger, dispatcher, errorService, clipboardService))
					.SetNext(new RmqProfileHandler(messenger, dispatcher, errorService, clipboardService));

				serviceCollection.AddSingleton<IProfileHandler>(profileHandler);
				serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetSnippetService());

				// Register viewmodels
				serviceCollection.AddSingleton<LoginViewModel>();
				serviceCollection.AddScoped<MenuViewModel>();

				serviceCollection.AddScoped<AllSnippetsViewModel>();

				serviceCollection.AddSingleton<INavigationService>(_navigationPage);

				var serviceProvider = serviceCollection.BuildServiceProvider();

				loginView.BindingContext = serviceProvider.GetRequiredService<LoginViewModel>();
			}

			MainPage = _navigationPage;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
