using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Models
{
	public class DependencyModel : AbstractValidationModel, IDependency
	{
		private string _name;

		public int Id { get; set; }

		public string Name { get => _name; set => Set(ref _name, value); }
	}
}
