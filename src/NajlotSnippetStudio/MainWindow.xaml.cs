using NajlotSnippetStudio.IO;
using NajlotSnippetStudio.Utils;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NajlotSnippetStudio
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.Icon = NajlotSnippetStudio.Resources.Resource.App.ToImageSource();

			ViewModel.MainWindow data;
			TemplateReader.ReadAllTemplates(out data);
			this.DataContext = data;

			var args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				foreach (var tpl in data.Templates)
				{
					if (tpl.Name == args[1])
					{
						this.Visibility = Visibility.Hidden;
						var output = new System.Collections.Specialized.StringCollection();
						TemplateRunner.Run(tpl, ref output);

						var outStr = "";

						foreach (var line in output)
						{
							outStr += line + "\r\n";
						}

						if (outStr.Length > 0)
						{
							MessageBox.Show(outStr, "Compile Error", MessageBoxButton.OK, MessageBoxImage.Error);
						}

						this.Close();
						return;
					}
				}
			}

			this.Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				TemplateWriter.Save(this.DataContext as ViewModel.MainWindow);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
