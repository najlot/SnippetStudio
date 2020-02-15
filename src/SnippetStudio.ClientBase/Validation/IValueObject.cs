using System.Collections.Generic;

namespace SnippetStudio.ClientBase.Validation
{
	public interface IValueObject { IEnumerable<ValidationResult> Validate(); }
}
