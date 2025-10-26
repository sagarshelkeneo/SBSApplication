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
    //public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, List<UserDto>>
    //{
    //    private readonly IUserRepository _repository;

    //    public DeleteUserCommandHandler(IUserRepository repository)
    //    {
    //        _repository = repository;
    //    }

    //    public async Task<List<UserDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    //    {
    //        return await _repository.DeleteUserAsync(request.Id,request.updatedBy,request.companyId);
    //    }
    //}
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.DeleteUserAsync(request.Id, request.updatedBy, request.companyId);
        }
    }


}
