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
    public class Variable
    {
        public string Name { get; set; }
        public string RequestName { get; set; }
        public string Default { get; set; }
        public string Value { get; set; }
		public bool Cancel { get; set; } = false;

		[System.Xml.Serialization.XmlIgnore]
		public ObservableCollection<Variable> Variables { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand DeleteCommand { get; set; }

		public Variable()
		{
			DeleteCommand = new RelayCommand(() =>
			{
				Variables.Remove(this);
			}, true);
		}

	}
}
