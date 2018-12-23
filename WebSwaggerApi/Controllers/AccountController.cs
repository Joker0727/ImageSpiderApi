using ImageSpiderApi.EF;
using ImageSpiderApi.Models;
using ImageSpiderApi.MyToolHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ImageSpiderApi.Controllers
{
    /// <summary>
    /// WeChat账号API
    /// </summary>
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private ImageSpiderEntities ise = null;

        public AccountController()
        {
            ise = new ImageSpiderEntities();
        }
        /// <summary>
        /// 获取微信小程序授权信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route("getwxvalidate"), ResponseType(typeof(WxResponseUserInfo))]
        public async Task<IHttpActionResult> GetWxValidate(string code, string encryptedData, string iv)
        {
            StringBuilder urlStr = new StringBuilder();
            urlStr.AppendFormat(@"https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}"
                    + "&grant_type=authorization_code",
                    ConfigurationManager.AppSettings["XCXAppID"].ToString(),
                    ConfigurationManager.AppSettings["XCXAppSecrect"].ToString(),
                    code
                );
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlStr.ToString());
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            WxValidateUserResponse vdModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WxValidateUserResponse>(retString);
            WxResponseUserInfo responseData = null;
            if (vdModel != null)
            {
                GetWXUsersHelper.AesIV = iv;
                GetWXUsersHelper.AesKey = vdModel.session_key;
                string result = new GetWXUsersHelper().AESDecrypt(encryptedData);
                JObject _usrInfo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                responseData = new WxResponseUserInfo
                {
                    nickName = _usrInfo["nickName"].ToString(),
                    gender = _usrInfo["gender"].ToString(),
                    city = _usrInfo["city"].ToString(),
                    province = _usrInfo["province"].ToString(),
                    country = _usrInfo["country"].ToString(),
                    avatarUrl = _usrInfo["avatarUrl"].ToString(),
                    sessionKey = vdModel.session_key
                };
                responseData.openId = _usrInfo["openId"].ToString();
                try
                {
                    responseData.unionId = _usrInfo["unionId"].ToString();
                }
                catch (Exception)
                {
                    responseData.unionId = "null";
                }
                return Ok(responseData);
            }
            else
            {
                return Ok(responseData);
            }
        }
    }
}