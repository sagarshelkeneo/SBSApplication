using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;
using MediatR;

namespace Client.Application.Features.Role.Queries
{
    public record GetRolesQuery(int? Id = null) : IRequest<List<RoleDto>>;

}
