using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class User : IUser
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }
	}
}
