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
	public class LanguageController : ControllerBase
	{
		private readonly LanguageService _languageService;

		public LanguageController(LanguageService languageService)
		{
			_languageService = languageService;
		}

		[HttpGet]
		public ActionResult<List<Language>> List()
		{
			var items = _languageService.GetItemsForUser(User.Identity.Name);
			return Ok(items);
		}

		[HttpGet("{id}")]
		public ActionResult<Language> GetItem(Guid id)
		{
			var item = _languageService.GetItem(id);
			if (item == null)
			{
				return NotFound();
			}

			return Ok(item);
		}

		[HttpPost]
		public ActionResult Create([FromBody] CreateLanguage command)
		{
			_languageService.CreateLanguage(command, User.Identity.Name);
			return Ok();
		}

		[HttpPut]
		public ActionResult Update([FromBody] UpdateLanguage command)
		{
			_languageService.UpdateLanguage(command, User.Identity.Name);
			return Ok();
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(Guid id)
		{
			_languageService.DeleteLanguage(id, User.Identity.Name);
			return Ok();
		}
	}
}