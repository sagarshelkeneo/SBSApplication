using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Company.Commands;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Company.Handlers
{
    public class SendCompanyEmailCommandHandler : IRequestHandler<SendCompanyEmailCommand, string>
    {
        private readonly ICompanyRepository _repository;

        public SendCompanyEmailCommandHandler(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(SendCompanyEmailCommand request, CancellationToken cancellationToken)
        {
            return await _repository.SendCompanyEmailAsync(request.Dto);
        }
    }

}
