using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajlotSnippetStudio.ViewModel
{
	/// <summary>
	/// 
	/// </summary>
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
