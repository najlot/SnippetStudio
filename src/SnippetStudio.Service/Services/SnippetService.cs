using System;
using System.Collections.Generic;
using System.Linq;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;

namespace SnippetStudio.Service.Services
{
	public class SnippetService : IDisposable
	{
		private readonly ISnippetRepository _snippetRepository;
		private readonly ILanguageRepository _languageRepository;
		private readonly ISnippetQuery _snippetQuery;

		public SnippetService(ISnippetRepository snippetRepository,
			ILanguageRepository languageRepository,
			ISnippetQuery snippetQuery)
		{
			_snippetRepository = snippetRepository;
			_languageRepository = languageRepository;
			_snippetQuery = snippetQuery;
		}

		public void CreateSnippet(CreateSnippet command, string userName)
		{
			var item = new SnippetModel()
			{
				Id = command.Id,
				Name = command.Name,
				Language = _languageRepository.Get(command.LanguageId),
				Dependencies = command.Dependencies,
				Variables = command.Variables,
				Template = command.Template,
				Code = command.Code,
			};

			_snippetRepository.Insert(item);
		}

		public void UpdateSnippet(UpdateSnippet command, string userName)
		{
			var item = _snippetRepository.Get(command.Id);
			
			item.Name = command.Name;
			item.Language = _languageRepository.Get(command.LanguageId);
			item.Template = command.Template;
			item.Code = command.Code;

			while (item.Dependencies.Count > command.Dependencies.Count)
			{
				item.Dependencies.RemoveAt(item.Dependencies.Count - 1);
			}

			while (item.Dependencies.Count < command.Dependencies.Count)
			{
				item.Dependencies.Add(new Dependency());
			}

			for (int i = 0; i < item.Dependencies.Count; i++)
			{
				item.Dependencies[i].Name = command.Dependencies[i].Name;
			}

			while (item.Variables.Count > command.Variables.Count)
			{
				item.Variables.RemoveAt(item.Variables.Count - 1);
			}

			while (item.Variables.Count < command.Variables.Count)
			{
				item.Variables.Add(new Variable());
			}

			for (int i = 0; i < item.Variables.Count; i++)
			{
				item.Variables[i].Name = command.Variables[i].Name;
				item.Variables[i].RequestName = command.Variables[i].RequestName;
				item.Variables[i].DefaultValue = command.Variables[i].DefaultValue;
			}

			_snippetRepository.Update(item);
		}

		public void DeleteSnippet(Guid id, string userName)
		{
			_snippetRepository.Delete(id);
		}

		public Snippet GetItem(Guid id)
		{
			return _snippetQuery.Get(id);
		}

		public List<Snippet> GetItemsForUser(string userName)
		{
			IEnumerable<Snippet> items = _snippetQuery.GetAll();

			return items.ToList();
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					(_snippetRepository as IDisposable)?.Dispose();
					(_languageRepository as IDisposable)?.Dispose();
					(_snippetQuery as IDisposable)?.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}