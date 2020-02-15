using System;
using System.Collections.Generic;
using SnippetStudio.ClientBase.Validation;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Models
{
	public class VariableModel : AbstractValidationModel, IVariable
	{
		private string _name;
		private string _requestName;
		private string _defaultValue;

		public int Id { get; set; }

		public string Name { get => _name; set => Set(ref _name, value); }
		public string RequestName { get => _requestName; set => Set(ref _requestName, value); }
		public string DefaultValue { get => _defaultValue; set => Set(ref _defaultValue, value); }
	}
}
