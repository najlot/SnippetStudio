using CommonServiceLocator;
using NajlotSnippetStudio.IO;
using NajlotSnippetStudio.Utils;
using NajlotSnippetStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

			var args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				foreach (var tpl in TemplateReader.ReadAllTemplates())
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
							MessageBox.Show(outStr, "Build failed", MessageBoxButton.OK, MessageBoxImage.Error);
						}

						this.Close();
						return;
					}
				}
			}

			this.Icon = NajlotSnippetStudio.Resources.Resource.App.ToImageSource();
			
			this.Closing += MainWindow_Closing;
			this.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var mainWindow = ServiceLocator.Current.GetInstance<ViewModel.MainWindow>();
			var templates = TemplateReader.ReadAllTemplates();

			foreach(var template in templates)
			{
				mainWindow.Templates.Add(template);
			}
			
			if (mainWindow.Templates.Count > 0)
			{
				mainWindow.CurrentTemplate = mainWindow.Templates[0];
			}
			else
			{
				mainWindow.CurrentTemplate = new Template()
				{
					IsEnabled = false
				};
			}
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				var mainWindow = this.DataContext as ViewModel.MainWindow;
				TemplateWriter.Save(mainWindow.Templates);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
