using NajlotSnippetStudio.Utils;
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
			ViewModel.MainWindow mainWindow;

			using (var file = File.OpenRead(fileName))
			{
				mainWindow = XmlUtils.XmlStreamToObject<ViewModel.MainWindow>(file, true);
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
				mainWindow.CurrentTemplate = new ViewModel.Template()
				{
					IsEnabled = false
				};
			}

			return mainWindow;
		}
	}
}
