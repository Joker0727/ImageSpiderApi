using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    /// <summary>
    /// 添加用户收藏记录的实体类
    /// </summary>
    public class AddCollectionRecordDto
    {
        /// <summary>
        /// 记录id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户的OpenId
        /// </summary>
        public int? AccountId { get; set; }
        /// <summary>
        /// 图片的Id
        /// </summary>
        public Nullable<int> CatalogId { get; set; }
        /// <summary>
        /// 收藏时间
        /// </summary>
        public Nullable<System.DateTime> CollectionTime { get; set; }
    }
}