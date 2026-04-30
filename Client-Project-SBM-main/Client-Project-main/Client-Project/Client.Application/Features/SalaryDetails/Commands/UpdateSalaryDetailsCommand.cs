using Client.Application.Features.SalaryDetails.Dtos;
using MediatR;

namespace Client.Application.Features.SalaryDetails.Commands
{
    public record UpdateSalaryDetailsCommand(UpdateSalaryDetailsDto Dto) : IRequest<List<SalaryDetailsDto>>;

}
