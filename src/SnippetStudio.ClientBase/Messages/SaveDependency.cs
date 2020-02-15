using System;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveDependency
	{
		public Guid ParentId { get; }
		public DependencyModel Item { get; }

		public SaveDependency(Guid parentId, DependencyModel item)
		{
			ParentId = parentId;
			Item = item;
		}
	}
}