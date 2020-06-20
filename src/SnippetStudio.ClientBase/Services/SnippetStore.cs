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
	public class SnippetStore : IDataStore<SnippetModel>
	{
		private readonly IRequestClient _client;
		private readonly TokenProvider _tokenProvider;
		private IEnumerable<SnippetModel> items;

		public SnippetStore(IRequestClient client, TokenProvider tokenProvider)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			items = new List<SnippetModel>();
		}

		public async Task<IEnumerable<SnippetModel>> GetItemsAsync(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				var response = await _client.GetAsync("api/Snippet", headers);
				response = response.EnsureSuccessStatusCode();
				var responseString = Encoding.UTF8.GetString(response.Body.ToArray());

				items = JsonConvert.DeserializeObject<List<SnippetModel>>(responseString);
			}

			return items;
		}

		public async Task<SnippetModel> GetItemAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				var response = await _client.GetAsync($"api/Snippet/{id}", headers);
				response = response.EnsureSuccessStatusCode();
				var responseString = Encoding.UTF8.GetString(response.Body.ToArray());

				return JsonConvert.DeserializeObject<SnippetModel>(responseString);
			}

			return null;
		}

		public async Task<bool> AddItemAsync(SnippetModel item)
		{
			if (item == null)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};

			var request = new CreateSnippet(item.Id,
				item.Name,
				item.Language,
				item.Variables,
				item.Template,
				item.Code);

			var response = await _client.PostAsync($"api/Snippet", JsonConvert.SerializeObject(request), "application/json", headers);
			response.EnsureSuccessStatusCode();

			return true;
		}

		public async Task<bool> UpdateItemAsync(SnippetModel item)
		{
			if (item == null || item.Id == Guid.Empty)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};

			var request = new UpdateSnippet(item.Id,
				item.Name,
				item.Language,
				item.Variables,
				item.Template,
				item.Code);

			var response = await _client.PutAsync($"api/Snippet", JsonConvert.SerializeObject(request), "application/json", headers);
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

			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};

			var response = await _client.DeleteAsync($"api/Snippet/{id}", headers);
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