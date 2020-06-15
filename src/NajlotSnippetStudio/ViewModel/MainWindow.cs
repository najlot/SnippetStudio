using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;

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
				VisibleTemplates.Refresh();
			}
        }

		public System.ComponentModel.ICollectionView VisibleTemplates
		{
			get
			{
				var templates = System.Windows.Data.CollectionViewSource.GetDefaultView(Templates);
				templates.Filter = (template) =>
				{
					var templateEntry = template as Template;
					return !templateEntry.MarkedForDeletion;
				};
				
				return templates;
			}
		}

		public bool NameIsUnique(string value)
		{
			return Templates.Where(tpl => !tpl.MarkedForDeletion && tpl.Name == value).Count() == 0;
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

				if (_currentTemplate != null)
				{
					_currentTemplate.IsCurrentSelectionVisibility = Visibility.Collapsed;
				}
				
				if (value == null)
				{
					_currentTemplate = new Template()
					{
						IsEnabled = false
					};
				}
				
				Set(nameof(CurrentTemplate), ref _currentTemplate, value);

				_currentTemplate.IsCurrentSelectionVisibility = Visibility.Visible;
			}
        }

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand AddTemplateCommand { get => new RelayCommand(() =>
			{
				var newName = "";

				for (int i = 1; i < int.MaxValue; i++)
				{
					newName = "Snippet_" + i;
					if(NameIsUnique(newName))
					{
						break;
					}
				}
				
				var newTpl = new Template()
				{
					Name = newName
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

			});
		}

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand DeleteTemplateCommand { get => new RelayCommand(() =>
			{
				if (CurrentTemplate == null)
				{
					return;
				}

				Template newSelection = null;

				foreach (var item in VisibleTemplates)
				{
					var visibleTemplate = item as Template;

					if (visibleTemplate == CurrentTemplate)
					{
						if (newSelection != null)
						{
							break;
						}
						else
						{
							continue;
						}
					}

					newSelection = visibleTemplate;
				}

				CurrentTemplate.MarkedForDeletion = true;
				VisibleTemplates.Refresh();

				CurrentTemplate = newSelection;
			});
		}

        public MainWindow()
        {
			
        }
    }
}
