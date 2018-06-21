using System;
using System.IO;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	[Obsolete("Please use the TemplateReader. This class is only for compability and will be removed in one of the following releases.")]
	public static class SingleXMLTemplateReader
	{
		public static ViewModel.MainWindow Load(string fileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ViewModel.MainWindow));
			ViewModel.MainWindow mainWindow;

			using (var file = File.OpenRead(fileName))
			{
				mainWindow = xmlSerializer.Deserialize(file) as ViewModel.MainWindow;
			}

			foreach (var tpl in mainWindow.Templates)
			{
				foreach (var dep in tpl.Dependencies)
				{
					dep.Dependencies = tpl.Dependencies;
				}

				foreach (var variable in tpl.Variables)
				{
					variable.Variables = tpl.Variables;
				}
			}

			if (mainWindow.Templates.Count == 0)
			{
				mainWindow.CurrentTemplate = new ViewModel.Template();
				mainWindow.CurrentTemplate.IsEnabled = false;
			}

			return mainWindow;
		}
	}
}
