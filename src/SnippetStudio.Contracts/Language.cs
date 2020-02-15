using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class Language : ILanguage
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
