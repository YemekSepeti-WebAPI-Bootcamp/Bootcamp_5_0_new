using System;

namespace Hotels.API.Models
{
    public class TokenRequest
    {
        public string LoginUser { get; set; }
        public string LoginPassword { get; set; }
    }
}
