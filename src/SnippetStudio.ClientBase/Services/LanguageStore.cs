using Cosei.Client.RabbitMq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class LanguageStore : IDataStore<LanguageModel>
	{
		private readonly IRequestClient _client;
		private readonly TokenProvider _tokenProvider;
		private IEnumerable<LanguageModel> items;

		public LanguageStore(IRequestClient client, TokenProvider tokenProvider)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			items = new List<LanguageModel>();
		}

		public async Task<IEnumerable<LanguageModel>> GetItemsAsync(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				var token = await _tokenProvider.GetToken();

				_client.DefaultHeaders.Clear();
				_client.DefaultHeaders.Add("Authorization", $"Bearer {token}");

				var response = await _client.GetAsync("api/Language");
				var responseString = Encoding.UTF8.GetString(response.Body);

				items = JsonConvert.DeserializeObject<List<LanguageModel>>(responseString);
			}

			return items;
		}

		public async Task<LanguageModel> GetItemAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var token = await _tokenProvider.GetToken();

				_client.DefaultHeaders.Clear();
				_client.DefaultHeaders.Add("Authorization", $"Bearer {token}");

				var response = await _client.GetAsync($"api/Language/{id}");
				var responseString = Encoding.UTF8.GetString(response.Body);

				return JsonConvert.DeserializeObject<LanguageModel>(responseString);
			}

			return null;
		}

		public async Task<bool> AddItemAsync(LanguageModel item)
		{
			if (item == null)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			_client.DefaultHeaders.Clear();
			_client.DefaultHeaders.Add("Authorization", $"Bearer {token}");

			var request = new CreateLanguage(item.Id,
				item.Name);

			var response = await _client.PostAsync($"api/Language", JsonConvert.SerializeObject(request), "application/json");
			response.EnsureSuccessStatusCode();

			return true;
		}

		public async Task<bool> UpdateItemAsync(LanguageModel item)
		{
			if (item == null || item.Id == Guid.Empty)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			_client.DefaultHeaders.Clear();
			_client.DefaultHeaders.Add("Authorization", $"Bearer {token}");

			var request = new UpdateLanguage(item.Id,
				item.Name);

			var response = await _client.PutAsync($"api/Language", JsonConvert.SerializeObject(request), "application/json");
			response.EnsureSuccessStatusCode();

			return true;
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			_client.DefaultHeaders.Clear();
			_client.DefaultHeaders.Add("Authorization", $"Bearer {token}");

			var response = await _client.DeleteAsync($"api/Language/{id}");
			response.EnsureSuccessStatusCode();

			return true;
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
					_client.Dispose();
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