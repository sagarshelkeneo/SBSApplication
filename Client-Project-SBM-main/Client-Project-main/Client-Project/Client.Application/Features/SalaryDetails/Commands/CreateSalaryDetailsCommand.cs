using Client.Application.Features.SalaryDetails.Dtos;
using MediatR;

namespace Client.Application.Features.SalaryDetails.Commands
{
    public record CreateSalaryDetailsCommand(CreateSalaryDetailsDto Dto) : IRequest<List<SalaryDetailsDto>>;

}
