using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;

namespace NajlotSnippetStudio.ViewModel
{
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

				CurrentTemplate = newTpl;

			}, true);

            DeleteTemplateCommand = new RelayCommand(() =>
            {
                Templates.Remove(CurrentTemplate);
                if(Templates.Count > 0)
                {
					CurrentTemplate = Templates[0];
				}
                else
                {
					CurrentTemplate = null;
				}
            }, true);
        }

    }
}
