using System;
using SnippetStudio.Contracts;

namespace SnippetStudio.Service.Query
{
	public interface ILanguageQuery : IQuery<Guid, Language>
	{
	}
}
