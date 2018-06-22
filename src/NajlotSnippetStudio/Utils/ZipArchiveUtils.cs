using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NajlotSnippetStudio.Utils
{
	public static class ZipArchiveUtils
	{
		public static void StreamToArchieveEntry(ZipArchive archive, string entryName, Stream stream)
		{
			var zipArchiveEntryVersion = archive.CreateEntry(entryName, CompressionLevel.Optimal);
			using (var zipStream = zipArchiveEntryVersion.Open())
			{
				stream.CopyTo(zipStream);
			}
		}

		public static Stream ZipArchiveStreamEntriesToStream(IEnumerable<ZipArchiveStreamEntry> zipArchiveStreamEntries)
		{
			var memoryStream = new MemoryStream();
			ZipArchiveStreamEntriesToStream(zipArchiveStreamEntries, memoryStream);

			memoryStream.Seek(0, SeekOrigin.Begin);
			return memoryStream;
		}

		public static void ZipArchiveStreamEntriesToStream(IEnumerable<ZipArchiveStreamEntry> zipArchiveStreamEntries, Stream stream)
		{
			using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
			{
				foreach (var streamEntry in zipArchiveStreamEntries)
				{
					StreamToArchieveEntry(archive, streamEntry.EntryName, streamEntry.Stream);
				}
			}
		}

		public static List<ZipArchiveStreamEntry> StreamToZipArchiveEntries(Stream zipArchiveStream)
		{
			var zipArchiveStreamEntries = new List<ZipArchiveStreamEntry>();
			
			using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
			{
				foreach(var entry in zipArchive.Entries)
				{
					var memoryStream = new MemoryStream();

					using (var entryStream = entry.Open())
					{
						entryStream.CopyTo(memoryStream);
					}

					memoryStream.Seek(0, SeekOrigin.Begin);
					zipArchiveStreamEntries.Add(new ZipArchiveStreamEntry(entry.FullName, memoryStream));
				}
			}

			return zipArchiveStreamEntries;
		}
	}
}
