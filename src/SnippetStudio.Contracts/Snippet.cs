using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class Snippet : ISnippet<Variable>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public List<Variable> Variables { get; set; }
		public string Template { get; set; }
		public string Code { get; set; }
	}
}
