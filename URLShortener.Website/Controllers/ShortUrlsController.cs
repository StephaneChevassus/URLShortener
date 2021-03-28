using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using URLShortener.Website.Interfaces;
using URLShortener.Website.Models;

namespace URLShortener.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlsController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlsController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        // GET: api/<ShortUrlsController>
        [HttpGet]
        public ActionResult<IEnumerable<ShortUrl>> Get()
        {
            return _shortUrlService.Get();
        }

        // GET api/<ShortUrlsController>/alias
        [HttpGet("{alias}")]
        public ActionResult<ShortUrl> Get(string alias)
        {
            var shortUrl = _shortUrlService.GetByAlias(alias);

            if(shortUrl == null)
            {
                return NotFound();
            }

            return shortUrl;
        }

        // POST api/<ShortUrlsController>
        [HttpPost]
        public ActionResult<ShortUrl> Post([FromBody] ShortUrl shortUrl)
        {
            if(String.IsNullOrEmpty(shortUrl.Alias))
                shortUrl.Alias = _shortUrlService.Add(shortUrl.Url);
            else
                _shortUrlService.Add(shortUrl.Alias, shortUrl.Url);

            return CreatedAtAction("Get", shortUrl);
        }
    }
}
