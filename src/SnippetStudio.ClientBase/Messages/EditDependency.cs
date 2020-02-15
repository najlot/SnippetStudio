using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class EditDependency
	{
		public Guid ParentId { get; }
		public int Id { get; }

		public EditDependency(Guid parentId, int id)
		{
			ParentId = parentId;
			Id = id;
		}
	}
}