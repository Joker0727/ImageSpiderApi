using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    /// <summary>
    /// 获取图片的实体
    /// </summary>
    public class GetImageDto
    {
        /// <summary>
        /// 图片id唯一标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 图片描述字段
        /// </summary>
        public string Alt { get; set; }
        /// <summary>
        /// 图片原链接
        /// </summary>
        public string OriginalUrl { get; set; }
        /// <summary>
        /// 图片现在的链接
        /// </summary>
        public string NewUrl { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public Nullable<double> Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public Nullable<double> Height { get; set; }
        /// <summary>
        /// 该图片所在目录的目录id
        /// </summary>
        public int? CatalogId { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        public string WebSiteUrl { get; set; }
        /// <summary>
        /// 是否已经下载
        /// </summary>
        public Nullable<bool> IsDownLoad { get; set; }
        /// <summary>
        /// 下载时间
        /// </summary>
        public Nullable<System.DateTime> DownLoadTime { get; set; }
    }
}