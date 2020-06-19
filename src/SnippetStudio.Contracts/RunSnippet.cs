using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class RunSnippet
	{
		public Guid Id { get; set; }
		public string Language { get; set; }
		public List<string> Dependencies { get; set; }
		public Dictionary<string, string> Variables { get; set; }
		public string Template { get; set; }
		public string Code { get; set; }

		private RunSnippet() { }

		public RunSnippet(
			Guid id,
			string language,
			List<string> dependencies,
			Dictionary<string, string> variables,
			string template,
			string code)
		{
			Id = id;
			Language = language;
			Dependencies = dependencies;
			Variables = variables;
			Template = template;
			Code = code;
		}
	}
}
