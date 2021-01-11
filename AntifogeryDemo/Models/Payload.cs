using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntifogeryDemo.Models
{
    public class Payload
    {
        //使用者資訊
        public User info { get; set; }
        //過期時間
        public DateTime exp { get; set; }
    }
}