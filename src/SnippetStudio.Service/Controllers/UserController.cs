using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnippetStudio.Contracts;
using SnippetStudio.Service.Services;

namespace SnippetStudio.Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserController : ControllerBase
	{
		private readonly UserService _userService;

		public UserController(UserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public async Task<ActionResult<List<User>>> List()
		{
			var items = await _userService.GetItemsForUserAsync(User.Identity.Name).ToListAsync();
			return Ok(items);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetItem(Guid id)
		{
			var item = await _userService.GetItemAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			return Ok(item);
		}

		[HttpPost]
		public ActionResult Create([FromBody] CreateUser command)
		{
			_userService.CreateUser(command, User.Identity.Name);
			return Ok();
		}

		[HttpPut]
		public ActionResult Update([FromBody] UpdateUser command)
		{
			_userService.UpdateUser(command, User.Identity.Name);
			return Ok();
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(Guid id)
		{
			_userService.DeleteUser(id, User.Identity.Name);
			return Ok();
		}
	}
}