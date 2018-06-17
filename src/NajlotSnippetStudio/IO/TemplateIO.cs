using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public static class TemplateIO
	{
		public static void SaveAsXML( ViewModel.MainWindow mw )
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string appDataVolder = Path.Combine(appData, "NajlotSnippetStudio");
			string fileName = Path.Combine(appDataVolder, "NajlotSnippetStudio.xml");
			if (!Directory.Exists(appDataVolder)) Directory.CreateDirectory(appDataVolder);

			if (File.Exists(fileName))
			{
				File.Copy(fileName, fileName + ".bak", true);
				File.Delete(fileName);
			}

			try
			{
				var data = mw;
				data.SaveID++;

				XmlSerializer xsSubmit = new XmlSerializer(typeof(ViewModel.MainWindow));
				var subReq = data;
				var xml = "";

				using (var sww = new StringWriter())
				{
					using (XmlWriter writer = XmlWriter.Create(sww))
					{
						xsSubmit.Serialize(writer, subReq);
						xml = sww.ToString();
					}
				}

				File.WriteAllText(fileName, xml, System.Text.Encoding.Unicode);

				File.Delete(fileName + ".bak");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
				File.Copy(fileName + ".bak", fileName);
			}
		}

		public static void LoadFromXML( out ViewModel.MainWindow mw)
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string appDataVolder = Path.Combine(appData, "NajlotSnippetStudio");
			string fileName = Path.Combine(appDataVolder, "NajlotSnippetStudio.xml");
			if (!Directory.Exists(appDataVolder)) Directory.CreateDirectory(appDataVolder);

			if (!File.Exists(fileName))
			{
				mw = new ViewModel.MainWindow();
				mw.CurrentTemplate = new ViewModel.Template();
				mw.CurrentTemplate.IsEnabled = false;
				return;
			}

			XmlSerializer xsSubmit = new XmlSerializer(typeof(ViewModel.MainWindow));

			using (var file = File.OpenRead(fileName))
			{
				mw = xsSubmit.Deserialize(file) as ViewModel.MainWindow;
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
