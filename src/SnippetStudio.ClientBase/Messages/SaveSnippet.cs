using System;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveSnippet
	{
		public SnippetModel Item { get; }

		public SaveSnippet(SnippetModel item)
		{
			Item = item;
		}
	}
}