using System.Threading.Tasks;
using SnippetStudio.ClientBase;
using SnippetStudio.ClientBase.Services;
using SnippetStudio.ClientBase.ViewModel;
using SnippetStudio.Mobile.View;
using Xamarin.Forms;

namespace SnippetStudio.Mobile
{
	public class NavigationServicePage : NavigationPage, INavigationService
	{
		public NavigationServicePage(Page root) : base(root)
		{
		}

		public async void NavigateBack()
		{
			if (Navigation.ModalStack.Count > 0)
			{
				await Navigation.PopModalAsync();
			}
			else
			{
				await Navigation.PopAsync();
			}
		}

		public async void NavigateForward(AbstractViewModel vm)
		{
			ContentPage cp = null;
			bool isPopup = vm is IPopupViewModel;

			if (vm is LoginViewModel)
			{
				cp = new LoginView();
			}
			else if (vm is AllLanguagesViewModel)
			{
				cp = new AllLanguagesView();
			}
			else if (vm is AllSnippetsViewModel)
			{
				cp = new AllSnippetsView();
			}
			else if (vm is YesNoPageViewModel)
			{
				cp = new YesNoPageView();
			}
			else if (vm is DependencyViewModel)
			{
				cp = new DependencyView();
			}
			else if (vm is LanguageViewModel)
			{
				cp = new LanguageView();
			}
			else if (vm is SnippetViewModel)
			{
				cp = new SnippetView();
			}
			else if (vm is VariableViewModel)
			{
				cp = new VariableView();
			}
			else if (vm is MenuViewModel)
			{
				cp = new MenuView();
			}
			else if (vm is AlertViewModel)
			{
				cp = new AlertView();
			}
			else if (vm is ProfileViewModel)
			{
				cp = new ProfileView();
			}

			cp.BindingContext = vm;

			await Task.Delay(100);

			if (isPopup)
			{
				SetHasBackButton(cp, false);
				await Navigation.PushModalAsync(cp);
			}
			else
			{
				await Navigation.PushAsync(cp);
			}
		}
	}
}