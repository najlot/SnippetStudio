using System.Collections.Generic;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class LanguageInputViewModel : AbstractPopupViewModel<string>
	{
		public IEnumerable<string> Languages { get; } = new[] { "C#", "Python", "NodeJS" };
		public string SelectedLanguage { get; set; } = "C#";

		public RelayCommand OkCommand { get; }
		public RelayCommand CancelCommand { get; }

		public LanguageInputViewModel()
		{
			OkCommand = new RelayCommand(() => SetResult(SelectedLanguage));
			CancelCommand = new RelayCommand(() => SetResult(null));
		}
	}
}