using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface ISnippet<TDependency, TVariable>
		where TDependency : IDependency
		where TVariable : IVariable
	{
		Guid Id { get; set; }
		string Name { get; set; }
		Guid LanguageId { get; set; }
		List<TDependency> Dependencies { get; set; }
		List<TVariable> Variables { get; set; }
		string Template { get; set; }
		string Code { get; set; }
	}
}
