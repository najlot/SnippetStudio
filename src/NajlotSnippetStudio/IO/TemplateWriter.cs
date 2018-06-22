using NajlotSnippetStudio.Utils;
using NajlotSnippetStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace NajlotSnippetStudio.IO
{
	public class TemplateWriter
	{
		private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static readonly string NajlotAppDataFolder = Path.Combine(AppDataFolder, "NajlotSnippetStudio");
		
		public static void Save(Template template)
		{
			string fileName = Path.Combine(NajlotAppDataFolder, template.Name + ".nss");
			
			try
			{
				var dependenciesString = XmlUtils.ObjectToXmlString(template.Dependencies);
				var variablesString = XmlUtils.ObjectToXmlString(template.Variables);

				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}

				using (var fileStream = File.OpenWrite(fileName))
				{
					using (var dependenciesStream = StreamUtils.StringToStream(dependenciesString))
					{
						using (var variablesStream = StreamUtils.StringToStream(variablesString))
						{
							using (var templateStream = StreamUtils.StringToStream(template.TemplateString))
							{
								using (var codeStream = StreamUtils.StringToStream(template.Code))
								{
									using (var versionStream = StreamUtils.StringToStream("1.0"))
									{
										var zipArchiveStreamEntries = new List<ZipArchiveStreamEntry>();
										zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry("Version", versionStream));
										zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry("Dependencies", dependenciesStream));
										zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry("Variables", variablesStream));
										zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry("Template", templateStream));
										zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry("Code", codeStream));
										ZipArchiveUtils.ZipArchiveStreamEntriesToStream(zipArchiveStreamEntries, fileStream);
									}
								}
							}
						}
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
