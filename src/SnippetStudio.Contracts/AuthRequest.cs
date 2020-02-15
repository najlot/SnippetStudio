namespace SnippetStudio.Contracts
{
	public class AuthRequest
	{
		public string Username { get; set; }
		public byte[] PasswordHash { get; set; }
	}
}