using System;

namespace SnippetStudio.ClientBase.Messages
{
	public class EditVariable
	{
		public Guid ParentId { get; }
		public int Id { get; }

		public EditVariable(Guid parentId, int id)
		{
			ParentId = parentId;
			Id = id;
		}
	}
}