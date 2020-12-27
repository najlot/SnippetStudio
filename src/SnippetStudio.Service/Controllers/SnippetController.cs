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
	public class SnippetController : ControllerBase
	{
		private readonly SnippetService _snippetService;

		public SnippetController(SnippetService snippetService)
		{
			_snippetService = snippetService;
		}

		[HttpGet]
		public async Task<ActionResult<List<Snippet>>> List()
		{
			var items = await _snippetService.GetItemsForUserAsync(User.Identity.Name).ToListAsync();
			return Ok(items);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Snippet>> GetItem(Guid id)
		{
			var item = await _snippetService.GetItemAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			return Ok(item);
		}

		[HttpPost]
		public ActionResult Create([FromBody] CreateSnippet command)
		{
			_snippetService.CreateSnippet(command, User.Identity.Name);
			return Ok();
		}

		[HttpPut]
		public ActionResult Update([FromBody] UpdateSnippet command)
		{
			_snippetService.UpdateSnippet(command, User.Identity.Name);
			return Ok();
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(Guid id)
		{
			_snippetService.DeleteSnippet(id, User.Identity.Name);
			return Ok();
		}
	}
}