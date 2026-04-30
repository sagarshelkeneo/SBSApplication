using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SalaryDetails.Dtos;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{

    public class SalaryDetailsRepository : ISalaryDetailsRepository
    {
        private readonly IDbConnection _connection;

        public SalaryDetailsRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<SalaryDetailsDto>> InsertAsync(CreateSalaryDetailsDto dto)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_SalaryDetails_insert",
                new
                {
                    p_ToliNo = dto.ToliNo,
                    p_amount = dto.Amount,
                    //p_quantity = dto.Quantity,
                    p_date = dto.Date,
                    p_companyId = dto.CompanyId,
                    p_bankId = dto.BankId,
                    p_createdBy = dto.CreatedBy
                },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(dto.CompanyId);
        }

        public async Task<List<SalaryDetailsDto>> UpdateAsync(UpdateSalaryDetailsDto dto)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_SalaryDetails_update",
                new
                {
                    p_id = dto.Id,
                    p_toliNo = dto.ToliNo,
                    p_amount = dto.Amount,
                    //p_quantity = dto.Quantity,
                    p_date = dto.Date,
                    p_companyId = dto.CompanyId,
                    p_bankId = dto.BankId,
                    p_updatedBy = dto.UpdatedBy
                },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(dto.CompanyId);
        }

        public async Task<List<SalaryDetailsDto>> DeleteAsync(int id, int updatedBy, int companyId)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_SalaryDetails_delete",
                new { p_id = id, p_updatedBy = updatedBy },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(companyId);
        }


        public async Task<List<SalaryDetailsDto>> GetAsync(int companyId, int? id = null, int? bankId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_id", id);
            parameters.Add("p_companyId", companyId);
            parameters.Add("p_bankId", bankId);


            var result = await _connection.QueryAsync<SalaryDetailsDto>(
                "usp_sbs_SalaryDetails_get",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result.ToList();

        }
    }

}
