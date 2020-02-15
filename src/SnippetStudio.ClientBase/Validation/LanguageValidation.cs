using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class LanguageValidation : ValidationBase<LanguageModel>
	{
		public override IEnumerable<ValidationResult> Validate(LanguageModel o)
		{
			return new List<ValidationResult>();
		}
	}
}
