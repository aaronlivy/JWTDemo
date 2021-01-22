using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntifogeryDemo.Methods
{
    public class TokenCache
    {
        private Dictionary<string, DateTime> Pool = new Dictionary<string, DateTime>();
        private static TokenCache Instance = new TokenCache();
        private int seconds = 60;

        private TokenCache()
        {

        }

        private void _SetToken(string token)
        {
            if(!Pool.Keys.Contains(token))
            {
                var expiredate = DateTime.Now.AddSeconds(seconds);
                Pool.Add(token, expiredate);
            }
                
        }        

        public static void SetToken(string token)
        {
            if (Instance == null)
                Instance = new TokenCache();

            Instance._SetToken(token);
        }

        private bool _GetToken(string token)
        {
            var KeyExist = Pool.Keys.Contains(token);
            var result = false;

            if (KeyExist)
            {
                var item = Pool[token];
                if (item >= DateTime.Now)
                    result = true;

                Pool.Remove(token);
            }

            return result;
        }

        public static bool GetToken(string token)
        {
            if (Instance == null)
                return false;
            
            return Instance._GetToken(token);
        }

    }
}