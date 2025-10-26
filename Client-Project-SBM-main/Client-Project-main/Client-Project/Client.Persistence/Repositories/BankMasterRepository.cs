using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{
    public class BankMasterRepository : IBankMasterRepository
    {
        private readonly IDbConnection _db;

        public BankMasterRepository(IDbConnection db)
        {
            _db = db;
        }
        public async Task<List<BankMasterDto>> CreateBankAsync(CreateBankMasterDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_bankName", dto.BankName);
            parameters.Add("@P_branch", dto.Branch);
            parameters.Add("@P_createdBy", dto.CreatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_bankMaster_insert",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Insert Failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            // Return the full list of banks after creation
            return await GetBanksAsync(null);
        }
        public async Task<List<BankMasterDto>> UpdateBankAsync(UpdateBankMasterDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", dto.Id);
            parameters.Add("@P_bankName", dto.BankName);
            parameters.Add("@P_branch", dto.Branch);
            parameters.Add("@P_updatedBy", dto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_bankMaster_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Update Failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            return await GetBanksAsync(null);
        }
        public async Task<List<BankMasterDto>> DeleteBankAsync(DeleteBankMasterDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", dto.Id);
            parameters.Add("@P_updatedBy", dto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_bankMaster_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Delete failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            return await GetBanksAsync(null);
        }


        public async Task<List<BankMasterDto>> GetBanksAsync(int? id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", id);

            var result = await _db.QueryAsync<BankMasterDto>(
                "usp_sbs_bankMaster_get",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}
