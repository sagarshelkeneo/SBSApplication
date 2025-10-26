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

    //public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, List<UserDto>>
    //{
    //    private readonly IUserRepository _repository;

    //    public UpdateUserCommandHandler(IUserRepository repository)
    //    {
    //        _repository = repository;
    //    }

    //    public async Task<List<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    //    {
    //        return await _repository.UpdateUserAsync(request.User);
    //    }
    //}
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.UpdateUserAsync(request.UserDto);
        }
    }


}
