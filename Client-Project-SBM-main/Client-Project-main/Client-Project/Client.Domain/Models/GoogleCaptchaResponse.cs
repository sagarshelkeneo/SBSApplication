using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Models
{
    public class GoogleCaptchaResponse
    {
        public bool success { get; set; }
        public List<string>? ErrorCodes { get; set; }
    }

}
