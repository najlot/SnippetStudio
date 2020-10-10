using System;
using System.Linq;
using SnippetStudio.Service.Model;

namespace SnippetStudio.Service.Repository
{
	public class MySqlUserRepository : IUserRepository, IDisposable
	{
		private readonly MySqlDbContext _context;

		public MySqlUserRepository(MySqlDbContext context)
		{
			_context = context;
		}

		public void Delete(Guid id)
		{
			var model = _context.Users.FirstOrDefault(i => i.Id == id);

			if (model != null)
			{
				_context.Users.Remove(model);
				_context.SaveChanges();
			}
		}

		public UserModel Get(Guid id)
		{
			var e = _context.Users.FirstOrDefault(i => i.Id == id);

			if (e == null)
			{
				return null;
			}


			return e;
		}

		public UserModel Get(string username)
		{
			var e = _context.Users.FirstOrDefault(i => i.IsActive && i.Username == username);

			if (e == null)
			{
				return null;
			}

			return e;
		}

		public void Insert(UserModel model)
		{

			_context.Users.Add(model);
			_context.SaveChanges();
		}

		public void Update(UserModel model)
		{
			_context.Users.Update(model);
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
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}