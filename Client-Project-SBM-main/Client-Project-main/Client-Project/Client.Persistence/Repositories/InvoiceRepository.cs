using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Commands;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbConnection _db;

        public InvoiceRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<InvoiceDetailsDto>> GetInvoicesAsync(bool IsLeviApplicable, int companyId, int? id = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@P_id", id);
            parameters.Add("@P_companyId", companyId);
            parameters.Add("@P_IsLeviApplicable", IsLeviApplicable);

            var result = await _db.QueryAsync<InvoiceDetailsDto>(
                "usp_sbs_invoiceDetails_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var insertParams = new DynamicParameters();
            insertParams.Add("@P_invoiceNo", dto.InvoiceNo);
            insertParams.Add("@P_companyId", dto.CompanyId);
            insertParams.Add("@P_subcontractorId", dto.SubcontractorId);
            insertParams.Add("@P_productId", dto.ProductId);
            insertParams.Add("@P_invoiceDate", dto.InvoiceDate);
            insertParams.Add("@P_quantity", dto.Quantity);
            insertParams.Add("@P_unitAmount", dto.UnitAmount);
            insertParams.Add("@P_totalAmount", dto.TotalAmount);
            insertParams.Add("@P_commissionPercentage", dto.CommissionPercentage);
            insertParams.Add("@P_commissionAmount", dto.CommissionAmount);
            insertParams.Add("@P_paymentMode", dto.PaymentMode);
                //insertParams.Add("@P_status", dto.Status);
            insertParams.Add("@P_createdBy", dto.CreatedBy);
            insertParams.Add("@P_GroupNumber", dto.GroupNumber);
            insertParams.Add("@P_LRNumber", dto.LRNumber);
            insertParams.Add("@P_VehicleNumber", dto.VehicleNumber);
            insertParams.Add("@P_IsLeviApplicable", dto.IsLeviApplicable);
            insertParams.Add("@P_Levi", dto.Levi);
            insertParams.Add("@P_DocketNumber", dto.DocketNumber);
            insertParams.Add("@P_TrollyQuantity", dto.TrollyQuantity);
            insertParams.Add("@P_TrollyAmount", dto.TrollyAmount);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_invoiceDetails_insert",
                insertParams,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
            {
                throw new Exception($"Insert failed: {result?.R_ErrorMessage ?? "Unknown error"}");
            }

            if (result.R_Status == "SUCCESS")
            {
                return await GetInvoicesAsync(dto.IsLeviApplicable, dto.CompanyId, null);
            }

            throw new Exception($"Insert Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");
        }
        public async Task<List<InvoiceDetailsDto>> UpdateInvoiceAsync(UpdateInvoiceDto dto)
        {
            var updateParams = new DynamicParameters();
            updateParams.Add("@P_id", dto.Id);
            updateParams.Add("@P_invoiceNo", dto.InvoiceNo);
            updateParams.Add("@P_companyId", dto.CompanyId);
            updateParams.Add("@P_productId", dto.ProductId);
            updateParams.Add("@P_invoiceDate", dto.InvoiceDate);
            updateParams.Add("@P_quantity", dto.Quantity);
            updateParams.Add("@P_unitAmount", dto.UnitAmount);
            updateParams.Add("@P_totalAmount", dto.TotalAmount);
            updateParams.Add("@P_commissionPercentage", dto.CommissionPercentage);
            updateParams.Add("@P_commissionAmount", dto.CommissionAmount);
            updateParams.Add("@P_paymentMode", dto.PaymentMode);
            updateParams.Add("@P_status", dto.Status);
            updateParams.Add("@P_updatedBy", dto.UpdatedBy);
            updateParams.Add("@P_GroupNumber", dto.GroupNumber);
            updateParams.Add("@P_LRNumber", dto.LRNumber);
            updateParams.Add("@P_VehicleNumber", dto.VehicleNumber);

            //updateParams.Add("@P_IsLeviApplicable", dto.IsLeviApplicable);
            updateParams.Add("@P_Levi", dto.Levi);
            updateParams.Add("@P_DocketNumber", dto.DocketNumber);
            updateParams.Add("@P_TrollyQuantity", dto.TrollyQuantity);
            updateParams.Add("@P_TrollyAmount", dto.TrollyAmount);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_invoiceDetails_update",
                updateParams,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
            {
                throw new Exception($"Update failed: {result?.R_ErrorMessage ?? "Unknown error"}");
            }

            //int updatedId = updateResult.R_UpdatedID;

            //var getParams = new DynamicParameters();
            //getParams.Add("@P_id", updatedId);

            //var updatedInvoice = await _db.QueryFirstOrDefaultAsync<InvoiceDetailsDto>(
            //    "usp_sbs_invoiceDetails_get",
            //    getParams,
            //    commandType: CommandType.StoredProcedure
            //);
            if (result.R_Status == "SUCCESS")
            {
                return await GetInvoicesAsync(dto.IsLeviApplicable, dto.CompanyId, null);
            }

            throw new Exception($"Update Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");

        }

        //public async Task<InvoiceDetailsDto> DeleteInvoiceAsync(int id, int updatedBy)
        //{
        //    var deleteParams = new DynamicParameters();
        //    deleteParams.Add("@P_id", id);
        //    deleteParams.Add("@P_updatedBy", updatedBy);

        //    var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
        //        "usp_sbs_invoiceDetails_delete",
        //        deleteParams,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    if (result == null || result.R_Status != "Success")
        //        throw new Exception($"Delete failed: {result?.R_ErrorMessage ?? "Unknown error"}");

        //    // Bypass SP and fetch deleted invoice directly
        //    var sql = @"SELECT 
        //            id AS R_id,
        //            companyId AS R_companyId,
        //            subcontractorId AS R_subcontractorId,
        //            productId AS R_productId,
        //            invoiceDate AS R_invoiceDate,
        //            status AS R_status,
        //            quantity AS R_quantity,
        //            totalAmount AS R_totalAmount,
        //            paymentMode AS R_paymentMode
        //        FROM sbs_invoiceDetails
        //        WHERE id = @Id";

        //    return await _db.QueryFirstOrDefaultAsync<InvoiceDetailsDto>(sql, new { Id = id })
        //           ?? throw new Exception("Invoice not found after deletion.");
        //}

        public async Task<List<InvoiceDetailsDto>> DeleteInvoiceAsync(int id, int updatedBy,int companyId, bool isLeviApplicable)
        {
            var deleteParams = new DynamicParameters();
            deleteParams.Add("@P_id", id);
            deleteParams.Add("@P_updatedBy", updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_invoiceDetails_delete",
                deleteParams,
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Delete failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            if (result.R_Status == "SUCCESS")
            {
                return await GetInvoicesAsync(isLeviApplicable, companyId, null);
            }

            throw new Exception($"Update Failed: {result.R_ErrorMessage} (ErrorCode: {result.R_ErrorNumber})");

        }

        public async Task<List<InvoiceDetailsAsPerContractorDto>> GetInvoiceAsPerContractorAsync(
            int? invoiceId, int? subContractorId, DateTime? paymentDate, DateTime? fromDate, DateTime? toDate, bool isLeviApplicable = false)
        {
            var param = new DynamicParameters();
            param.Add("@P_InvoiceId", invoiceId);
            param.Add("@P_SubContractorId", subContractorId);
            param.Add("@P_PaymentDate", paymentDate);
            param.Add("@P_FromDate", fromDate);
            param.Add("@P_ToDate", toDate);
            param.Add("@P_IsLeviApplicable", isLeviApplicable);

            var result = await _db.QueryAsync<InvoiceDetailsAsPerContractorDto>(
                "usp_sbs_invoiceDetails_get_InvoiceAsPerContrator",
                param,
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }
    }
}
