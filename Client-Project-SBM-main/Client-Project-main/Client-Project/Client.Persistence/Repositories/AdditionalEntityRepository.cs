using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{

    public class AdditionalEntityRepository : IAdditionalEntityRepository
    {
        private readonly IDbConnection _connection;

        public AdditionalEntityRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<AdditionalEntityDto>> InsertAsync(CreateAdditionalEntityDto dto)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_additional_entities_insert",
                new
                {
                    p_type = dto.Type,
                    p_amount = dto.Amount,
                    p_quantity = dto.Quantity,
                    p_date = dto.Date,
                    p_companyId = dto.CompanyId,
                    p_subContractorId = dto.SubContractorId,
                    p_createdBy = dto.CreatedBy
                },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(dto.CompanyId);
        }

        public async Task<List<AdditionalEntityDto>> UpdateAsync(UpdateAdditionalEntityDto dto)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_additional_entities_update",
                new
                {
                    p_id = dto.Id,
                    p_type = dto.Type,
                    p_amount = dto.Amount,
                    p_quantity = dto.Quantity,
                    p_date = dto.Date,
                    p_companyId = dto.CompanyId,
                    p_subContractorId = dto.SubContractorId,
                    p_updatedBy = dto.UpdatedBy
                },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(dto.CompanyId);
        }

        public async Task<List<AdditionalEntityDto>> DeleteAsync(int id, int updatedBy, int companyId)
        {
            await _connection.ExecuteAsync(
                "usp_sbs_additional_entities_delete",
                new { p_id = id, p_updatedBy = updatedBy },
                commandType: CommandType.StoredProcedure);

            return await GetAsync(companyId);
        }


        public async Task<List<AdditionalEntityDto>> GetAsync(int companyId, int? id = null, int? subContractorId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_id", id);
            parameters.Add("p_companyId", companyId);
            parameters.Add("p_subContractorId", subContractorId);


            var result = await _connection.QueryAsync<AdditionalEntityDto>(
                "usp_sbs_additional_entities_get",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result.ToList();

        }
    }

}
