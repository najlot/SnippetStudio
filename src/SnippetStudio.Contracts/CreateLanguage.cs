using System;
using System.Collections.Generic;

namespace SnippetStudio.Contracts
{
	public class CreateLanguage
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		private CreateLanguage(){}

		public CreateLanguage(
			Guid id,
			string name)
		{
			Id = id;
			Name = name;
		}
	}
}
