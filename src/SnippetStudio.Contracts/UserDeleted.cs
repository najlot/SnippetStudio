using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class UserDeleted
	{
		public Guid Id { get; set; }

		private UserDeleted(){}

		public UserDeleted(Guid id)
		{
			Id = id;
		}
	}
}
