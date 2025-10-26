using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;
using MediatR;

namespace Client.Application.Features.Role.Commands
{
    public record UpdateRoleCommand(UpdateRoleDto Role) : IRequest<List<RoleDto>>;

}
