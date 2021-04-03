using Cosei.Client.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.ClientBase.Models;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class UserStore : IDataStore<UserModel>
	{
		private readonly IRequestClient _client;
		private readonly TokenProvider _tokenProvider;
		private IEnumerable<UserModel> items;

		public UserStore(IRequestClient client, TokenProvider tokenProvider)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			items = new List<UserModel>();
		}

		public async Task<IEnumerable<UserModel>> GetItemsAsync(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				items = await _client.GetAsync<List<UserModel>>("api/User", headers);
			}

			return items;
		}

		public async Task<UserModel> GetItemAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				return await _client.GetAsync<UserModel>($"api/User/{id}", headers);
			}

			return null;
		}

		public async Task<bool> AddItemAsync(UserModel item)
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

			var request = new CreateUser(item.Id,
				item.Username,
				item.EMail,
				item.Password);

			await _client.PostAsync($"api/User", request, headers);

			return true;
		}

		public async Task<bool> UpdateItemAsync(UserModel item)
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

			var request = new UpdateUser(item.Id,
				item.Username,
				item.EMail,
				item.Password);

			await _client.PutAsync($"api/User", request, headers);

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

			var response = await _client.DeleteAsync($"api/User/{id}", headers);
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