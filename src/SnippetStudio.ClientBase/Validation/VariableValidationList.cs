using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class VariableValidationList : ValidationList<VariableModel>
	{
		public VariableValidationList()
		{
			Add(new VariableValidation());
		}
	}
}
