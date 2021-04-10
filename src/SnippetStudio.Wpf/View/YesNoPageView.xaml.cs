using System.Windows.Controls;

namespace SnippetStudio.Wpf.View
{
	/// <summary>
	/// Interaction logic for YesNoPageView.xaml
	/// </summary>
	public partial class YesNoPageView : UserControl
	{
		public YesNoPageView()
		{
			InitializeComponent();

			Loaded += (sender, e) => YesButton.Focus();
		}
	}
}
