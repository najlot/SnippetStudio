using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services.Implementation
{
	public class ProfilesService : IProfilesService
	{
		public class KnownTypesBinder : ISerializationBinder
		{
			public IList<Type> KnownTypes { get; set; }

			public Type BindToType(string assemblyName, string typeName)
			{
				return KnownTypes.SingleOrDefault(t => t.Name == typeName);
			}

			public void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				assemblyName = null;
				typeName = serializedType.Name;
			}
		}

		private static readonly string _profilesPath = BuildProfilesPath();
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Objects,
			SerializationBinder = new KnownTypesBinder()
			{
				KnownTypes = new List<Type>
				{
					typeof(LocalProfile),
					typeof(RestProfile),
					typeof(RmqProfile)
				}
			}
		};

		private List<ProfileBase> _profiles;

		private static string BuildProfilesPath()
		{
			var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnippetStudio");
			Directory.CreateDirectory(appdataDir);
			return Path.Combine(appdataDir, "Profiles.json");
		}

		public List<ProfileBase> Load()
		{
			// Take cache if not null
			if (_profiles != null)
			{
				return _profiles;
			}

			// Try load from file
			if (File.Exists(_profilesPath))
			{
				var content = File.ReadAllText(_profilesPath);
				_profiles = JsonConvert.DeserializeObject<List<ProfileBase>>(content, _jsonSerializerSettings);
				return _profiles;
			}

			// Try migrate from old
			var NajlotAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NajlotSnippetStudio");

			if (Directory.Exists(NajlotAppDataFolder))
			{
				var oldEntries = new List<ProfileBase>()
				{
					new LocalProfile()
					{
						Id = Guid.NewGuid(),
						Name = "Local",
						FolderName = NajlotAppDataFolder,
						Source = Source.Local
					}
				};

				Save(oldEntries);

				return oldEntries;
			}

			var id = Guid.NewGuid();

			var list = new List<ProfileBase>()
			{
				new LocalProfile()
				{
					Id = id,
					Name = "Local",
					FolderName = id.ToString(),
					Source = Source.Local
				}
			};

			Save(list);

			return list;
		}

		public void Remove(ProfileBase profile)
		{
			var profiles = _profiles.Where(p => p.Id != profile.Id).ToList();
			Save(profiles);
		}

		public void Save(List<ProfileBase> profiles)
		{
			// Set cache
			_profiles = profiles;

			// Write to file
			var content = JsonConvert.SerializeObject(profiles, _jsonSerializerSettings);
			File.WriteAllText(_profilesPath, content);
		}
	}
}