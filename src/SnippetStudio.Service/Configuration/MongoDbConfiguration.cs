﻿namespace SnippetStudio.Service.Configuration
{
	public class MongoDbConfiguration
	{
		public string Host { get; set; } = "";
		public int Port { get; set; } = 27017;
		public string Database { get; set; } = "";
		public string User { get; set; } = "";
		public string Password { get; set; } = "";
	}
}