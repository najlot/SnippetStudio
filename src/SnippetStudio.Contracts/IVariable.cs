using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface IVariable
	{
		int Id { get; set; }
		string Name { get; set; }
		string RequestName { get; set; }
		string DefaultValue { get; set; }
	}
}
