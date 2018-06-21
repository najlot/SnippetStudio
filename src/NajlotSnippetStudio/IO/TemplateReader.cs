using NajlotSnippetStudio.ViewModel;
using System;
using System.IO;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public class TemplateReader
	{
		private static XmlSerializer XmlTemplateSerializer = new XmlSerializer(typeof(Template));

		public static ViewModel.MainWindow ReadAllTemplates()
		{
			string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string najlotAppDataFolder = Path.Combine(appDataFolder, "NajlotSnippetStudio");
			
			if (!Directory.Exists(najlotAppDataFolder)) Directory.CreateDirectory(najlotAppDataFolder);

			string oldFileName = Path.Combine(najlotAppDataFolder, "NajlotSnippetStudio.xml");

			if (File.Exists(oldFileName))
			{
				return SingleXMLTemplateReader.Load(oldFileName);
			}

			ViewModel.MainWindow mainWindow = new ViewModel.MainWindow();
			
			foreach (var filePath in Directory.GetFiles(najlotAppDataFolder, "*.nss"))
			{
				var template = ReadTemplate(filePath);
				
				template.Name = Path.GetFileNameWithoutExtension(filePath);
				template.OriginalName = template.Name;
				mainWindow.Templates.Add(template);
			}

			if(mainWindow.Templates.Count > 0)
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

			return mainWindow;
		}

		private static Template ReadTemplate(string filePath)
		{
			using (var fileStream = File.OpenRead(filePath))
			{
				var template = XmlTemplateSerializer.Deserialize(fileStream) as Template;

				foreach (var dependency in template.Dependencies)
				{
					dependency.Dependencies = template.Dependencies;
				}

				foreach (var variable in template.Variables)
				{
					variable.Variables = template.Variables;
				}

				return template;
			}
		}
	}
}
