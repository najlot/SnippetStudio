using System.Windows.Controls;

namespace SnippetStudio.Wpf.View
{
	/// <summary>
	/// Interaction logic for ResultViewModel.xaml
	/// </summary>
	public partial class ResultView : UserControl
	{
		public ResultView()
		{
			InitializeComponent();

			Loaded += (sender, e) => OkButton.Focus();
		}
	}
}
