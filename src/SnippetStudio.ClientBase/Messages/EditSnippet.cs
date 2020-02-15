using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class EditSnippet
	{
		public Guid Id { get; }

		public EditSnippet(Guid id)
		{
			Id = id;
		}
	}
}