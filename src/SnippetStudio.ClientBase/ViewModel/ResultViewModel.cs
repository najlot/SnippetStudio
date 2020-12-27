namespace SnippetStudio.ClientBase.ViewModel
{
	public class ResultViewModel : AbstractPopupViewModel<bool>
	{
		public string Result { get; set; }

		public RelayCommand OkCommand { get; }
		public RelayCommand CancelCommand { get; }

		public ResultViewModel()
		{
			OkCommand = new RelayCommand(() => SetResult(true));
		}
	}
}