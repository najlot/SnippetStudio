using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public interface ILanguage
	{
		Guid Id { get; set; }
		string Name { get; set; }
	}
}
