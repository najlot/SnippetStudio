using System;
using System.Collections.Generic;
using SnippetStudio.Contracts;

namespace SnippetStudio.Service.Query
{
	public interface ISnippetQuery : IQuery<Guid, Snippet>
	{
		IEnumerable<Snippet> GetAllForUser(string username);
	}
}
