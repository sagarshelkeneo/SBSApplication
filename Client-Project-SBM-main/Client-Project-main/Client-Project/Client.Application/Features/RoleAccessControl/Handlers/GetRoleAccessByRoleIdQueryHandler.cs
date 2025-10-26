using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client.Application.Features.RoleAccessControl.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Handlers
{
    public class GetRoleAccessByRoleIdQueryHandler : IRequestHandler<GetRoleAccessByRoleIdQuery, List<GetRoleAccessByRoleIdDto>>
    {
        private readonly IRoleAccessRepository _roleAccess;
        public GetRoleAccessByRoleIdQueryHandler(IRoleAccessRepository roleAccess)
        {
            _roleAccess = roleAccess;
        }
        public async Task<List<GetRoleAccessByRoleIdDto>> Handle(GetRoleAccessByRoleIdQuery request, CancellationToken cancellationToken)
        {
            return await _roleAccess.GetRoleAccessByRoleIdAsync(request.id);
        }
    }
}
