using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Dtos;
using MediatR;

namespace Client.Application.Features.AdditionalEntity.Commands
{
    public record DeleteAdditionalEntityCommand(int Id, int UpdatedBy, int CompanyId) : IRequest<List<AdditionalEntityDto>>;

}
