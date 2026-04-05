using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _db;

        public PaymentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<PaymentDetailsDto>> GetPaymentsAsync(int companyId,int? id = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", id);
            parameters.Add("@P_companyID", companyId);

            var result = await _db.QueryAsync<PaymentDetailsDto>(
                "usp_sbs_paymentDetails_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        //public async Task<PaymentDetailsDto?> CreatePaymentAsync(CreatePaymentDto dto)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@P_invoiceId", dto.InvoiceId);
        //    parameters.Add("@P_paymentDate", dto.PaymentDate);
        //    parameters.Add("@P_amountPaid", dto.AmountPaid);
        //    parameters.Add("@P_paymentMode", dto.PaymentMode);
        //    parameters.Add("@P_bankName", dto.BankName);
        //    parameters.Add("@P_paymentStatus", dto.PaymentStatus);
        //    parameters.Add("@P_createdBy", dto.CreatedBy);

        //    var insertResult = await _db.QueryFirstOrDefaultAsync<(string R_Status, int? R_InsertedID, int? R_ErrorNumber, string R_ErrorMessage)>(
        //        "usp_sbs_paymentDetails_insert",
        //        parameters,
        //        commandType: CommandType.StoredProcedure);

        //    if (insertResult.R_Status != "Success" || insertResult.R_InsertedID == null)
        //        return null; 

        //    var getParams = new DynamicParameters();
        //    getParams.Add("@P_id", insertResult.R_InsertedID.Value);

        //    var payment = await _db.QueryFirstOrDefaultAsync<PaymentDetailsDto>(
        //        "usp_sbs_paymentDetails_get",
        //        getParams,
        //        commandType: CommandType.StoredProcedure);

        //    return payment;
        //}

        public async Task<List<PaymentDetailsDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_invoiceNo", dto.InvoiceNo);
            parameters.Add("@P_paymentDate", dto.PaymentDate);
            parameters.Add("@P_amountPaid", dto.AmountPaid);
            parameters.Add("@p_fromDate", dto.FromDate);  
            parameters.Add("@p_toDate", dto.ToDate);
            parameters.Add("@P_paymentMode", dto.PaymentMode);
            parameters.Add("@P_bankId", dto.BankId);
            parameters.Add("@P_paymentStatus", dto.PaymentStatus);
            parameters.Add("@P_createdBy", dto.CreatedBy);


            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_paymentDetails_insert",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result == null || result.R_Status != "SUCCESS")
            {
                throw new Exception($"Insert failed: {result?.R_ErrorMessage ?? "Unknown error"}");
            }
            if (result.R_Status == "SUCCESS")
            {
                return await GetPaymentsAsync(dto.CompanyId, null);
            }

            throw new Exception($"Insert Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");

        }
        public async Task<List<PaymentDetailsDto>> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", dto.Id);
            parameters.Add("@P_invoiceNo", dto.InvoiceNo);
            parameters.Add("@P_paymentDate", dto.PaymentDate);
            parameters.Add("@P_fromDate", dto.FromDate);
            parameters.Add("@P_toDate", dto.ToDate);
            parameters.Add("@P_amountPaid", dto.AmountPaid);
            parameters.Add("@P_paymentMode", dto.PaymentMode);
            parameters.Add("@P_bankId", dto.BankId);
            parameters.Add("@P_paymentStatus", dto.PaymentStatus);
            parameters.Add("@P_updatedBy", dto.UpdatedBy);


            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_paymentDetails_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            if (result == null || result.R_Status != "SUCCESS")
            {
                throw new Exception($"Insert failed: {result?.R_ErrorMessage ?? "Unknown error"}");
            }
            if (result.R_Status == "SUCCESS")
            {
                return await GetPaymentsAsync(dto.CompanyId, null);
            }

            throw new Exception($"Update Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");

        }
        public async Task<List<PaymentDetailsDto>> DeletePaymentAsync(int id, int updatedBy,int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", id);
            parameters.Add("@P_updatedBy", updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<(string R_Status, int? R_DeletedID, int? R_ErrorNumber, string R_ErrorMessage)>(
                "usp_sbs_paymentDetails_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result.R_Status != "SUCCESS")
                throw new Exception($" Payment deletionfailed: {result.R_ErrorMessage ?? "Unknown error"}");

            return await GetPaymentsAsync(companyId,null);
        }


    }

}
