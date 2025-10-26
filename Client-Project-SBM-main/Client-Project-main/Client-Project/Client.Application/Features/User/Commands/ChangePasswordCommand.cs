using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Dtos;
using MediatR;

namespace Client.Application.Features.User.Commands
{
    public class ChangePasswordCommand : IRequest<string>
    {
        public ChangePasswordDto PasswordDto { get; set; }
    }

}
