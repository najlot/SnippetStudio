using System;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveLanguage
	{
		public LanguageModel Item { get; }

		public SaveLanguage(LanguageModel item)
		{
			Item = item;
		}
	}
}