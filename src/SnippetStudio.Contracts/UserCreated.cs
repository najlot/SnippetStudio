using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class UserCreated
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }

		private UserCreated(){}

		public UserCreated(
			Guid id,
			string username,
			string eMail,
			string password)
		{
			Id = id;
			Username = username;
			EMail = eMail;
			Password = password;
		}
	}
}
