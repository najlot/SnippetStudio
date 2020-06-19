using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class DependencyValidationList : ValidationList<DependencyModel>
	{
		public DependencyValidationList()
		{
			Add(new DependencyValidation());
		}
	}
}
