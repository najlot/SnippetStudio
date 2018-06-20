using NajlotSnippetStudio.ViewModel;
using System;
using System.IO;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public static class TemplateReader
	{
		private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Template));

		public static void ReadAllTemplates(out ViewModel.MainWindow mw)
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string appDataFolder = Path.Combine(appData, "NajlotSnippetStudio");
			
			if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);

			string oldFileName = Path.Combine(appDataFolder, "NajlotSnippetStudio.xml");

			if (File.Exists(oldFileName))
			{
				SingleXMLTemplateReader.Load(oldFileName, out mw);
				return;
			}

			mw = new ViewModel.MainWindow();
			
			foreach (var filePath in Directory.GetFiles(appDataFolder))
			{
				ReadTemplate(mw, filePath);
			}
		}

		private static void ReadTemplate(ViewModel.MainWindow mw, string filePath)
		{
			using (var file = File.OpenRead(filePath))
			{
				var tpl = xmlSerializer.Deserialize(file) as Template;

				foreach (var dep in tpl.Dependencies)
				{
					dep.Dependencies = tpl.Dependencies;
				}

				foreach (var variable in tpl.Variables)
				{
					variable.Variables = tpl.Variables;
				}

				mw.Templates.Add(tpl);
			}
		}
	}
}
