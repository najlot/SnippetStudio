using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class SnippetDeleted
	{
		public Guid Id { get; set; }

		private SnippetDeleted(){}

		public SnippetDeleted(Guid id)
		{
			Id = id;
		}
	}
}
