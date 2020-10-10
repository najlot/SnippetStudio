using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Collections.Generic;
using System.Linq;

namespace SnippetStudio.ClientBase.Services
{
	public class CsScriptRunService
	{
		private readonly InteractiveAssemblyLoader _loader;
		private readonly ScriptOptions _options;
		private readonly IClipboardService _clipboardService;

		public CsScriptRunService(IClipboardService clipboardService)
		{
			_clipboardService = clipboardService;

			var types = new[]
			{
				typeof(object),
				typeof(System.IO.FileInfo),
				typeof(System.Linq.IQueryable),
				typeof(System.Threading.Tasks.Task),
				typeof(System.Dynamic.DynamicObject),
				typeof(System.Collections.Generic.List<>),
				typeof(System.Text.RegularExpressions.Regex),
				typeof(System.Collections.Concurrent.ConcurrentDictionary<,>),
				typeof(Newtonsoft.Json.JsonConvert),
			};

			var references = types.Select(type => type.Assembly).ToArray();

			_loader = new InteractiveAssemblyLoader();
			foreach (var reference in references)
			{
				_loader.RegisterDependency(reference);
			}

			_options = ScriptOptions.Default
				.WithReferences(references)
				.AddImports(types.Select(type => type.Namespace).ToArray());
		}

		public async Task<string> Run(string code, string template, Dictionary<string, string> variables)
		{
			var globals = new Globals(_clipboardService)
			{
				Template = template,
				Variables = variables
			};

			var run = CSharpScript
					.Create(code, _options, typeof(Globals), _loader)
					.CreateDelegate();

			var result = await run(globals);

			return result?.ToString();
		}
	}
}