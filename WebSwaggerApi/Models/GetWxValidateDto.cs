using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSpiderApi.Models
{
    public class GetWxValidateDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public WxResponseUserInfo Data { get; set; }
    }
}