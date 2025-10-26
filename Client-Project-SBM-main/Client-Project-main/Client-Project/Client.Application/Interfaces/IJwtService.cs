using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Dtos;

namespace Client.Application.Interfaces
{

    public interface IJwtService
    {
        string GenerateToken(UserDto user);
    }

}
