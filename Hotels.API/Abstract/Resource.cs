using System;
using Newtonsoft.Json;

namespace Hotels.API.Abstract
{
    public class Resource
    {
        // :TODO : HBK - Property Order neden çalışmıyor...
        [JsonProperty(Order = -1)]
        public string Href {get;set;}
    }
}
