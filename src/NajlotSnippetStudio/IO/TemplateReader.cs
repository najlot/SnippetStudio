using NajlotSnippetStudio.Utils;
using NajlotSnippetStudio.ViewModel;
using System;
using System.Collections.ObjectModel;
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
				var template = new Template
				{
					Name = Path.GetFileNameWithoutExtension(filePath)
				};

				template.OriginalName = template.Name;

				foreach (var entry in ZipArchiveUtils.StreamToZipArchiveEntries(fileStream))
				{
					switch(entry.EntryName)
					{
						case "Version":
							var version = StreamUtils.StreamToString(entry.Stream, false);
							
							if (version != "1.0")
							{
								throw new NotImplementedException($"Version {version} is not supported!");
							}

							break;

						case "Dependencies":
							template.Dependencies = XmlUtils.XmlStreamToObject<ObservableCollection<Dependency>>(entry.Stream, false);

							foreach (var dep in template.Dependencies)
							{
								dep.Dependencies = template.Dependencies;
							}

							break;
							
						case "Variables":
							template.Variables = XmlUtils.XmlStreamToObject<ObservableCollection<Variable>>(entry.Stream, false);

							foreach (var variable in template.Variables)
							{
								variable.Variables = template.Variables;
							}

							break;

						case "Template":
							template.TemplateString = StreamUtils.StreamToString(entry.Stream, false);
							break;

						case "Code":
							template.Code = StreamUtils.StreamToString(entry.Stream, false);
							break;
					}
				}
				
				return template;
			}
		}
	}
}
