using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajlotSnippetStudio.Utils
{
	public class StreamUtils
	{
		public static Stream StringToStream(string s)
		{
			var stream = new MemoryStream();

			using (var streamWriter = new StreamWriter(stream, Encoding.Default, s.Length, true))
			{
				streamWriter.Write(s);
				streamWriter.Flush();
			}

			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
	}
}
