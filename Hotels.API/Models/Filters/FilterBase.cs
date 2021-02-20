using System.Runtime.CompilerServices;
using System;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Hotels.API.Models.Filters
{
    public abstract class FilterBase
    {
        public int PageIndex { get; set; }
        public int RowsPerPage { get; set; }

        protected int SkipPage => (this.PageIndex > 0 ? this.PageIndex - 1 : 0) * this.RowsPerPage;
        protected int TakeCount => this.RowsPerPage;

        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public RouteValueDictionary NextRouteValues
        {
            get
            {
                if (this.RouteValues != null)
                {
                    var newRouteValue = JsonConvert.DeserializeObject<RouteValueDictionary>
                        (JsonConvert.SerializeObject(this.RouteValues));
                    newRouteValue["PageIndex"] = PageIndex >= 1 ? PageIndex + 1 : PageIndex;
                    newRouteValue["RowsperPage"] = RowsPerPage;
                    return newRouteValue;
                }
                return new RouteValueDictionary();
            }
        }

        public RouteValueDictionary PreviousRouteValues
        {
            get
            {
                if (this.RouteValues != null)
                {
                    var newRouteValue = JsonConvert.DeserializeObject<RouteValueDictionary>
                        (JsonConvert.SerializeObject(this.RouteValues));
                    newRouteValue["PageIndex"] = PageIndex > 1 ? PageIndex - 1 : 1;
                    newRouteValue["RowsperPage"] = RowsPerPage;
                    return newRouteValue;
                }
                return new RouteValueDictionary();
            }
        }

        public RouteValueDictionary CloneRouteValues(string routeType)
        {
            var newRouteValue = JsonConvert.DeserializeObject<RouteValueDictionary>(JsonConvert.SerializeObject(this.RouteValues));

            if (routeType == "next")
            {
                newRouteValue["PageIndex"] = PageIndex >= 1 ? PageIndex + 1 : PageIndex;
                newRouteValue["RowsperPage"] = RowsPerPage;

            }
            else
            {
                newRouteValue["PageIndex"] = PageIndex > 1 ? PageIndex - 1 : 1;
                newRouteValue["RowsperPage"] = RowsPerPage;
            }

            return newRouteValue;

        }


        public FilterBase()
        {
            this.PageIndex = 1;
            this.RowsPerPage = 10;
        }


    }
}
