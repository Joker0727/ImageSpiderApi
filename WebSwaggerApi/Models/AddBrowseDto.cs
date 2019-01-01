using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    public class AddBrowseDto
    {
        public int Id { get; set; }
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> CatalogId { get; set; }
        public Nullable<System.DateTime> BrowseTime { get; set; }
    }
}