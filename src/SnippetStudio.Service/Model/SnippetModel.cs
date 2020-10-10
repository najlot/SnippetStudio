using MongoDB.Bson.Serialization.Attributes;
using SnippetStudio.Contracts;
using System;
using System.Collections.Generic;

namespace SnippetStudio.Service.Model
{
	[BsonIgnoreExtraElements]
	public class SnippetModel
	{
		[BsonId]
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public List<Variable> Variables { get; set; }
		public string Template { get; set; }
		public string Code { get; set; }
		public string CreatedBy { get; set; }
	}
}
