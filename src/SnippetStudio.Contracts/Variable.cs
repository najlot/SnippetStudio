using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class Variable : IVariable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string RequestName { get; set; }
		public string DefaultValue { get; set; }
	}
}
