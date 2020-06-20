using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Models
{
	public class SnippetModel : AbstractValidationModel, ISnippet<Variable>
	{
		private string _name;
		private string _language;
		private List<Variable> _variables;
		private string _template;
		private string _code;

		public Guid Id { get; set; }

		public string Name { get => _name; set => Set(ref _name, value); }
		public string Language { get => _language; set => Set(ref _language, value); }
		public List<Variable> Variables { get => _variables; set => Set(ref _variables, value); }
		public string Template { get => _template; set => Set(ref _template, value); }
		public string Code { get => _code; set => Set(ref _code, value); }
	}
}
