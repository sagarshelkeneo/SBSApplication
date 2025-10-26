using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Dtos;
using Client.Application.Features.User.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.User.Handlers
{
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IUserRepository _repository;

        public GetUsersQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetUsersAsync(request.Id, request.Search,request.companyId);
        }
    }

}
