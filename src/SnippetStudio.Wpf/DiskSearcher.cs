using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnippetStudio.ClientBase;

namespace SnippetStudio.Wpf
{
	public class DiskSearcher : IDiskSearcher
	{
		private string _initialDirectory = "C:\\";
		private string _initialSaveDirectory = "C:\\";

		public DiskSearcher()
		{
			var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			_initialDirectory = Path.Combine(Path.GetDirectoryName(docs), "Downloads");
			_initialSaveDirectory = _initialDirectory;
		}

		public Task<string> SelectFileAsync(string filter)
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog
			{
				InitialDirectory = _initialDirectory,
				Filter = filter,
				FilterIndex = 1,
				RestoreDirectory = true
			};

			if (openFileDialog.ShowDialog() ?? false)
			{
				return Task.FromResult(openFileDialog.FileName);
			}

			return Task.FromResult("");
		}

		public Task<string> SelectFolderAsync()
		{
			var dir = Directory.GetCurrentDirectory();

			try
			{
				using var openFileDialog = new FolderBrowserDialog();

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					return Task.FromResult(openFileDialog.SelectedPath);
				}

				return Task.FromResult("");
			}
			finally
			{
				Directory.SetCurrentDirectory(dir);
			}
		}

		public Task<string> SelectSaveFileAsync(string filename, string filter, string defaultExt)
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();

			var fileName = new string(filename.Where(c => !invalidChars.Contains(c)).ToArray());

			var openFileDialog = new Microsoft.Win32.SaveFileDialog
			{
				InitialDirectory = _initialSaveDirectory,
				DefaultExt = defaultExt,
				FileName = fileName,
				Filter = filter,
				FilterIndex = 1,
				RestoreDirectory = true
			};

			if (openFileDialog.ShowDialog() ?? false)
			{
				_initialSaveDirectory = Path.GetDirectoryName(openFileDialog.FileName);
				return Task.FromResult(openFileDialog.FileName);
			}

			return Task.FromResult("");
		}
	}
}