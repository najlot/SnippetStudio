using System.Windows.Controls;

namespace SnippetStudio.Wpf.View
{
	/// <summary>
	/// Interaction logic for LanguageInputView.xaml
	/// </summary>
	public partial class LanguageInputView : UserControl
	{
		public LanguageInputView()
		{
			InitializeComponent();

			Loaded += (sender, e) => LanguagesComboBox.Focus();
		}
	}
}
