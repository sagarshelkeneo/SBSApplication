using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Role.Commands;
using Client.Application.Features.Role.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Role.Handlers
{


    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, List<RoleDto>>
    {
        private readonly IRoleRepository _repository;

        public DeleteRoleCommandHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDto>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteRoleAsync(request.Id, request.updatedBy);
        }
    }


}
