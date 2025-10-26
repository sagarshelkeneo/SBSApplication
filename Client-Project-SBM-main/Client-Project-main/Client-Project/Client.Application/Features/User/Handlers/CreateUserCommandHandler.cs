using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Commands;
using Client.Application.Features.User.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.User.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, List<UserDto>>
    {
        private readonly IUserRepository _repo;

        public CreateUserCommandHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _repo.CreateUserAsync(request.User);
        }
    }
}
