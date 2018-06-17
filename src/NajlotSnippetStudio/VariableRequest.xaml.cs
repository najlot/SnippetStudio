using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NajlotSnippetStudio
{
	/// <summary>
	/// Interaction logic for VariableRequest.xaml
	/// </summary>
	public partial class VariableRequest : Window
	{
		public VariableRequest()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var variable = this.DataContext as ViewModel.Variable;

			variable.Cancel = true;

			this.Close();
		}
	}
}
