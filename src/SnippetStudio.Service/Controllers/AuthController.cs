using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Services;

namespace SnippetStudio.Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly TokenService _tokenService;

		public AuthController(TokenService tokenService)
		{
			_tokenService = tokenService;
		}

		[AllowAnonymous]
		[HttpPost()]
		public ActionResult<string> Auth([FromBody] AuthRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var token = _tokenService.GetToken(request.Username, request.PasswordHash);

			if (token == null)
			{
				return Forbid();
			}

			return Ok(token);
		}
	}
}