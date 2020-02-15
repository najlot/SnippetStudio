using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class VariableValidationList : ValidationList<VariableModel>
	{
		protected override IEnumerable<ValidationBase<VariableModel>> GetValidations()
		{
			yield return new VariableValidation();
		}
	}
}
