using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP.NET_MVC___SignalR_sample.Models
{
    public class Operation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte Progress { get; set; }

        public bool IsFinished { get; set; }
    }
}