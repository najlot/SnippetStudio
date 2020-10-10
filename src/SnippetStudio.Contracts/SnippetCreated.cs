using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class SnippetCreated
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public List<Variable> Variables { get; set; }
		public string Template { get; set; }
		public string Code { get; set; }
		public string CreatedBy { get; set; }

		private SnippetCreated(){}

		public SnippetCreated(
			Guid id,
			string name,
			string language,
			List<Variable> variables,
			string template,
			string code,
			string createdBy)
		{
			Id = id;
			Name = name;
			Language = language;
			Variables = variables;
			Template = template;
			Code = code;
			CreatedBy = createdBy;
		}
	}
}
