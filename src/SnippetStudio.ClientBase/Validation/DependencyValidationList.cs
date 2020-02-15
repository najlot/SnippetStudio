using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class DependencyValidationList : ValidationList<DependencyModel>
	{
		protected override IEnumerable<ValidationBase<DependencyModel>> GetValidations()
		{
			yield return new DependencyValidation();
		}
	}
}
