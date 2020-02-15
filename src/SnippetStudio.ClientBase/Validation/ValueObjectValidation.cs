using System.Collections.Generic;

namespace SnippetStudio.ClientBase.Validation
{
	public class ValueObjectValidation<T> : ValidationBase<T>
	{
		public override IEnumerable<ValidationResult> Validate(T o)
		{
			foreach (var property in typeof(T).GetProperties())
			{
				if (property.GetValue(o) is IValueObject value)
				{
					foreach (var entry in value.Validate())
					{
						yield return entry;
					}
				}
			}
		}
	}
}
