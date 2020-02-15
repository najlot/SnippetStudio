using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class SnippetValidationList : ValidationList<SnippetModel>
	{
		protected override IEnumerable<ValidationBase<SnippetModel>> GetValidations()
		{
			yield return new SnippetValidation();
		}
	}
}
