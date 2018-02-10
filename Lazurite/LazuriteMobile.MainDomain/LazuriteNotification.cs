using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public class LazuriteNotification
    {
        public Message Message { get; set; }
        public bool IsRead { get; set; }
        public int Id { get; set; }
    }
}
