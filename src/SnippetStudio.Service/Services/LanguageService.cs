using System;
using System.Collections.Generic;
using System.Linq;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;

namespace SnippetStudio.Service.Services
{
	public class LanguageService : IDisposable
	{
		private readonly ILanguageRepository _languageRepository;
		private readonly ILanguageQuery _languageQuery;

		public LanguageService(ILanguageRepository languageRepository,
			ILanguageQuery languageQuery)
		{
			_languageRepository = languageRepository;
			_languageQuery = languageQuery;
		}

		public void CreateLanguage(CreateLanguage command, string userName)
		{
			var item = new LanguageModel()
			{
				Id = command.Id,
				Name = command.Name,
			};

			_languageRepository.Insert(item);
		}

		public void UpdateLanguage(UpdateLanguage command, string userName)
		{
			var item = _languageRepository.Get(command.Id);
			
			item.Name = command.Name;

			_languageRepository.Update(item);
		}

		public void DeleteLanguage(Guid id, string userName)
		{
			_languageRepository.Delete(id);
		}

		public Language GetItem(Guid id)
		{
			return _languageQuery.Get(id);
		}

		public List<Language> GetItemsForUser(string userName)
		{
			IEnumerable<Language> items = _languageQuery.GetAll();

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
					(_languageRepository as IDisposable)?.Dispose();
					(_languageQuery as IDisposable)?.Dispose();
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