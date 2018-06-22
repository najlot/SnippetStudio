using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NajlotSnippetStudio.Utils
{
	public class XmlUtils
	{
		public static string ObjectToXmlString<T>(T objectInstance) where T: class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
				{
					xmlSerializer.Serialize(xmlWriter, objectInstance);
					return stringWriter.ToString();
				}
			}
		}

		public static T XmlStreamToObject<T>(Stream stream, bool leaveOpen) where T: class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (var streamReader = new StreamReader(stream, Encoding.Default, true, 1024, leaveOpen))
			{
				return xmlSerializer.Deserialize(streamReader) as T;
			}
		}
	}
}
