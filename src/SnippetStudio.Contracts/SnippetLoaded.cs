using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class SnippetLoaded : SnippetUpdated
	{
		public SnippetLoaded(
			Guid id,
			string name,
			string language,
			List<Variable> variables,
			string template,
			string code) : base(
				id,
				name,
				language,
				variables,
				template,
				code)
		{
		}
	}
}
