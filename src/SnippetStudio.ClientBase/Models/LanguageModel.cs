using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Models
{
	public class LanguageModel : AbstractValidationModel, ILanguage
	{
		private string _name;

		public Guid Id { get; set; }

		public string Name { get => _name; set => Set(ref _name, value); }
	}
}
