using Microsoft.Extensions.DependencyInjection;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.ProfileHandler;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.ViewModel;
using SnippetStudio.Mobile.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SnippetStudio.Mobile
{
	public partial class App : Application
	{
		private static NavigationServicePage _navigationPage;

		public App()
		{
			if (_navigationPage == null)
			{
				var loginView = new LoginView();
				_navigationPage = new NavigationServicePage(loginView);

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
