using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    public class GetRandomPictureDto
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Alt { get; set; }
        public string OriginalUrl { get; set; }
        public string NewUrl { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public int CatalogId { get; set; }
        public string WebSiteUrl { get; set; }
        public Nullable<bool> IsDownLoad { get; set; }
        public Nullable<System.DateTime> DownLoadTime { get; set; }
    }
}