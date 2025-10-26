using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.User.Dtos
{
    public class LoginResult
    {
        public string isValid { get; set; }
        public int user_Id { get; set; }
        public int company_ID { get; set; }
       
    }
}
