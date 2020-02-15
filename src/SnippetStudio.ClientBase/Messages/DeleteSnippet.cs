using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class DeleteSnippet
	{
		public Guid Id { get; }

		public DeleteSnippet(Guid id)
		{
			Id = id;
		}
	}
}