using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SnippetStudio.Service.Query
{
	public interface IQuery<TKey, TModel>
		where TModel : class, new()
	{
		TModel Get(TKey id);

		IEnumerable<TModel> GetAll();

		IEnumerable<TModel> GetAll(Expression<Func<TModel, bool>> predicate);
	}
}
