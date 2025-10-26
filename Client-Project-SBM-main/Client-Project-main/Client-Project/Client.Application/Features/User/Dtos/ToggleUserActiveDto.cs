using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.User.Dtos
{
    public class ToggleUserActiveDto
    {
        public int Id { get; set; }
        public int IsActive { get; set; } // 1 or 0
    }
}
