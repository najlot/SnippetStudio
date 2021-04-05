using System.Threading.Tasks;

namespace SnippetStudio.ClientBase
{
	public interface IDiskSearcher
	{
		Task<string> SelectFolderAsync();
		Task<string> SelectFileAsync(string filter);
		Task<string> SelectSaveFileAsync(string filename, string filter, string defaultExt);
	}
}