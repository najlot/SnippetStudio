using System;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public interface IUserRepository : IRepository<Guid, UserModel>
	{
		UserModel Get(string username);
	}
}