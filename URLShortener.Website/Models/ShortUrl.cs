using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using URLShortener.Website.Common;

namespace URLShortener.Website.Models
{
    public class ShortUrl
    {
        // I first implemented the Remote Attribute Validation however
        // Remote action only occurs when client javascript is enabled
        //[Remote(action: "VerifyAlias", controller: "Home")]

        // CustomRemote is executed by the client (if javascript is enabled) and subsequently by server
        [CustomRemote("VerifyAlias", "Home", ErrorMessage = "Alias already exists.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Alias contains illegal characters.")]
        [StringLength(20)]
        [DisplayName("Alias")]
        public string Alias { get; set; }

        // [Url] has some bugs therefore cannot rely on the built-in format
        // [Url] does not validate attribute correctly if javascript is disabled (in some cases). 
        // ModelState.IsValid returns true which is incorrect
        //[Url]

        // DataType.Url seems to also provide input validation and misbehave like [Url]
        //[DataType(DataType.Url)]

        // RegularExpression is executed on both client/server
        [RegularExpression(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$", ErrorMessage = "URL is invalid.")]
        // Can validate the Url with the built-in Regex attribute or via a CustomeRemote implementation
        [DisplayName("URL")]
        [Required]
        public string Url { get; set; }
    }
}
