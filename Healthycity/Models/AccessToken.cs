using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.Models
{
    public class AccessToken
    {
        public ObjectId id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string TokenType { get; set; } // "Bearer" is expected
        public int ExpiresIn { get; set; } //maybe convert this to a DateTime ?
        public string RefreshToken { get; set; }
    }
}