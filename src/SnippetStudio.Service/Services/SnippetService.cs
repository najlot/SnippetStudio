using Cosei.Service.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Model;
using SnippetStudio.Service.Query;
using SnippetStudio.Service.Repository;

namespace SnippetStudio.Service.Services
{
	public class SnippetService : IDisposable
	{
		private readonly ISnippetRepository _snippetRepository;
		private readonly ISnippetQuery _snippetQuery;
		private readonly IPublisher _publisher;

		public SnippetService(ISnippetRepository snippetRepository,
			ISnippetQuery snippetQuery,
			IPublisher publisher)
		{
			_snippetRepository = snippetRepository;
			_snippetQuery = snippetQuery;
			_publisher = publisher;
		}

		public void CreateSnippet(CreateSnippet command, string userName)
		{
			var item = new SnippetModel()
			{
				Id = command.Id,
				Name = command.Name,
				Language = command.Language,
				Variables = command.Variables,
				Template = command.Template,
				Code = command.Code,
			};

			_snippetRepository.Insert(item);

			_publisher.PublishAsync(new SnippetCreated(
				command.Id,
				command.Name,
				command.Language,
				command.Variables,
				command.Template,
				command.Code));
		}

		public void UpdateSnippet(UpdateSnippet command, string userName)
		{
			var item = _snippetRepository.Get(command.Id);
			
			item.Name = command.Name;
			item.Language = command.Language;
			item.Template = command.Template;
			item.Code = command.Code;

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

			_publisher.PublishAsync(new SnippetUpdated(
				command.Id,
				command.Name,
				command.Language,
				command.Variables,
				command.Template,
				command.Code));
		}

		public void DeleteSnippet(Guid id, string userName)
		{
			_snippetRepository.Delete(id);

			_publisher.PublishAsync(new SnippetDeleted(id));
		}

		public async Task<Snippet> GetItemAsync(Guid id)
		{
			var item = await _snippetQuery.GetAsync(id);

			if (item == null)
			{
				return null;
			}

			return new Snippet
			{
				Id = item.Id,
				Name = item.Name,
				Language = item.Language,
				Variables = item.Variables,
				Template = item.Template,
				Code = item.Code,
			};
		}

		public async IAsyncEnumerable<Snippet> GetItemsForUserAsync(string userName)
		{
			await foreach (var item in _snippetQuery.GetAllAsync())
			{
				yield return new Snippet
				{
					Id = item.Id,
					Name = item.Name,
					Language = item.Language,
					Variables = item.Variables,
					Template = item.Template,
					Code = item.Code,
				};
			}
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
					(_snippetQuery as IDisposable)?.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}