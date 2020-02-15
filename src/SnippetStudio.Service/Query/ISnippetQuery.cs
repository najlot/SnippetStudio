using System;
using SnippetStudio.Contracts;

namespace SnippetStudio.Service.Query
{
	public interface ISnippetQuery : IQuery<Guid, Snippet>
	{
	}
}
