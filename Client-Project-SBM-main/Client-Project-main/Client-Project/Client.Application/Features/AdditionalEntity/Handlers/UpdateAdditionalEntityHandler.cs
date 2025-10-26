using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Commands;
using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.AdditionalEntity.Handlers
{
    public class UpdateAdditionalEntityHandler : IRequestHandler<UpdateAdditionalEntityCommand, List<AdditionalEntityDto>>
    {
        private readonly IAdditionalEntityRepository _repo;

        public UpdateAdditionalEntityHandler(IAdditionalEntityRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AdditionalEntityDto>> Handle(UpdateAdditionalEntityCommand request, CancellationToken cancellationToken)
        {
            return await _repo.UpdateAsync(request.Dto);
        }
    }
}
