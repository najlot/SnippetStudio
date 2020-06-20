using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class UserValidationList : ValidationList<UserModel>
	{
		public UserValidationList()
		{
			Add(new UserValidation());
		}
	}
}
