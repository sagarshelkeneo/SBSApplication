using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Commands
{
    public class InsertRoleAccessCommand : IRequest<string>
    {
        public RoleAccessDto RoleAccess { get; set; }
        public InsertRoleAccessCommand(RoleAccessDto dto) => RoleAccess = dto;
    }
}
