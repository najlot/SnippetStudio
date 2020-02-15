using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class DeleteLanguage
	{
		public Guid Id { get; }

		public DeleteLanguage(Guid id)
		{
			Id = id;
		}
	}
}