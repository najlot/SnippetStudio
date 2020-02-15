using MongoDB.Bson.Serialization.Attributes;
using SnippetStudio.Contracts;
using System;
using System.Collections.Generic;

namespace SnippetStudio.Service.Model
{
	[BsonIgnoreExtraElements]
	public class LanguageModel
	{
		[BsonId]
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
