using System.Collections.Generic;
using SnippetStudio.ClientBase.Models;

namespace SnippetStudio.ClientBase.Validation
{
	public class SnippetValidationList : ValidationList<SnippetModel>
	{
		public SnippetValidationList()
		{
			Add(new SnippetValidation());
		}
	}
}
