using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface IProfilesService
	{
		List<ProfileBase> Load();
		void Remove(ProfileBase profile);
		void Save(List<ProfileBase> profiles);
	}
}
