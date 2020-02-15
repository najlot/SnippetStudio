using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface IDependency
	{
		int Id { get; set; }
		string Name { get; set; }
	}
}
