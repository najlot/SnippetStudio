using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class LanguageValidationList : ValidationList<LanguageModel>
	{
		protected override IEnumerable<ValidationBase<LanguageModel>> GetValidations()
		{
			yield return new LanguageValidation();
		}
	}
}
