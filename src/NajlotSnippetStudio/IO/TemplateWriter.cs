using NajlotSnippetStudio.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public static class TemplateWriter
	{
		private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Template));

		public static void Save(Template template, string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Copy(fileName, fileName + ".bak", true);
				File.Delete(fileName);
			}

			try
			{
				using (var sww = new StringWriter())
				{
					using (XmlWriter writer = XmlWriter.Create(sww))
					{
						xmlSerializer.Serialize(writer, (object)template);
						File.WriteAllText(fileName, sww.ToString(), System.Text.Encoding.Unicode);
						File.Delete(fileName + ".bak");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
				File.Copy(fileName + ".bak", fileName);
			}
		}

		public static void Save(ViewModel.MainWindow mw)
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string appDataFolder = Path.Combine(appData, "NajlotSnippetStudio");
			if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);

			foreach (var template in mw.Templates)
			{
				Save(template, Path.Combine(appDataFolder, template.Name + ".xml"));
			}

			CleanUp(appDataFolder);
		}

		[Obsolete("Will be removed with SingleXMLTemplateReader in one of the following releases.")]
		private static void CleanUp(string appDataFolder)
		{
			// Drop old version of file, of exists
			if (File.Exists(Path.Combine(appDataFolder, "NajlotSnippetStudio.xml")))
			{
				File.Delete(Path.Combine(appDataFolder, "NajlotSnippetStudio.xml"));
			}
		}
	}
}
