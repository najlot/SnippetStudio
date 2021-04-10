using System.Windows.Controls;

namespace SnippetStudio.Wpf.View
{
	/// <summary>
	/// Interaction logic for SnippetView.xaml
	/// </summary>
	public partial class SnippetView : UserControl
	{
		public SnippetView()
		{
			InitializeComponent();

			Loaded += (sender, e) => NameTextBox.Focus();
		}

		private void TextEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			ContentScrollViewer.ScrollToVerticalOffset(ContentScrollViewer.ContentVerticalOffset + e.Delta * -1);
		}
	}
}