using System;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Messages
{
	public class SaveUser
	{
		public UserModel Item { get; }

		public SaveUser(UserModel item)
		{
			Item = item;
		}
	}
}