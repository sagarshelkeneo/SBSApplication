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
    public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public ToggleUserActiveCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.ToggleIsActiveAsync(request.Dto,request.CompanyId);
        }
    }

}
