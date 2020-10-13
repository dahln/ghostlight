using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace template.Server.Services
{
    public class EmailConfiguration
    {
        public string SmtpFromName { get; set; }
        public string SmtpFromEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpStartTLS { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}
