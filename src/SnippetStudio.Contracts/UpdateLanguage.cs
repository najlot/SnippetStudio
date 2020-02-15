using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class UpdateLanguage
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		private UpdateLanguage(){}

		public UpdateLanguage(
			Guid id,
			string name)
		{
			Id = id;
			Name = name;
		}
	}
}
