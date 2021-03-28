using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using URLShortener.Website.Interfaces;
using URLShortener.Website.Models;

namespace URLShortener.Website.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private List<ShortUrl> _shortUrls;
        private List<String> _reservedAlias;

        public ShortUrlService()
        {
            _shortUrls = new List<ShortUrl>();

            //Seed some data
            _shortUrls.Add(new ShortUrl { Url = "https://dotnet.microsoft.com/apps/aspnet", Alias = "aspnet" });
            _shortUrls.Add(new ShortUrl { Url = "https://angular.io", Alias = "angular" });
            _shortUrls.Add(new ShortUrl { Url = "https://dotnet.microsoft.com/download", Alias = "dotnet" });

            // Add reserved words that can't be used as alias such as bad words etc...
            // Add controller route name etc...
            _reservedAlias = new List<string> { "Home", "Index", "Shared" };
        }

        public List<ShortUrl> Get()
        {
            return _shortUrls;
        }

        public ShortUrl GetByAlias(string alias)
        {
            return _shortUrls.FirstOrDefault(x => x.Alias == alias);
        }

        /// <summary>
        /// Add a url without alias
        /// </summary>
        /// <returns>auto-generated alias</returns>
        public string Add(string url)
        {
            string alias = CreateUniqueAlias();

            Add(alias, url);

            return alias;
        }

        public void Add(string alias, string url)
        {
            if (AliasExists(alias))
                throw new Exception("Cannot add shortUrl - Alias must be unique");

            var shortUrl = new ShortUrl
            {
                Alias = alias,
                Url = url
            };
            _shortUrls.Add(shortUrl);
        }

        public bool AliasExists(string alias)
        {
            if (String.IsNullOrEmpty(alias))
                return false;

            return (_shortUrls.Any(x => x.Alias.ToLower() == alias.ToLower()) || _reservedAlias.Any(str => str.ToLower().Equals(alias.ToLower())));
        }

        // Create a unique alias
        private string CreateUniqueAlias()
        {
            int counter = 0;
            int maxTry = 3;
            string alias;

            do
            {
                counter++;
                // Quick method to obtain a unique alphanumeric string of 8 chars
                // Very unlikely to iterate the same string twice, but just in case, let's give it a few tries
                alias = Guid.NewGuid().ToString("n").Substring(0, 8);
            } while (AliasExists(alias) && counter < maxTry);

            if (AliasExists(alias))
                throw new Exception("Cannot create unique Alias");

            return alias;
        }
    }
}
