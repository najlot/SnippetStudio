using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Collections.Generic;

namespace SnippetStudio.ClientBase.Services
{
	public class CsScriptRunService
	{
		private InteractiveAssemblyLoader loader;
		private ScriptOptions options;
		private readonly IClipboardService _clipboardService;

		public CsScriptRunService(IClipboardService clipboardService)
		{
			_clipboardService = clipboardService;
		}

		private readonly System.Reflection.Assembly[] references = new[]
		{
			typeof(object).Assembly,
			typeof(System.IO.FileInfo).Assembly,
			typeof(System.Linq.IQueryable).Assembly,
			typeof(System.Dynamic.DynamicObject).Assembly,
			typeof(System.Text.RegularExpressions.Regex).Assembly
		};

		private InteractiveAssemblyLoader GetLoader()
		{
			if (loader == null)
			{
				loader = new InteractiveAssemblyLoader();

				foreach (var reference in references)
				{
					loader.RegisterDependency(reference);
				}
			}

			return loader;
		}

		private ScriptOptions GetOptions()
		{
			if (options == null)
			{
				options = ScriptOptions.Default
					.WithReferences(references)
					.AddImports(
						"System",
						"System.IO",
						"System.Linq",
						"System.Text",
						"System.Dynamic",
						"System.Collections.Generic",
						"System.Text.RegularExpressions"
						);
			}

			return options;
		}

		public async Task<string> Run(string code, string template, Dictionary<string, string> variables)
		{
			var globals = new Globals(_clipboardService)
			{
				Template = template,
				Variables = variables
			};

			var run = CSharpScript
					.Create(code, GetOptions(), typeof(Globals), GetLoader())
					.CreateDelegate();

			var result = await run(globals);

			return result.ToString();
		}
	}
}