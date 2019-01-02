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
        /// <summary>
        /// 构造函数初始化
        /// </summary>
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
                    Account newAccObj = null;
                    AccessRecord accessRecord = null;
                    if (resAccObj == null)
                    {
                        newAccObj = new Account();
                        newAccObj.OpenId = openId;
                        newAccObj.UnionId = responseData.unionId;
                        newAccObj.NickName = responseData.nickName;
                        newAccObj.Gender = Convert.ToInt32(responseData.gender) == 1 ? true : false;
                        newAccObj.Country = responseData.country;
                        newAccObj.Province = responseData.province;
                        newAccObj.City = responseData.city;
                        newAccObj.AvatarUrl = responseData.avatarUrl;
                        newAccObj.RegistrationTime = currentTime;
                        ise.Accounts.Add(newAccObj);//添加新用户

                        accessRecord = new AccessRecord();
                        accessRecord.AccessTime = currentTime;
                        accessRecord.AccountId = newAccObj.Id;
                        ise.AccessRecords.Add(accessRecord);//添加的访问记录
                        await ise.SaveChangesAsync();
                    }
                    else
                    {
                        accessRecord = new AccessRecord();
                        accessRecord.AccessTime = currentTime;
                        accessRecord.AccountId = resAccObj.Id;
                        ise.AccessRecords.Add(accessRecord);
                        await ise.SaveChangesAsync();
                    }
                    responseData.AccountId = accessRecord.AccountId;
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
        /// 更新离开时间
        /// </summary>
        /// <param name="accessRecordId"></param>
        /// <returns></returns>
        [HttpGet, Route("updataexittime"), ResponseType(typeof(AccessRecord))]
        public async Task<IHttpActionResult> UpDataExitTime(int accessRecordId)
        {
            AccessRecord accessRecord = null;
            try
            {
                accessRecord = await ise.AccessRecords.Where(w => w.Id == accessRecordId).FirstOrDefaultAsync();
                accessRecord.ExitTime = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(((DateTime)accessRecord.AccessTime).Ticks);
                TimeSpan ts2 = new TimeSpan(((DateTime)accessRecord.ExitTime).Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                accessRecord.StayTime = ts.TotalMinutes;
                await ise.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(accessRecord);
        }
        /// <summary>
        /// 新增收藏记录
        /// </summary>
        /// <param name="accountId">用户的accountId</param>
        /// <param name="catalogId">图片的Id</param>
        /// <returns></returns>
        [HttpGet, Route("addcollectionrecord"), ResponseType(typeof(CollectionTable))]
        public async Task<IHttpActionResult> AddCollectionRecord(int accountId, int catalogId)
        {
            string message = string.Empty;
            AddCollectionRecordDto addCollectionRecordDto = null;
            if (accountId * catalogId < 0)
            {
                message = "参数不合法！";
                return BadRequest(message);
            }
            try
            {
                CollectionTable collectionObj = new CollectionTable
                {
                    AccountId = accountId,
                    CatalogId = catalogId,
                    CollectionTime = DateTime.Now
                };
                ise.CollectionTables.Add(collectionObj);
                await ise.SaveChangesAsync();
                addCollectionRecordDto = new AddCollectionRecordDto
                {
                    Id = collectionObj.Id,
                    AccountId = collectionObj.AccountId,
                    CatalogId = collectionObj.CatalogId,
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
        /// <param name="accountId">用户的accountId</param>
        /// <param name="catalogId">图片的Id</param>
        /// <returns></returns>
        [HttpGet, Route("deletecollectionrecord"), ResponseType(typeof(CollectionTable))]
        public async Task<IHttpActionResult> DeleteCollectionRecord(int accountId, int catalogId)
        {
            string message = string.Empty;
            if (accountId * catalogId < 0)
            {
                message = "参数不合法！";
                return BadRequest(message);
            }
            CollectionTable collectionObj = null;
            try
            {
                collectionObj = await ise.CollectionTables.Where(w => w.AccountId == accountId && w.CatalogId == catalogId).FirstOrDefaultAsync();
                if (collectionObj != null)
                    ise.CollectionTables.Remove(collectionObj);
                await ise.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return BadRequest(message);
            }
            return Ok(collectionObj);
        }
        /// <summary>
        /// 新增浏览记录
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="catalogId"></param>
        [HttpGet, Route("addbrowse"), ResponseType(typeof(AddBrowseDto))]
        public async Task<IHttpActionResult> AddBrowse(int accountId, int catalogId)
        {
            string message = string.Empty;
            AddBrowseDto addBrowseDto = null;
            if (accountId * catalogId < 0)
            {
                message = "参数不合法！";
                return BadRequest(message);
            }
            try
            {
                Browse browseObj = new Browse
                {
                    AccountId = accountId,
                    CatalogId = catalogId,
                    BrowseTime = DateTime.Now
                };
                ise.Browses.Add(browseObj);
                await ise.SaveChangesAsync();
                addBrowseDto = new AddBrowseDto
                {
                    Id = browseObj.Id,
                    AccountId = browseObj.AccountId,
                    CatalogId = browseObj.CatalogId,
                    BrowseTime = browseObj.BrowseTime
                };
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return BadRequest(message);
            }
            return Ok(addBrowseDto);
        }
    }
}