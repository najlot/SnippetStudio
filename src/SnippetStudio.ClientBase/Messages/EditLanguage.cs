using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class EditLanguage
	{
		public Guid Id { get; }

		public EditLanguage(Guid id)
		{
			Id = id;
		}
	}
}