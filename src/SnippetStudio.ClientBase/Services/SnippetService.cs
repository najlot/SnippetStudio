using Cosei.Client.RabbitMq;
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
		private readonly IDispatcherHelper _dispatcher;
		private readonly ISubscriber _subscriber;
		private readonly string _myName;

		public SnippetService(
			IDataStore<SnippetModel> dataStore,
			Messenger messenger,
			CsScriptRunService csScriptRunService,
			IDispatcherHelper dispatcher,
			ISubscriber subscriber,
			string myName)
		{
			_store = dataStore;
			_messenger = messenger;
			_csScriptRunService = csScriptRunService;
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

		public SnippetModel CreateSnippet()
		{
			return new SnippetModel()
			{
				Id = Guid.NewGuid(),
				Name = "",
				Language = "C#",
				Template = "",
				Code = @"
var result = Template;

foreach(var variable in Variables)
{
	result = result.Replace(""%"" + variable.Key + ""%"", variable.Value);
}

return result;",
			};
		}

		public async Task<string> Run(string language, string code, string template, Dictionary<string, string> variables)
		{
			var result = await Task.Run(async () => await _csScriptRunService.Run(code, template, variables));
			return result;
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