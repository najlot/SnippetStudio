using System;
using System.Threading.Tasks;

namespace SnippetStudio.ClientBase.Services
{
	public interface IErrorService
	{
		Task ShowAlertAsync(Exception ex);
		Task ShowAlertAsync(string message, Exception ex);
		Task ShowAlertAsync(string title, string message);
	}
}