using System.IO;

namespace NajlotSnippetStudio.Utils
{
	public class ZipArchiveStreamEntry
	{
		public string EntryName { get; private set; }
		public Stream Stream { get; private set; }

		public ZipArchiveStreamEntry(string entryName, Stream stream)
		{
			EntryName = entryName;
			Stream = stream;
		}
	}
}
