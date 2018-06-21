using NajlotSnippetStudio.ViewModel;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.IO
{
	public abstract class XmlTemplateIoBase
	{
		protected static XmlSerializer XmlTemplateSerializer = new XmlSerializer(typeof(Template));
		/*
		 * does not work with list
		private static XmlSerializer DependenciesSerializer = new XmlSerializer(typeof(IList<Dependency>));
		private static XmlSerializer TemplatesSerializer = new XmlSerializer(typeof(IList<Dependency>));
		private static XmlSerializer VariablesSerializer = new XmlSerializer(typeof(IList<Dependency>));
		*/
	}
}
