﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Services
{
	public interface IProfilesService
	{
		Task<List<ProfileBase>> LoadAsync();
		Task RemoveAsync(ProfileBase profile);
		Task SaveAsync(List<ProfileBase> profiles);
	}
}
