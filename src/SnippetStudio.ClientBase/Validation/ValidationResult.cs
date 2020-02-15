﻿namespace SnippetStudio.ClientBase.Validation
{
	public class ValidationResult
	{
		public ValidationSeverity Severity { get; }
		public string PropertyName { get; }
		public string Text { get; }

		public ValidationResult(ValidationSeverity severity,
			string propertyName, string text)
		{
			Severity = severity;
			PropertyName = propertyName;
			Text = text;
		}
	}
}
