using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Commands;
using Client.Application.Features.Role.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Role.Handlers
{ 
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, List<RoleDto>>
    {
        private readonly IRoleRepository _repository;

        public UpdateRoleCommandHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            return await _repository.UpdateRoleAsync(request.Role);
        }
    }

}
