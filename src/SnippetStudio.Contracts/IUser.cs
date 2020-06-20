using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface IUser
	{
		Guid Id { get; set; }
		string Username { get; set; }
		string EMail { get; set; }
		string Password { get; set; }
	}
}
