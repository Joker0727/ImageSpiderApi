using ImageSpiderApi.EF;
using ImageSpiderApi.Models;
using ImageSpiderApi.MyToolHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
        /// 获取微信小程序的授权信息，并且判断登陆状态
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpGet, Route("getwxvalidate"), ResponseType(typeof(WxResponseUserInfo))]
        public async Task<IHttpActionResult> GetWxValidate(string code, string encryptedData, string iv)
        {
            WxResponseUserInfo responseData = null;
            try
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
                        responseData.unionId = _usrInfo["unionId"]?.ToString();
                    }
                    catch (Exception)
                    {
                        responseData.unionId = "null";
                    }

                    string openId = responseData?.openId;
                    var resAccObj = await ise.Accounts.Where(w => w.OpenId == openId).FirstOrDefaultAsync();
                    DateTime currentTime = DateTime.Now;
                    if (resAccObj == null)
                    {
                        Account newAccObj = new Account();
                        newAccObj.OpenId = openId;
                        newAccObj.UnionId = responseData.unionId;
                        newAccObj.NickName = responseData.nickName;
                        newAccObj.Gender = Convert.ToInt32(responseData.gender) == 1 ? true : false;
                        newAccObj.Country = responseData.country;
                        newAccObj.Province = responseData.province;
                        newAccObj.City = responseData.city;
                        newAccObj.AvatarUrl = responseData.avatarUrl;
                        newAccObj.RegistrationTime = currentTime;
                        newAccObj.LatestLoginTime = currentTime;
                        ise.Accounts.Add(newAccObj);
                        await ise.SaveChangesAsync();
                        AccessRecord accessRecord = new AccessRecord();
                        accessRecord.OpenId = newAccObj.OpenId;
                        accessRecord.AccessTime = currentTime;
                        ise.AccessRecords.Add(accessRecord);
                        await ise.SaveChangesAsync();

                    }
                    else
                    {
                        resAccObj.LatestLoginTime = currentTime;
                        await ise.SaveChangesAsync();
                    }
                    return Ok(responseData);
                }
                else
                {
                    return Ok(responseData);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 新增收藏记录
        /// </summary>
        /// <param name="openId">用户的openId</param>
        /// <param name="imageId">图片的Id</param>
        /// <returns></returns>
        [HttpGet, Route("addcollectionrecord"), ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> AddCollectionRecord(string openId, int imageId)
        {
            string message = string.Empty;
            AddCollectionRecordDto addCollectionRecordDto = null;
            if (string.IsNullOrEmpty(openId) || imageId < -1)
            {
                message = "参数不合法！";
                return BadRequest(message);
            }
            try
            {
                Collection collectionObj = new Collection
                {
                    OpenId = openId,
                    ImageId = imageId,
                    CollectionTime = DateTime.Now
                };
                ise.Collections.Add(collectionObj);
                await ise.SaveChangesAsync();
                addCollectionRecordDto = new AddCollectionRecordDto
                {
                    Id = collectionObj.Id,
                    OpenId = collectionObj.OpenId,
                    ImageId = collectionObj.ImageId,
                    CollectionTime = collectionObj.CollectionTime
                };
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return BadRequest(message);
            }
            return Ok(addCollectionRecordDto);
        }
        /// <summary>
        /// 删除收藏的图片
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpGet, Route("deletecollectionrecord"), ResponseType(typeof(Collection))]
        public async Task<IHttpActionResult> DeleteCollectionRecord(string openId, int imageId)
        {
            string message = string.Empty;
            if (string.IsNullOrEmpty(openId) || imageId < -1)
            {
                message = "参数不合法！";
                return BadRequest(message);
            }
            Collection collectionObj = null;
            try
            {
                collectionObj = await ise.Collections.Where(w => w.OpenId == openId && w.ImageId == imageId).FirstOrDefaultAsync();
                if (collectionObj != null)
                    ise.Collections.Remove(collectionObj);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return BadRequest(message);
            }
            return Ok(collectionObj);
        }
    }
}