using System;
using SnippetStudio.Contracts;

namespace SnippetStudio.Service.Query
{
	public interface IUserQuery : IQuery<Guid, User>
	{
	}
}
