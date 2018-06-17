using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajlotSnippetStudio.ViewModel
{
	/// <summary>
	/// 
	/// </summary>
    public class MainWindow : ViewModelBase
    {
        private ObservableCollection<Template> _templates = new ObservableCollection<Template>();
        /// <summary>
        /// Sets and gets the Templates property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Template> Templates
        {
            get
            {
                return _templates;
            }
            set
            {
                if (_templates == value)
                {
                    return;
                }

                Set(nameof(Templates), ref _templates, value);
            }
        }
        
        private Template _currentTemplate = null;
        /// <summary>
        /// Sets and gets the CurrentTemplate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Template CurrentTemplate
        {
            get
            {
                return _currentTemplate;
            }
            set
            {
                if (_currentTemplate == value)
                {
                    return;
                }

                Set(nameof(CurrentTemplate), ref _currentTemplate, value);
            }
        }

        private int _selectedTemplateIndex;
        /// <summary>
        /// Sets and gets the SelectedTemplateIndex property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedTemplateIndex
        {
            get
            {
                return _selectedTemplateIndex;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                if (_selectedTemplateIndex == value)
                {
                    //return;
                }

				if(Templates.Count > value)
				{
					try
					{
						// HACK:
						// Beim Ändern des Templates (und damit den dazugehörigen Quellcode) versucht
						// AvalonEditBehavior das CaretOffset zu behalten.
						// Sollte der Quelltext länger sein, schmiert die Anwendung ab...
						CurrentTemplate = Templates[value];
					}
					catch(Exception ex)
					{
						Console.Error.WriteLine(ex);
					}
				}
                
                Set(nameof(SelectedTemplateIndex), ref _selectedTemplateIndex, value);
            }
        }

		public int SaveID { get; set; } = 0;

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand AddTemplateCommand { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand DeleteTemplateCommand { get; set; }

        public MainWindow()
        {
            AddTemplateCommand = new RelayCommand(() =>
            {
                var newTpl = new Template()
                {
                    Name = "New Template"
                };

				newTpl.Dependencies.Add(new Dependency()
				{
					Assembly = "mscorlib.dll",
					Dependencies = newTpl.Dependencies
				});

				newTpl.Dependencies.Add(new Dependency()
				{
					Assembly = "System.dll",
					Dependencies = newTpl.Dependencies
				});

				newTpl.Dependencies.Add(new Dependency()
				{
					Assembly = "System.Core.dll",
					Dependencies = newTpl.Dependencies
				});

				newTpl.Dependencies.Add(new Dependency()
				{
					Assembly = "System.Data.dll",
					Dependencies = newTpl.Dependencies
				});

				newTpl.Dependencies.Add(new Dependency()
				{
					Assembly = "System.Windows.Forms.dll",
					Dependencies = newTpl.Dependencies
				});

				Templates.Add(newTpl);

                SelectedTemplateIndex = Templates.Count -1;
            }, true);

            DeleteTemplateCommand = new RelayCommand(() =>
            {
                Templates.Remove(CurrentTemplate);
                if(Templates.Count > 0)
                {
                    SelectedTemplateIndex = 0;
                }
                else
                {
                    SelectedTemplateIndex = -1;
                    CurrentTemplate = new Template()
                    {
                        IsEnabled = false
                    };
                }
                
            }, true);
        }

    }
}
