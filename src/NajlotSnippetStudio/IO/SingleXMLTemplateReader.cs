using System;
using System.IO;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	[Obsolete("Please use the TemplateReader. This class is only for compability and will be removed in one of the following releases.")]
	public static class SingleXMLTemplateReader
	{
		public static void Load(string fileName, out ViewModel.MainWindow mw)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ViewModel.MainWindow));

			using (var file = File.OpenRead(fileName))
			{
				mw = xmlSerializer.Deserialize(file) as ViewModel.MainWindow;
			}

			foreach (var tpl in mw.Templates)
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

			if (mw.Templates.Count == 0)
			{
				mw.CurrentTemplate = new ViewModel.Template();
				mw.CurrentTemplate.IsEnabled = false;
			}

		}
	}
}
