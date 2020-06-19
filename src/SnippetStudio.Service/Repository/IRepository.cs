namespace SnippetStudio.Service.Repository
{
	public interface IRepository<in TKey, TModel>
		where TModel : class, new()
	{
		TModel Get(TKey id);

		void Insert(TModel model);

		void Update(TModel model);

		void Delete(TKey id);
	}
}