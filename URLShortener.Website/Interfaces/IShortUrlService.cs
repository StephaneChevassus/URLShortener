using System.Collections.Generic;
using URLShortener.Website.Models;

namespace URLShortener.Website.Interfaces
{
    public interface IShortUrlService
    {
        List<ShortUrl> Get();
        ShortUrl GetByAlias(string alias);
        /// <summary>
        /// Add a url without alias
        /// </summary>
        /// <returns>auto-generated alias</returns>
        string Add(string url);
        void Add(string alias, string url);
        bool AliasExists(string alias);
    }
}