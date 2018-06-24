using NajlotSnippetStudio.Utils;
using NajlotSnippetStudio.ViewModel;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NajlotSnippetStudio.IO
{
	public class TemplateReader
	{
		private static XmlSerializer XmlTemplateSerializer = new XmlSerializer(typeof(Template));
		
		private static string NajlotAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NajlotSnippetStudio");
		
		public static IEnumerable<Template> ReadAllTemplates()
		{
			
			if (!Directory.Exists(NajlotAppDataFolder)) Directory.CreateDirectory(NajlotAppDataFolder);

			string oldFileName = Path.Combine(NajlotAppDataFolder, "NajlotSnippetStudio.xml");

			if (File.Exists(oldFileName))
			{
				return SingleXMLTemplateReader.Load(oldFileName).Templates;
			}

			return Directory.GetFiles(NajlotAppDataFolder, "*.nss").Select(path => ReadTemplateFromPath(path));
		}

		public static Template ReadTemplateFromName(string filePath)
		{
			var path = filePath;

			if (!File.Exists(path))
			{
				path = Path.Combine(NajlotAppDataFolder, filePath);

				if (!File.Exists(path))
				{
					path += ".nss";
				}
			}
			
			return ReadTemplateFromPath(path);
		}

		private static Template ReadTemplateFromPath(string filePath)
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
