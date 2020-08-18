using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrinterSpyService.Models
{
    public class Config
    {
        public int Id { get; set; }
        public byte AdEnabled { get; set; }
        public string AdServer { get; set; }
        public string AdUser { get; set; }
        public string AdPassword { get; set; }
    }
}
