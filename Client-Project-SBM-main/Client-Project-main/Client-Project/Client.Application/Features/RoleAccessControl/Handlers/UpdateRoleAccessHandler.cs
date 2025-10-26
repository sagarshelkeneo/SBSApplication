using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Commands;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Handlers
{

    public class UpdateRoleAccessHandler : IRequestHandler<UpdateRoleAccessCommand, string>
    {
        private readonly IRoleAccessRepository _roleAccess;
        public UpdateRoleAccessHandler(IRoleAccessRepository roleAccess) => _roleAccess = roleAccess;

        public async Task<string> Handle(UpdateRoleAccessCommand request, CancellationToken cancellationToken)
        {
            return await _roleAccess.UpdateRoleAccessAsync(request.RoleAccess);
        }
    }
}
