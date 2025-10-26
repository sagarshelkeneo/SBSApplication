using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Queries
{

    public class GetUserAccessQuery : IRequest<List<UserAccessDto>>
    {
        public int? UserId { get; set; }
        public string Username { get; set; }

        public GetUserAccessQuery(int? userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }

}
