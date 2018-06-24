
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommonServiceLocator;

namespace NajlotSnippetStudio.ViewModel
{
	public class Template : ViewModelBase
	{
		public string Name { get => _name;
			set
			{
				value = value.Replace("\\", "").Replace("/", "").Replace(":", "").Trim();

				if (value == _name) return;

				if (NameIsValid(value))
				{
					if(NameIsUnique(value))
					{
						Set(nameof(Name), ref _name, value);
					}
					else
					{
						NotifyAboutErrorForShortTime( $"Template {value} already exists!");
					}
				}
				else
				{
					NotifyAboutErrorForShortTime(value + " in an invalid name!");
				}
			}
		}

		public void NotifyAboutErrorForShortTime(string errorText)
		{
			Task.Run(() =>
			{
				LogString = errorText;

				Thread.Sleep(3000);

				if(LogString == errorText)
				{
					LogString = "";
				}
			});
		}

		private bool NameIsUnique(string value)
		{
			var mainWindow = ServiceLocator.Current.GetInstance<ViewModel.MainWindow>();
			return mainWindow.NameIsUnique(value);
		}

		public bool MarkedForDeletion { get; set; } = false;

		private bool NameIsValid(string value)
		{
			try
			{
				var tempPath = Path.GetTempPath();
				var fileName = Path.Combine(tempPath, value);
				File.WriteAllText(fileName, "");
				File.Delete(fileName);
			}
			catch
			{
				return false;
			}

			return true;
		}

		// TODO implement (saving only changed)
		public bool IsChanged { get; set; } = true;

		public string OriginalName { get; set; }
		
		public string TemplateString { get; set; } = "";
		
		private string _code = Resources.Resource.InitialCodeCS;
		public string Code { get { return _code; } set { Set(nameof(Code), ref _code, value); } }

		private string _codeLanguage = "C#";
		public string CodeLanguage
		{
			get
			{
				return _codeLanguage;
			}
			set
			{

				if (value == _codeLanguage)
				{
					return;
				}

				if (Code == "" || (Code == Resources.Resource.InitialCodeCS && _codeLanguage == "C#"))
				{
					Code = Resources.Resource.InitialCodeVB;
				}
				else if (Code == "" || (Code == Resources.Resource.InitialCodeVB && _codeLanguage == "VB"))
				{
					Code = Resources.Resource.InitialCodeCS;
				}

				Set(nameof(CodeLanguage), ref _codeLanguage, value);
			}
		}

		[XmlIgnore]
		public List<string> PossibleLanguages { get; set; } = new List<string>();

		[XmlArray("Variables")]
		public ObservableCollection<Variable> Variables { get; set; } = new ObservableCollection<Variable>();

		[XmlArray("Dependencies")]
		public ObservableCollection<Dependency> Dependencies { get; set; } = new ObservableCollection<Dependency>();
		public bool IsEnabled { get; set; } = true;

		[XmlIgnore]
		public RelayCommand RunCommand { get => new RelayCommand(() =>
			{
				var output = new StringCollection();
				LogString = "";

				try
				{
					TemplateRunner.Run(this, ref output);
					foreach (var line in output) LogString += line + "\r\n";
				}
				catch (Exception ex)
				{
					var e = ex;

					while (e != null)
					{
						var arr = e.ToString().Replace("\r\n", "\n").Split('\n');

						foreach (var msg in arr)
						{
							LogString += msg + "\r\n";
						}

						e = e.InnerException;
					}
				}

			});
		}

		[XmlIgnore]
		public RelayCommand AddDependencyCommand { get => new RelayCommand(() =>
			{
				Dependencies.Add(new Dependency()
				{
					Dependencies = this.Dependencies
				});
			});
		}

		[XmlIgnore]
		public RelayCommand AddVariableCommand { get => new RelayCommand(() =>
			{
				Variables.Add(new Variable()
				{
					Variables = this.Variables
				});
			});
		}

		private string _logString = "";
		/// <summary>
		/// Sets and gets the LogString property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string LogString
		{
			get
			{
				return _logString;
			}
			set
			{
				Set(nameof(LogString), ref _logString, value);
				HasErrors = _logString.Length > 0;
			}
		}

		private bool _hasErrors = false;
		private string _name;

		public bool HasErrors
		{
			get
			{
				return _hasErrors;
			}
			set
			{
				Set(nameof(HasErrors), ref _hasErrors, value);
			}
		}

		public Template()
		{
			PossibleLanguages.Clear();
			PossibleLanguages.Add("C#");
			PossibleLanguages.Add("VB");
			
		}
	}
}
