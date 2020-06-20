using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
		public ActionResult<List<User>> List()
		{
			var items = _userService.GetItemsForUser(User.Identity.Name);
			return Ok(items);
		}

		[HttpGet("{id}")]
		public ActionResult<User> GetItem(Guid id)
		{
			var item = _userService.GetItem(id);
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