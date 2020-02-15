using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SnippetStudio.Mobile.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YesNoPageView : ContentPage
	{
		public YesNoPageView()
		{
			InitializeComponent();
		}

		protected override bool OnBackButtonPressed()
		{
			// Reject going back with back-button
			return true;
		}
	}
}