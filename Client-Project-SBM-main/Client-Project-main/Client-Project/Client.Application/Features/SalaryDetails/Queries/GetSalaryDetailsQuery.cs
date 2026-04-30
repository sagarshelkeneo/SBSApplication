using Client.Application.Features.SalaryDetails.Dtos;
using MediatR;

namespace Client.Application.Features.SalaryDetails.Queries
{
    public record GetSalaryDetailsQuery(int CompanyId, int? Id = null, int? BankId = null) : IRequest<List<SalaryDetailsDto>>;

}
