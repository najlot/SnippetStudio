using System;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Query
{
	public interface ISnippetQuery : IAsyncQuery<Guid, SnippetModel>
	{
	}
}
