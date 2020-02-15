using System;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public interface ISnippetRepository : IRepository<Guid, SnippetModel>
	{
	}
}