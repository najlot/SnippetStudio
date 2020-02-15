using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class VariableValidation : ValidationBase<VariableModel>
	{
		public override IEnumerable<ValidationResult> Validate(VariableModel o)
		{
			return new List<ValidationResult>();
		}
	}
}
