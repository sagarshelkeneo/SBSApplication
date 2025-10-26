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

    public class InsertRoleAccessHandler : IRequestHandler<InsertRoleAccessCommand, string>
    {
        private readonly IRoleAccessRepository _roleAccess;
        public InsertRoleAccessHandler(IRoleAccessRepository roleAccess) => _roleAccess = roleAccess;

        public async Task<string> Handle(InsertRoleAccessCommand request, CancellationToken cancellationToken)
        {
            return await _roleAccess.InsertRoleAccessAsync(request.RoleAccess);
        }
    }
}
