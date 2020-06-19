using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class SnippetValidation : ValidationBase<SnippetModel>
	{
		public override IEnumerable<ValidationResult> Validate(SnippetModel o)
		{
			return Array.Empty<ValidationResult>();
		}
	}
}
