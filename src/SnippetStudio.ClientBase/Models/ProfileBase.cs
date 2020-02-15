using System;

namespace SnippetStudio.ClientBase.Models
{
	public abstract class ProfileBase
	{
		public Guid Id { get; set; }
		public Source Source { get; set; }
		public string Name { get; set; }

		public ProfileBase Clone()
		{
			return this.MemberwiseClone() as ProfileBase;
		}
	}
}