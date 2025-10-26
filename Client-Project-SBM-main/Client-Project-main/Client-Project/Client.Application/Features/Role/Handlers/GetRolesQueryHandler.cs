using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;
using Client.Application.Features.Role.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Role.Handlers
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository _repository;

        public GetRolesQueryHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetRolesAsync(request.Id);
        }
    }

}
