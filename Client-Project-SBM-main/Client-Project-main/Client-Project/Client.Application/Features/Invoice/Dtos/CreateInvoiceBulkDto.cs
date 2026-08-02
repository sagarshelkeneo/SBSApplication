using System.Collections.Generic;

namespace Client.Application.Features.Invoice.Dtos
{
    /// <summary>
    /// DTO for bulk-inserting multiple LR rows in a single stored procedure call.
    /// Shared booking fields are scalars; per-row data lives in LRItems.
    /// </summary>
    public class CreateInvoiceBulkDto
    {
        public string?  InvoiceNo             { get; set; }
        public int      CompanyId             { get; set; }
        public int      SubcontractorId       { get; set; }
        public int      ProductId             { get; set; }
        public System.DateTime InvoiceDate    { get; set; }
        public decimal  CommissionPercentage  { get; set; }
        public decimal  CommissionAmount      { get; set; }
        public string   PaymentMode           { get; set; } = "CASH";
        public int      CreatedBy             { get; set; }
        public string?  GroupNumber           { get; set; }
        public string?  VehicleNumber         { get; set; }
        public bool     IsLeviApplicable      { get; set; }
        public string?  Levi                  { get; set; }
        public string?  DocketNumber          { get; set; }
        public int?     TrollyQuantity        { get; set; }
        public decimal? TrollyAmount          { get; set; }

        /// <summary>LR / amount detail rows from the dynamic table.</summary>
        public List<LRItemDto> LRItems { get; set; } = new List<LRItemDto>();
    }

    /// <summary>One row in the Amount Details table.</summary>
    public class LRItemDto
    {
        public int      ProductId   { get; set; }
        public string?  LRNumber    { get; set; }
        public decimal  UnitAmount  { get; set; }
        public decimal  Quantity    { get; set; }
        public decimal  TotalAmount { get; set; }
    }
}
