using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class LoadSnippet
	{
		public Guid Id { get; }

		public LoadSnippet(Guid id)
		{
			Id = id;
		}
	}
}