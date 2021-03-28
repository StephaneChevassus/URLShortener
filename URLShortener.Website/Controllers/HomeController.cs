using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using URLShortener.Website.Interfaces;
using URLShortener.Website.Models;
using URLShortener.Website.Common;

namespace URLShortener.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShortUrlService _shortUrlService;

        public HomeController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost([Bind("Alias,Url")] ShortUrl shortUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (String.IsNullOrEmpty(shortUrl.Alias))
                        shortUrl.Alias = _shortUrlService.Add(shortUrl.Url);
                    else
                        _shortUrlService.Add(shortUrl.Alias, shortUrl.Url);

                    return View("Success", shortUrl);
                }
                catch // Future implementation - Check what type of exception is caught
                {
                    /* The only reason it would fail is if we are trying to add an alias
                     * that is already existing.
                     * 
                     * When using Remote Attribute validation, if javascript is disabled
                     * then the validation is never checked on the server.
                     * 
                     * It says in the documentation that it is used for client validation however it would
                     * be beneficial to also validate it on the server in case javascript is disabled.
                     * It can be achieved by creating an extension of RemoteAttribute and overriding IsValid method 
                     * although it may not be the right way of doing it.
                     * 
                     * There are other ways to explore like: Create custom attributes / Implement IValidatableObject
                     * 
                     * https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-3.1#alternatives-to-built-in-attributes
                     */

                    ModelState.AddModelError("Alias", "Alias already exists.");
                }
            }
            return View();
        }

        public IActionResult AliasRedirect(string alias)
        {
            var shortUrl = _shortUrlService.GetByAlias(alias);

            if (shortUrl == null)
                return NotFound();
            else
                return Redirect(shortUrl.Url);
        }

        public IActionResult ShortUrls()
        {
            return View("ShortUrls", _shortUrlService.Get());
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyAlias(string alias)
        {
            // Check for reserved alias
            if (IsAliasAttributeValid(alias))
            {
                if (AliasExists(alias))
                    return Json($"Alias \"{alias}\" already exists.");
                else
                    return Json(true);
            }
            else
                return Json($"Alias \"{alias}\" contains illegal characters.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsAliasAttributeValid(string alias)
        {
            if (String.IsNullOrEmpty(alias)) // Alias can be null or empty, then we auto-generate one
                return true;
            else if (!Regex.IsMatch(alias, "^[a-zA-Z0-9]*$")) // Alias must be an alphanumeric string
                return false;
            else
                return true;
        }

        private bool AliasExists(string alias) => _shortUrlService.AliasExists(alias);
    }
}
