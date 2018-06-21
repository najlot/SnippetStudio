
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace NajlotSnippetStudio.ViewModel
{
	public class Template : ViewModelBase
	{
		// TODO notify the user
		public string Name { get => _name;
			set
			{
				if(string.Compare(value, "NajlotSnippetStudio") == 0)
				{
					return; // name reserved
				}

				value = value.Replace("\\", "").Replace("/", "").Replace(":", "").Trim();

				if (NameIsValid(value))
				{
					if(NameIsUnique(value))
					{
						Set(nameof(Name), ref _name, value);
					}
				}
			}
		}

		private bool NameIsUnique(string value)
		{
			// TODO implement
			return true;
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

		// TODO implement
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

				if (Code == "" || (Code == Resources.Resource.InitialCodeVB && _codeLanguage == "VB"))
				{
					Code = Resources.Resource.InitialCodeCS;
				}

				Set(nameof(CodeLanguage), ref _codeLanguage, value);
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public List<string> PossibleLanguages { get; set; } = new List<string>();

		public ObservableCollection<Variable> Variables { get; set; } = new ObservableCollection<Variable>();
		public ObservableCollection<Dependency> Dependencies { get; set; } = new ObservableCollection<Dependency>();
		public bool IsEnabled { get; set; } = true;

		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand RunCommand { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand AddDependencyCommand { get; set; }
		[System.Xml.Serialization.XmlIgnore]
		public RelayCommand AddVariableCommand { get; set; }

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

			RunCommand = new RelayCommand(() =>
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

				HasErrors = LogString.Length > 0;
			}, true);

			AddDependencyCommand = new RelayCommand(() =>
			{
				Dependencies.Add(new Dependency()
				{
					Dependencies = this.Dependencies
				});
			}, true);

			AddVariableCommand = new RelayCommand(() =>
			{
				Variables.Add(new Variable()
				{
					Variables = this.Variables
				});
			}, true);
		}

	}
}
