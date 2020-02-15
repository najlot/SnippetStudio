using System;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public interface ILanguageRepository : IRepository<Guid, LanguageModel>
	{
	}
}