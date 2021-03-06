﻿using ImageSpiderApi.EF;
using ImageSpiderApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ImageSpiderApi.Controllers
{
    /// <summary>
    /// ImageSpiderApi
    /// </summary>
    [RoutePrefix("api/imagespiderapi")]
    public class CatalogController : ApiController
    {
        private ImageSpiderEntities ise = null;
        /// <summary>
        /// 构造函数初始化
        /// </summary>
        public CatalogController()
        {
            ise = new ImageSpiderEntities();
        }
        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="accId">用户accId</param>
        /// <param name="page">第几页</param>
        /// <param name="count">每页的数量</param>
        /// <returns></returns>
        [HttpPost, Route("getcatalog"), ResponseType(typeof(GetCatalogDto))]
        public async Task<IHttpActionResult> GetCatalogAsync(int accId, int page = 1, int count = 10)
        {
            if (page < 1)
                return BadRequest("页数不能小于1！");
            if (count < 6)
                return BadRequest("每页图片数不能小于10！");
            List<GetCatalogDto> catalogList = new List<GetCatalogDto>();
            try
            {

                catalogList = await ise.CatalogTables
                            .OrderBy(o => o.Id)
                            .Skip(count * (page - 1))
                            .Take(count)
                            .Select(s => new GetCatalogDto
                            {
                                Id = s.Id,
                                Describe = s.Describe,
                                CatalogUrl = s.CatalogUrl,
                                TotalImages = s.TotalImages,
                                WebSiteUrl = s.WebSiteUrl,
                                IsDownLoad = s.IsDownLoad,
                                CoverUrl = ise.ImageTables.Where(w => w.CatalogId == s.Id).Select(e => new
                                {
                                    e.NewUrl
                                }).FirstOrDefault().NewUrl.ToString(),
                                IsCollection = ise.CollectionTables.Where(w => w.CatalogId == s.Id & w.AccountId == accId)
                               .ToList().Count() > 0 ? true : false,
                                BrowseCount = ise.Browses.Where(w => w.CatalogId == s.Id).ToList().Count(),
                                CollectionCount = ise.CollectionTables.Where(w => w.CatalogId == s.Id).ToList().Count()
                            }).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("查询错误：" + ex.ToString());
            }
            return Ok(catalogList);
        }
        /// <summary>
        /// 获取指定目录下的图片
        /// </summary>
        /// <param name="catalogId">图片的目录Id</param>
        /// <param name="page">页数</param>
        /// <param name="count">每页图片数</param>
        /// <returns></returns>
        [HttpPost, Route("getimage"), ResponseType(typeof(GetImageDto))]
        public async Task<IHttpActionResult> GetImage(int catalogId, int page, int count = 10)
        {
            if (catalogId < 0)
                return BadRequest("目录Id不合法");
            List<GetImageDto> ImageList = new List<GetImageDto>();
            try
            {

                ImageList = await ise.ImageTables.Where(w => w.CatalogId == catalogId)
                    .OrderBy(o => o.Id)
                    .Skip(count * (page - 1))
                    .Take(count)
                    .Select(s => new GetImageDto
                    {
                        Id = s.Id,
                        Guid = s.Guid,
                        Alt = s.Alt,
                        OriginalUrl = s.OriginalUrl,
                        NewUrl = s.NewUrl,
                        Width = s.Width,
                        Height = s.Height,
                        CatalogId = s.CatalogId,
                        WebSiteUrl = s.WebSiteUrl,
                        IsDownLoad = s.IsDownLoad,
                        DownLoadTime = s.DownLoadTime
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("获取图片信息失败：" + ex.ToString());
            }
            return Ok(ImageList);
        }
        /// <summary>
        /// 随机获取一张图片
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getrandompicture"), ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetRandomPicture()
        {
            List<ImageTable> imageList = await ise.ImageTables.OrderBy(a => Guid.NewGuid()).Take(1).ToListAsync();
            string resultStr = string.Empty;
            if (imageList.Count > 0)
                resultStr = imageList[0].NewUrl;
            return Ok(resultStr);
        }

    }
}