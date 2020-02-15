using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class DeleteDependency
	{
		public Guid ParentId { get; }
		public int Id { get; }

		public DeleteDependency(Guid parentId, int id)
		{
			ParentId = parentId;
			Id = id;
		}
	}
}