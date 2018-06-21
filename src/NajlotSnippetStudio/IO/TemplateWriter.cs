using NajlotSnippetStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public class TemplateWriter : XmlTemplateIoBase
	{
		private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static readonly string NajlotAppDataFolder = Path.Combine(AppDataFolder, "NajlotSnippetStudio");
		
		public static void Save(Template template)
		{
			string fileName = Path.Combine(NajlotAppDataFolder, template.Name + ".nss");
			
			try
			{
				using (var stringWriter = new StringWriter())
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
					{
						XmlTemplateSerializer.Serialize(xmlWriter, template);
						File.WriteAllText(fileName, stringWriter.ToString(), Encoding.Unicode);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		
		private static void Remove(Template template)
		{
			if(template.OriginalName == null)
			{
				return;
			}

			string fileName = Path.Combine(NajlotAppDataFolder, template.OriginalName + ".nss");
			if(File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}
		
		public static void Save(IList<Template> templates)
		{
			if (!Directory.Exists(NajlotAppDataFolder)) Directory.CreateDirectory(NajlotAppDataFolder);
			
			foreach(var templateToRemove in templates.Where(tpl =>
			{
				if(tpl.MarkedForDeletion)
				{
					return true;
				}

				if(tpl.OriginalName == null)
				{
					return false;
				}

				return string.Compare(tpl.OriginalName, tpl.Name, true) != 0;
			}).ToArray())
			{
				Remove(templateToRemove);

				if(templateToRemove.MarkedForDeletion)
				{
					templates.Remove(templateToRemove);
				}
				
			}

			foreach (var template in templates.Where(tpl => tpl.IsChanged))
			{
				Save(template);
			}

			CleanUp(NajlotAppDataFolder);
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
