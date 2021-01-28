using AntifogeryDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;

namespace AntifogeryDemo.Methods
{
    public class TokenManager
    {
        //金鑰，從設定檔或資料庫取得
        public static string key = "AAAAAAAAAA-BBBBBBBBBB-CCCCCCCCCC-DDDDDDDDDD-EEEEEEEEEE-FFFFFFFFFF-GGGGGGGGGG";
        public const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        //紀錄 Refresh Token，需紀錄在資料庫
        private static Dictionary<string, Guid> refreshTokens = new Dictionary<string, Guid>();
        
        public static void Add(string Key, Guid guid) => refreshTokens.Add(Key, guid);

        public static Guid GetTokenGuid(string key) => refreshTokens.ContainsKey(key)? refreshTokens[key] : new Guid();

        public static bool ContainsKey(string Key) => refreshTokens.Keys.Contains(Key);

        public static void Remove(string Key)
        {
            if (refreshTokens.Keys.Contains(Key))
                refreshTokens.Remove(Key);
        }

        //產生 Token
        public static Token Create(User obj)
        {
            var exp = 30;   //過期時間(秒)

            //稍微修改 Payload 將使用者資訊和過期時間分開
            var payload = new Payload
            {
                info = obj,
                exp = DateTime.Now.AddSeconds(exp)
            };
            
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var guid = Guid.NewGuid();
            var iv = guid.ToString().Replace("-", "").Substring(0, 16);

            //使用 AES 加密 Payload
            var enc_str = Encrypt.aesEncryptBase64(JsonConvert.SerializeObject(payload), key.Substring(0, 16));
            
            var token = encoder.Encode(enc_str, secret);
            
            Add(token, guid);

            return new Token
            {
                //Token 為 iv + encrypt + signature，並用 . 串聯
                access_token = token,
                expires_in = exp,
            };
        }

        private static string Base64Encode(string content) => Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

        //取得使用者資訊
        public static User GetUser()
        {
            var token = HttpContext.Current.Request.Headers["RequestVerificationToken"];
            var json = string.Empty;

            if (string.IsNullOrEmpty(token) || ContainsKey(token)) return null;

            var guid = GetTokenGuid(token);
            
            var iv = guid.ToString().Replace("-", "").Substring(0, 16);
            var deconstring = string.Empty;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
    
                json = decoder.Decode(token, secret, verify:false ).Replace("\"", "");
                deconstring = Encrypt.aesDecryptBase64(json, key.Substring(0, 16));
                
                Console.WriteLine(json);
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
            
            var payload = JsonConvert.DeserializeObject<Payload>(deconstring);

            //檢查是否過期
            if (payload.exp < DateTime.Now)
            {
                return null;
            }

            System.Web.HttpContext.Current.Session["payload"] = deconstring;
            
            Remove(token);

            return payload.info;
        }
    }
}