using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface ISnippet<TVariable>
		where TVariable : IVariable
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string Language { get; set; }
		List<TVariable> Variables { get; set; }
		string Template { get; set; }
		string Code { get; set; }
	}
}
