using System;
using System.Linq;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MySqlLanguageRepository : ILanguageRepository, IDisposable
	{
		private readonly MySqlDbContext _context;

		public MySqlLanguageRepository(MySqlDbContext context)
		{
			_context = context;
		}

		public void Delete(Guid id)
		{
			var model = _context.Languages.FirstOrDefault(i => i.Id == id);

			if (model != null)
			{
				_context.Languages.Remove(model);
				_context.SaveChanges();
			}
		}

		public LanguageModel Get(Guid id)
		{
			var e = _context.Languages.FirstOrDefault(i => i.Id == id);

			if (e == null)
			{
				return null;
			}


			return e;
		}

		public void Insert(LanguageModel model)
		{

			_context.Languages.Add(model);
			_context.SaveChanges();
		}

		public void Update(LanguageModel model)
		{
			_context.Languages.Update(model);
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