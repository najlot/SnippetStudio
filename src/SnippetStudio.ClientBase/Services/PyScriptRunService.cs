using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System;
using Python.Runtime;

namespace SnippetStudio.ClientBase.Services
{
	public class PyScriptRunService
	{
		private readonly IClipboardService _clipboardService;

		private static bool _pyReady = false;
		public static async Task PyInitAsync()
		{
			if (_pyReady)
			{
				return;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// On Windows, to simplify setup: download the python nuget-package and extract in to the local directory
				var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				if (!File.Exists(Path.Combine(assemblyPath, "python37.dll")))
				{
					using (var client = new HttpClient() { BaseAddress = new Uri("https://www.nuget.org") })
					{
						var response = await client.GetStreamAsync("/api/v2/package/python/3.7.6");
						using (var memStr = new MemoryStream())
						{
							await response.CopyToAsync(memStr);
							using (var archive = new System.IO.Compression.ZipArchive(memStr))
							{
								foreach (var entry in archive.Entries)
								{
									if (entry.FullName.StartsWith("tools"))
									{
										var path = Path.Combine(assemblyPath, entry.FullName.Substring(6));
										Directory.CreateDirectory(Path.GetDirectoryName(path));

										using (var str = entry.Open())
										{
											using (var f = File.OpenWrite(path))
											{
												str.CopyTo(f);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			PythonEngine.Initialize();

			_pyReady = true;
		}

		public PyScriptRunService(IClipboardService clipboardService)
		{
			_clipboardService = clipboardService;
		}

		public string Run(string code, string template, Dictionary<string, string> variables)
		{
			using (Py.GIL())
			{
				using (var scope = Py.CreateScope())
				{
					scope.Set("result", "");
					scope.Set("template", template);

					using (var pyVariables = new PyDict())
					{
						foreach (var variable in variables)
						{
							pyVariables[variable.Key] = variable.Value.ToPython();
						}

						scope.Set("variables", pyVariables);

						scope.Set("get_clipboard_text", (Func<string>)(() => _clipboardService.GetText()));
						scope.Set("set_clipboard_text", (Action<string>)((text) => _clipboardService.SetText(text)));

						scope.Exec(code);

						return scope.Get<string>("result");
					}
				}
			}
		}
	}
}