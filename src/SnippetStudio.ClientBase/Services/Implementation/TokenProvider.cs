using Cosei.Client.Base;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services.Implementation
{
	public class TokenProvider : ITokenProvider
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly Func<IRequestClient> _clientFactory;
		private readonly string _userName;
		private readonly string _password;
		private DateTime _tokenValidUntil;
		private string _token;

		public TokenProvider(Func<IRequestClient> clientFactory, string userName, string password)
		{
			_clientFactory = clientFactory;
			_userName = userName;
			_password = password;
		}

		public async Task<string> GetToken()
		{
			if (_tokenValidUntil > DateTime.Now)
			{
				return _token;
			}

			await _semaphore.WaitAsync();

			try
			{
				if (_tokenValidUntil > DateTime.Now)
				{
					return _token;
				}

				using (var client = _clientFactory())
				{
					var request = new AuthRequest
					{
						Username = _userName,
						Password = _password
					};

					var response = await client.PostAsync("api/Auth", JsonSerializer.Serialize(request), "application/json");
					response = response.EnsureSuccessStatusCode();
					_token = Encoding.UTF8.GetString(response.Body.ToArray());
				}

				_tokenValidUntil = DateTime.Now.AddHours(1);

				return _token;
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}