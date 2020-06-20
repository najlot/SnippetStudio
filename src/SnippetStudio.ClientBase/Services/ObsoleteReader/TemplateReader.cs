using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SnippetStudio.ClientBase.Services.ObsoleteReader
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
				foreach (var entry in zipArchive.Entries)
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

	public class StreamUtils
	{
		public static Stream StringToStream(string s)
		{
			var stream = new MemoryStream();

			using (var streamWriter = new StreamWriter(stream, Encoding.Default, s.Length + 5, true))
			{
				streamWriter.Write(s);
				streamWriter.Flush();
			}

			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

		public static string StreamToString(Stream stream, bool leaveOpen)
		{
			using (var reader = new StreamReader(stream, Encoding.Default, true, 1024, leaveOpen))
			{
				return reader.ReadToEnd();
			}
		}
	}

	public class XmlUtils
	{
		public static string ObjectToXmlString<T>(T objectInstance) where T : class
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

		public static T XmlStreamToObject<T>(Stream stream, bool leaveOpen) where T : class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (var streamReader = new StreamReader(stream, Encoding.Default, true, 1024, leaveOpen))
			{
				return xmlSerializer.Deserialize(streamReader) as T;
			}
		}
	}

	public class Variable
	{
		public string Name { get; set; }
		public string RequestName { get; set; }
		public string Default { get; set; }
	}

	public class Template
	{
		public string Name { get; set; }
		public string OriginalName { get; set; }

		public string TemplateString { get; set; } = "";

		public string Code { get; set; }

		public string CodeLanguage { get; set; }

		[XmlArray("Variables")]
		public List<Variable> Variables { get; set; } = new List<Variable>();

		[XmlArray("Dependencies")]
		public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
	}

	public class Dependency
	{
		public string Assembly { get; set; }
	}

	public static class TemplateReader
	{
		public static IEnumerable<Models.SnippetModel> ReadAllTemplates(string sourcePath)
		{
			int variableId = 0;

			return Directory
				.GetFiles(sourcePath, "*.nss")
				.Select(path => ReadTemplateFromPath(path))
				.Select(old => new Models.SnippetModel()
				{
					Id = Guid.NewGuid(),
					Name = old.OriginalName,
					// Language = old.CodeLanguage,
					Code = old.Code,
					Template = old.TemplateString,
					Variables = old.Variables.Select(d => new Contracts.Variable
					{
						Id = ++variableId,
						Name = d.Name,
						DefaultValue = d.Default,
						RequestName = d.RequestName
					}).ToList(),
				});
		}

		private static Template ReadTemplateFromPath(string filePath)
		{
			using (var fileStream = File.OpenRead(filePath))
			{
				var template = new Template
				{
					Name = Path.GetFileNameWithoutExtension(filePath)
				};

				template.OriginalName = template.Name;

				foreach (var entry in ZipArchiveUtils.StreamToZipArchiveEntries(fileStream))
				{
					switch (entry.EntryName)
					{
						case "Version":
							var version = StreamUtils.StreamToString(entry.Stream, false);

							if (version != "1.0")
							{
								throw new NotImplementedException($"Version {version} is not supported!");
							}

							break;

						case "Dependencies":
							template.Dependencies = XmlUtils.XmlStreamToObject<List<Dependency>>(entry.Stream, false);
							break;

						case "Variables":
							template.Variables = XmlUtils.XmlStreamToObject<List<Variable>>(entry.Stream, false);
							break;

						case "Template":
							template.TemplateString = StreamUtils.StreamToString(entry.Stream, false);
							break;

						case "Code":
							template.Code = StreamUtils.StreamToString(entry.Stream, false);
							break;
					}
				}

				return template;
			}
		}
	}
}
