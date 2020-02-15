using System;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveVariable
	{
		public Guid ParentId { get; }
		public VariableModel Item { get; }

		public SaveVariable(Guid parentId, VariableModel item)
		{
			ParentId = parentId;
			Item = item;
		}
	}
}