namespace SnippetStudio.ClientBase.ViewModel
{
	public class TextInputViewModel : AbstractPopupViewModel<(bool shouldCancel, string input)>
	{
		public string Description { get; set; }
		public string Input { get; set; }

		public RelayCommand OkCommand { get; }
		public RelayCommand CancelCommand { get; }

		public TextInputViewModel()
		{
			OkCommand = new RelayCommand(() => SetResult((shouldCancel: false, input: Input)));
			CancelCommand = new RelayCommand(() => SetResult((shouldCancel: true, input: Input)));
		}
	}
}