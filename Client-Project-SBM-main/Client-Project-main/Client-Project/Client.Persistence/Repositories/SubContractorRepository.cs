using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{

    public class SubContractorRepository : ISubContractorRepository
    {
        private readonly IDbConnection _db;

        public SubContractorRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<SubContractorDto>> GetSubContractorsAsync(int? id, string? search, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);
            parameters.Add("@p_search", search);
            parameters.Add("@p_companyID", companyId);

            var result = await _db.QueryAsync<SubContractorDto>(
                "usp_sbs_subContractor_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task <List<SubContractorDto>> CreateSubContractorAsync(CreateSubContractorDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_companyId", dto.CompanyId);
            parameters.Add("@p_name", dto.Name);
            parameters.Add("@p_createdBy", dto.CreatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_subContractor_insert",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result?.R_Status == "SUCCESS" && result?.R_InsertedID != null)
            {
                //return new SubContractorDto
                //{
                //    Id = result.InsertedID,
                //    CompanyId = dto.CompanyId,
                //    Name = dto.Name
                //};
                return await GetSubContractorsAsync(null, null, dto.CompanyId); 
            }

            throw new Exception($"Insert Failed: {result?.ErrorMessage ?? "Unknown error"}");
        }
        public async Task<List<SubContractorDto>> UpdateSubContractorAsync(UpdateSubContractorDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", dto.Id);
            parameters.Add("@p_companyId", dto.CompanyId);
            parameters.Add("@p_name", dto.Name);
            parameters.Add("@p_updatedBy", dto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<string>(
                "usp_sbs_subContractor_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == "SUCCESS")
            {
                var getParams = new DynamicParameters();
                getParams.Add("@p_id", null);
                getParams.Add("@p_search", null);
                getParams.Add("p_companyID", dto.CompanyId);

                var updatedList = await _db.QueryAsync<SubContractorDto>(
                    "usp_sbs_subContractor_get",
                    getParams,
                    commandType: CommandType.StoredProcedure
                );

                return updatedList.ToList();
            }

            throw new Exception("SubContractor update failed.");
        }
        public async Task<List<SubContractorDto>> DeleteSubContractorAsync(int id,int updatedBy, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);
            parameters.Add("@p_updatedBy",updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<string>(
                "usp_sbs_subContractor_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != "SUCCESS")
                throw new Exception(result ?? "Failed to delete subcontractor.");

            return await GetSubContractorsAsync(null, null, companyId);


        }


    }

}
