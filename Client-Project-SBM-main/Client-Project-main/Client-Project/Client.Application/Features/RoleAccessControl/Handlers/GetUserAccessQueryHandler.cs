using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client.Application.Features.RoleAccessControl.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Handlers
{
    public class GetUserAccessQueryHandler : IRequestHandler<GetUserAccessQuery, List<UserAccessDto>>
    {
        private readonly IRoleAccessRepository _roleAccess;

        public GetUserAccessQueryHandler(IRoleAccessRepository roleAccess)
        {
            _roleAccess = roleAccess;
        }

        public async Task<List<UserAccessDto>> Handle(GetUserAccessQuery request, CancellationToken cancellationToken)
        {
           return await _roleAccess.GetUserAccessAsync(request.UserId, request.Username);
        }
    }

}
