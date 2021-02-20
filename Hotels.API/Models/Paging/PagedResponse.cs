using System.Collections.Generic;

namespace Hotels.API.Models.Paging
{
    public class PagedResponse<T> where T : class
    {
        
        public IEnumerable<T> Data {get;set;}

        public Links Links {get;set;}

    }
}

