using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualBasic;

internal class NotFoundException : Exception
{
    public NotFoundException(string what) : base(what + " not found!"){}
}

namespace NajlotSnippetStudio
{
	public static class TemplateRunner
	{
		public static void Run( ViewModel.Template template, ref StringCollection compileOutput)
		{
			string [] code = { template.Code };

			var parms = new CompilerParameters(){ GenerateInMemory = true, GenerateExecutable = false, IncludeDebugInformation = true };
			foreach( var dependency in template.Dependencies )
			{
				parms.ReferencedAssemblies.Add( dependency.Assembly );
			}

			CodeDomProvider provider;

			if(template.CodeLanguage == "VB")
			{
				provider = new VBCodeProvider();
			}
			else
			{
				provider = new CSharpCodeProvider();
			}
			
			var results = provider.CompileAssemblyFromSource(parms, code);

            compileOutput = results.Output;

            if (results.Errors.Count > 0) return;

			var asm = results.CompiledAssembly;
			var cls = asm.CreateInstance("MainClass");

            if(cls == null) throw new NotFoundException("MainClass (class)");

            if (template.TemplateString != null)
            {
                var templateProp = cls.GetType().GetProperty("Template");
                if (templateProp == null)
				{
					if(template.TemplateString.Length > 0)
					{
						throw new NotFoundException("MainClass.Template (property)");
					}
				}

				templateProp.SetValue(cls, template.TemplateString);
			}
            
			var method = cls.GetType().GetMethod("Main");
            if (method == null) throw new NotFoundException("MainClass.Main (function)");

			Dictionary<string, string> filledVariables = new Dictionary<string, string>();

			var variablesProp = cls.GetType().GetProperty("Variables");
			
			if (template.Variables.Count > 0)
			{
				if (variablesProp == null) throw new NotFoundException("MainClass.Variables (property)");
			}

			foreach(var variable in template.Variables)
			{
				VariableRequest vr = new VariableRequest();
				variable.Value = variable.Default;
				vr.DataContext = variable;
				variable.Cancel = false;
				
				vr.ShowDialog();

				filledVariables.Add(variable.Name, variable.Value);

				if (variable.Cancel) return;
			}

			if (variablesProp != null) variablesProp.SetValue(cls, filledVariables);
			
			method.Invoke(cls, null);
		}
	}
}
