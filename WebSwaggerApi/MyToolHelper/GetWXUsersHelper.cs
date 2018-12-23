using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ImageSpiderApi.MyToolHelper
{
    /// <summary>
    /// 获取用户帮助类
    /// </summary>
    public class GetWXUsersHelper
    {
        /// <summary> 
        /// 获取链接返回数据 
        /// </summary> 
        /// <param name="Url">链接</param> 
        /// <param name="type">请求类型</param> 
        /// <returns></returns> 
        public string GetUrltoHtml(string Url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
        #region 微信<a href="http://www.51softs.com/tag/%e5%b0%8f%e7%a8%8b%e5%ba%8f" title="查看更多关于 小程序 的文章" target="_blank">小程序</a>用户数据解密 
        /// <summary>
        /// AES Key
        /// </summary>
        public static string AesKey;
        /// <summary>
        /// AES IV
        /// </summary>
        public static string AesIV;
        /// <summary> 
        /// AES解密 
        /// </summary> 
        /// <param name="inputdata">输入的数据encryptedData</param> 
        /// <returns name="result">解密后的字符串</returns> 
        public string AESDecrypt(string inputdata)
        {
            try
            {
                AesIV = AesIV.Replace(" ", "+");
                AesKey = AesKey.Replace(" ", "+");
                inputdata = inputdata.Replace(" ", "+");
                byte[] encryptedData = Convert.FromBase64String(inputdata);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(AesKey); // Encoding.UTF8.GetBytes(AesKey); 
                rijndaelCipher.IV = Convert.FromBase64String(AesIV);// Encoding.UTF8.GetBytes(AesIV); 
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string result = Encoding.UTF8.GetString(plainText);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}