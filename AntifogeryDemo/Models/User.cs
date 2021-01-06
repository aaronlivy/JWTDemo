using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntifogeryDemo.Models
{
    public class User
    {
        public string Name { get; set; }
        public string CustId { get; set; }
        public DateTime LoginDateTime { get; set; }
    }
}