using Cosei.Client.RabbitMq;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SnippetStudio.Contracts;

namespace SnippetStudio.ClientBase.Services
{
	public class TokenProvider
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

				var serverPasswordBytes = Encoding.UTF8.GetBytes(_password);

				using (var sha = SHA256.Create())
				{
					var passwordHash = sha.ComputeHash(serverPasswordBytes);

					using (var client = _clientFactory())
					{
						var request = new AuthRequest
						{
							Username = _userName,
							PasswordHash = passwordHash
						};

						var response = await client.PostAsync("api/Auth", JsonConvert.SerializeObject(request), "application/json");
						response = response.EnsureSuccessStatusCode();
						_token = Encoding.UTF8.GetString(response.Body.ToArray());
					}
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