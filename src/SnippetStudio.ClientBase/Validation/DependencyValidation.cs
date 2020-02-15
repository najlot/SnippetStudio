using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class DependencyValidation : ValidationBase<DependencyModel>
	{
		public override IEnumerable<ValidationResult> Validate(DependencyModel o)
		{
			return new List<ValidationResult>();
		}
	}
}
