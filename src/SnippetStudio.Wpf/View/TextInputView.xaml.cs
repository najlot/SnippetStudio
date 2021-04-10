using System.Windows.Controls;

namespace SnippetStudio.Wpf.View
{
	/// <summary>
	/// Interaction logic for TextInputView.xaml
	/// </summary>
	public partial class TextInputView : UserControl
	{
		public TextInputView()
		{
			InitializeComponent();

			Loaded += (sender, e) => OkButton.Focus();
		}
	}
}
