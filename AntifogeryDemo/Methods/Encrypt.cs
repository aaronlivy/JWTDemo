using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AntifogeryDemo.Methods
{
    internal static class Encrypt
    {
        /// <summary>
        /// AES字串加密(非對稱式)
        /// </summary>
        /// <param name="Source">加密前字串</param>
        /// <param name="CryptoKey">加密金鑰</param>
        /// <returns>加密後字串</returns>
        public static string aesEncryptBase64(string SourceStr, string CryptoKey)
        {
            var encrypt = "";
            try
            {
                var aes = new AesCryptoServiceProvider();
                var md5 = new MD5CryptoServiceProvider();
                var sha256 = new SHA256CryptoServiceProvider();
                var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                var iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                var dataByteArray = Encoding.UTF8.GetBytes(SourceStr);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return encrypt;
        }

        /// <summary>
        /// AES字串解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <param name="CryptoKey">解密金鑰</param>
        /// <returns>解密後字串</returns>
        public static string aesDecryptBase64(string SourceStr, string CryptoKey)
        {
            var decrypt = "";
            try
            {
                var aes = new AesCryptoServiceProvider();
                var md5 = new MD5CryptoServiceProvider();
                var sha256 = new SHA256CryptoServiceProvider();
                var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                var iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                var dataByteArray = Convert.FromBase64String(SourceStr);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return decrypt;
        }


        /// <summary>
        /// Base64字串加密
        /// </summary>
        /// <param name="AStr"></param>
        /// <returns></returns>
        public static string Base64Encode(string AStr)
        {
            var result = string.Empty;
            try
            {
                result = Convert.ToBase64String(Encoding.UTF8.GetBytes(AStr));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }


        /// <summary>
        /// Base64字串解密
        /// </summary>
        /// <param name="ABase64"></param>
        /// <returns></returns>
        public static string Base64Decode(string ABase64)
        {
            var result = string.Empty;
            try
            {
                result = Encoding.UTF8.GetString(Convert.FromBase64String(ABase64));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }
    }
}
