using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class Dependency : IDependency
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
