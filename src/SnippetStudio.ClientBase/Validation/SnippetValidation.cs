using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class SnippetValidation : ValidationBase<SnippetModel>
	{
		public override IEnumerable<ValidationResult> Validate(SnippetModel o)
		{
			if (string.IsNullOrWhiteSpace(o.Name))
			{
				yield return Warning(nameof(o.Name), "Name should be provided");
			}

			if (string.IsNullOrWhiteSpace(o.Code))
			{
				yield return Error(nameof(o.Code), "Code should be provided");
			}
		}
	}
}
