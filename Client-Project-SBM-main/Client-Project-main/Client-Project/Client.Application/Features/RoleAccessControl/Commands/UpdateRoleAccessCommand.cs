using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Commands
{
    public class UpdateRoleAccessCommand : IRequest<string>
    {
        public UpdateRoleAccessDto RoleAccess { get; set; }
        public UpdateRoleAccessCommand(UpdateRoleAccessDto dto) => RoleAccess = dto;
    }
}
