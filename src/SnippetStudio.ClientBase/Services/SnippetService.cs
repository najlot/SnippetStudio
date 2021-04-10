using Cosei.Client.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class SnippetService : IDisposable
	{
		private IDataStore<SnippetModel> _store;
		private readonly Messenger _messenger;
		private readonly CsScriptRunService _csScriptRunService;
		private readonly PyScriptRunService _pyScriptRunService;
		private readonly IDispatcherHelper _dispatcher;
		private readonly ISubscriber _subscriber;
		private readonly string _myName;

		public SnippetService(
			IDataStore<SnippetModel> dataStore,
			Messenger messenger,
			CsScriptRunService csScriptRunService,
			PyScriptRunService pyScriptRunService,
			IDispatcherHelper dispatcher,
			ISubscriber subscriber,
			string myName)
		{
			_store = dataStore;
			_messenger = messenger;
			_csScriptRunService = csScriptRunService;
			_pyScriptRunService = pyScriptRunService;
			_dispatcher = dispatcher;
			_subscriber = subscriber;
			_myName = myName;

			subscriber.Register<SnippetCreated>(Handle);
			subscriber.Register<SnippetUpdated>(Handle);
			subscriber.Register<SnippetDeleted>(Handle);
		}

		private async Task Handle(SnippetCreated message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		private async Task Handle(SnippetUpdated message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		private async Task Handle(SnippetDeleted message)
		{
			await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
		}

		public SnippetModel CreateSnippet(string language)
		{
			string code = "";

			switch (language)
			{
				case "C#":
					code = @"
var result = Template;

foreach(var variable in Variables)
{
	result = result.Replace(""%"" + variable.Key + ""%"", variable.Value);
}

return result;";
					break;

				case "Python":
					code = @"
result = template

for key in variables:
	result = result.replace(""%"" + key + ""%"", variables[key])";
					break;
			}

			return new SnippetModel()
			{
				Id = Guid.NewGuid(),
				Name = "",
				Language = language,
				Template = "",
				Code = code,
				Variables = new List<Variable>()
			};
		}

		public async Task<string> Run(string language, string code, string template, Dictionary<string, string> variables)
		{
			switch (language)
			{
				case "C#":
					return await Task.Run(async () => await _csScriptRunService.Run(code, template, variables));

				case "Python":
					await PyScriptRunService.PyInitAsync();
					return _pyScriptRunService.Run(code, template, variables);
			}

			throw new NotImplementedException($"Language '{language}' not implemented!");
		}

		public string GetMyName() => _myName;

		public async Task<bool> AddItemAsync(SnippetModel item)
		{
			return await _store.AddItemAsync(item);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			return await _store.DeleteItemAsync(id);
		}

		public async Task<SnippetModel> GetItemAsync(Guid id)
		{
			return await _store.GetItemAsync(id);
		}

		public async Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return await _store.GetItemsAsync(forceRefresh);
		}

		public async Task<bool> UpdateItemAsync(SnippetModel item)
		{
			return await _store.UpdateItemAsync(item);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_subscriber.Unregister(this);
					_store?.Dispose();
					_store = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}