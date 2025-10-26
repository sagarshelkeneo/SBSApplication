using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Company.Dtos;
using MediatR;

namespace Client.Application.Features.Company.Commands
{
    public class SendCompanyEmailCommand : IRequest<string>
    {
        public SendCompanyEmailDto Dto { get; set; }

        public SendCompanyEmailCommand(SendCompanyEmailDto dto)
        {
            Dto = dto;
        }
    }

}
