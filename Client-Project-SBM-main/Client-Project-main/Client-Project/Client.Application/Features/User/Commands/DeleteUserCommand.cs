using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Dtos;
using MediatR;

namespace Client.Application.Features.User.Commands
{
    public record DeleteUserCommand(int Id, int updatedBy, int companyId) : IRequest<List<UserDto>>;
    //public class DeleteUserCommand : IRequest<List<UserDto>>
    //{
    //    public int Id { get; set; }
    //    public int UpdatedBy { get; set; }
    //    public int CompanyId { get; set; }
    //}


}
