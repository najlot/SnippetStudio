using System;
using System.Linq;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MySqlSnippetRepository : ISnippetRepository, IDisposable
	{
		private readonly MySqlDbContext _context;

		public MySqlSnippetRepository(MySqlDbContext context)
		{
			_context = context;
		}

		public void Delete(Guid id)
		{
			var model = _context.Snippets.FirstOrDefault(i => i.Id == id);

			if (model != null)
			{
				_context.Snippets.Remove(model);
				_context.SaveChanges();
			}
		}

		public SnippetModel Get(Guid id)
		{
			var e = _context.Snippets.FirstOrDefault(i => i.Id == id);

			if (e == null)
			{
				return null;
			}


			return e;
		}

		public void Insert(SnippetModel model)
		{

			foreach (var entry in model.Dependencies)
			{
				entry.Id = 0;
			}


			foreach (var entry in model.Variables)
			{
				entry.Id = 0;
			}

			_context.Snippets.Add(model);
			_context.SaveChanges();
		}

		public void Update(SnippetModel model)
		{
			_context.Snippets.Update(model);
			_context.SaveChanges();
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
					_context.Dispose();
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