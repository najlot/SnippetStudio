using SnippetStudio.ClientBase.Models;
using System.Collections.Generic;

namespace SnippetStudio.ClientBase.ViewModel
{
	public class TextInputViewModel : AbstractPopupViewModel<bool>
	{
		public IEnumerable<VariableModel> Variables { get; }

		public RelayCommand OkCommand { get; }
		public RelayCommand CancelCommand { get; }
		
		public TextInputViewModel(IEnumerable<VariableModel> variables)
		{
			Variables = variables;

			OkCommand = new RelayCommand(() => SetResult(true));
			CancelCommand = new RelayCommand(() => SetResult(false));
		}
	}
}