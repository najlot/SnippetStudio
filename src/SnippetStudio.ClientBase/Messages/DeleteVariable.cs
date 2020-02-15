using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class DeleteVariable
	{
		public Guid ParentId { get; }
		public int Id { get; }

		public DeleteVariable(Guid parentId, int id)
		{
			ParentId = parentId;
			Id = id;
		}
	}
}