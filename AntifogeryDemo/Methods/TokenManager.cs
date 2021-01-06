using AntifogeryDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AntifogeryDemo.Methods
{
    public class TokenManager
    {
        //金鑰，從設定檔或資料庫取得
        public static string key = "AAAAAAAAAA-BBBBBBBBBB-CCCCCCCCCC-DDDDDDDDDD-EEEEEEEEEE-FFFFFFFFFF-GGGGGGGGGG";

        //紀錄 Refresh Token，需紀錄在資料庫
        private static Dictionary<string, User> refreshTokens = new Dictionary<string, User>();

        public static void Add(string Key, User obj) => refreshTokens.Add(Key, obj);

        public static User GetToken(string Key) => refreshTokens.Keys.Contains(Key) ? refreshTokens[Key] : null;

        public static bool ContainsKey(string Key) => refreshTokens.Keys.Contains(Key);

        public static void Remove(string Key)
        {
            if (refreshTokens.Keys.Contains(Key))
                refreshTokens.Remove(Key);
        }

        //產生 Token
        public static Token Create(User user)
        {
            var exp = 30;   //過期時間(秒)

            //稍微修改 Payload 將使用者資訊和過期時間分開
            var payload = new Payload
            {
                info = user,
                //Unix 時間戳
                exp = Convert.ToInt32(
                    (DateTime.Now.AddSeconds(exp) -
                     new DateTime(1970, 1, 1)).TotalSeconds)
            };

            var json = JsonConvert.SerializeObject(payload);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            var iv = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);

            //使用 AES 加密 Payload
            var encrypt = TokenCrypto
                .AESEncrypt(base64, key.Substring(0, 16), iv);

            //取得簽章
            var signature = TokenCrypto
                .ComputeHMACSHA256(iv + "." + encrypt, key.Substring(0, 64));

            return new Token
            {
                //Token 為 iv + encrypt + signature，並用 . 串聯
                access_token = iv + "." + encrypt + "." + signature,
                //Refresh Token 使用 Guid 產生
                refresh_token = Guid.NewGuid().ToString().Replace("-", ""),
                expires_in = exp,
            };
        }

        //取得使用者資訊
        public static User GetUser()
        {
            var token = HttpContext.Current.Request.Headers["Authoriaztion"];

            if (string.IsNullOrEmpty(token) || !ContainsKey(token)) return null;

            var split = token.Split('.');
            var iv = split[0];
            var encrypt = split[1];
            var signature = split[2];

            //檢查簽章是否正確
            if (signature != TokenCrypto
                .ComputeHMACSHA256(iv + "." + encrypt, key.Substring(0, 64)))
            {
                return null;
            }

            //使用 AES 解密 Payload
            var base64 = TokenCrypto
                .AESDecrypt(encrypt, key.Substring(0, 16), iv);
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var payload = JsonConvert.DeserializeObject<Payload>(json);

            //檢查是否過期
            if (payload.exp < Convert.ToInt32(
                (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds))
            {
                return null;
            }

            return payload.info;
        }
    }
}