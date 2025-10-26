using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.PaymentReports.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _db;

        public ReportRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<PaidReportDto>> GetPaidReportAsync(string? subcontractorName,int? companyId, string? bankName, string fromDate,string toDate)
        {
            var param = new DynamicParameters();
            param.Add("@p_subcontractorName", subcontractorName);
            //param.Add("@p_companyID", companyId);
            param.Add("@p_bankName", bankName);
            param.Add("@p_fromDate", fromDate);
            param.Add("@p_toDate",toDate);

            var result = await _db.QueryAsync<PaidReportDto>("sp_PaidBalancePaymentReport", param, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<UnpaidReportDto>> GetUnpaidReportAsync(string? subcontractorName, int? companyId, string fromDate, string toDate)
        {
            var param = new DynamicParameters();
            param.Add("@p_subcontractorName", subcontractorName);
            //param.Add("p_companyID", companyId);
            param.Add("@p_fromDate", fromDate);
            param.Add("@p_toDate", toDate);

            var result = await _db.QueryAsync<UnpaidReportDto>("sp_UnPaidBalancePaymentReport", param, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<ProductWiseReportDto>> GetProductWiseReportAsync(string? productName,string? subcontractorName, int? companyId, string fromDate,string toDate)
        {
            var param = new DynamicParameters();
            param.Add("@p_productName", productName);
            param.Add("@p_subcontractorName", subcontractorName);
            //param.Add("@p_companyID", companyId);
            param.Add("@p_fromDate", fromDate);
            param.Add("@p_toDate", toDate);

            var result = await _db.QueryAsync<ProductWiseReportDto>("sp_ProductWisePayment", param, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<SubcontractorWiseReportDto>> GetSubcontractorWiseReportAsync(string? subcontractorName, int? companyId, string fromDate, string toDate)
        {
            var param = new DynamicParameters();
            param.Add("@p_subcontractorName", subcontractorName);
            param.Add("@p_fromDate", fromDate);
            param.Add("@p_toDate", toDate);
            //param.Add("@p_companyId", companyId);

            var result = await _db.QueryAsync<SubcontractorWiseReportDto>("sp_MonthlyPaymentSubcontractorWiseTotalPayment", param, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        public async Task<List<CombinedSubcontractorReportDto>> GetCombinedSubcontractorReportAsync()
        {
            var result = await _db.QueryAsync<CombinedSubcontractorReportDto>(
                "sp_CombinedSubcontractorEntityReport",
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }

}
