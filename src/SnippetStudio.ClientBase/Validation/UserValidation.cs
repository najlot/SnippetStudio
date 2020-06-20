using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class UserValidation : ValidationBase<UserModel>
	{
		public override IEnumerable<ValidationResult> Validate(UserModel o)
		{
			return Array.Empty<ValidationResult>();
		}
	}
}
