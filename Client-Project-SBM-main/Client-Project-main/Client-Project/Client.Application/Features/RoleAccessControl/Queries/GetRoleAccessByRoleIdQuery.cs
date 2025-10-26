using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;
using MediatR;

namespace Client.Application.Features.RoleAccessControl.Queries
{
    public record GetRoleAccessByRoleIdQuery(int id): IRequest<List<GetRoleAccessByRoleIdDto>>;
}
