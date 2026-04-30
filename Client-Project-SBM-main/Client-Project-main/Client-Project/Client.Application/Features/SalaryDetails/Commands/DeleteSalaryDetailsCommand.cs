using Client.Application.Features.SalaryDetails.Dtos;
using MediatR;

namespace Client.Application.Features.SalaryDetails.Commands
{
    public record DeleteSalaryDetailsCommand(int Id, int UpdatedBy, int CompanyId) : IRequest<List<SalaryDetailsDto>>;

}
