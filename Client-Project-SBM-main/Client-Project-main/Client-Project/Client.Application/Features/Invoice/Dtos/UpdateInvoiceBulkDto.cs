using System;
using System.Collections.Generic;

namespace Client.Application.Features.Invoice.Dtos
{
    /// <summary>
    /// DTO for bulk-updating multiple LR rows for an existing invoice.
    /// </summary>
    public class UpdateInvoiceBulkDto
    {
        public int       Id                   { get; set; }
        public string?   InvoiceNo            { get; set; }
        public int       CompanyId            { get; set; }
        public int       SubcontractorId      { get; set; }
        public int       ProductId            { get; set; }
        public DateTime  InvoiceDate          { get; set; }
        public decimal   CommissionPercentage { get; set; }
        public decimal   CommissionAmount     { get; set; }
        public string?   PaymentMode          { get; set; } = "CASH";
        public string?   Status               { get; set; }
        public int       UpdatedBy            { get; set; }
        public string?   GroupNumber          { get; set; }
        public string?   VehicleNumber        { get; set; }
        public bool      IsLeviApplicable     { get; set; }
        public string?   Levi                 { get; set; }
        public string?   DocketNumber         { get; set; }
        public int?      TrollyQuantity       { get; set; }
        public decimal?  TrollyAmount         { get; set; }

        /// <summary>LR / amount detail rows to update for this invoice.</summary>
        public List<LRItemDto> LRItems { get; set; } = new List<LRItemDto>();
    }
}
