using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class SnippetUpdated
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public List<Variable> Variables { get; set; }
		public string Template { get; set; }
		public string Code { get; set; }

		private SnippetUpdated(){}

		public SnippetUpdated(
			Guid id,
			string name,
			string language,
			List<Variable> variables,
			string template,
			string code)
		{
			Id = id;
			Name = name;
			Language = language;
			Variables = variables;
			Template = template;
			Code = code;
		}
	}
}
