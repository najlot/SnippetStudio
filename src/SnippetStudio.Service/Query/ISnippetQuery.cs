using System;
using System.Collections.Generic;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public interface ISnippetQuery : IAsyncQuery<Guid, SnippetModel>
	{
		IAsyncEnumerable<SnippetModel> GetAllForUserAsync(string username);
	}
}
