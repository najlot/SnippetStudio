using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;

namespace NajlotSnippetStudio.ViewModel
{
    public class Dependency
    {
        public string Assembly { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public ObservableCollection<Dependency> Dependencies { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand DeleteCommand { get; set; }

		public Dependency()
		{
			DeleteCommand = new RelayCommand(() =>
			{
				Dependencies.Remove(this);
			}, true);
		}
	}
}
