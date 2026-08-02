using System;

namespace Client.Application.Features.Invoice.Dtos
{
    public class InvoiceDetailsAsPerContractorDto
    {
        public int R_id { get; set; }
        public string R_invoiceNo { get; set; }
        public int R_companyId { get; set; }
        public int R_subcontractorId { get; set; }
        public string R_subcontractorName { get; set; }
        public int R_productId { get; set; }
        public string R_productName { get; set; }
        public decimal R_unitPrice { get; set; }
        public decimal R_unitAmount { get; set; }
        public DateTime R_invoiceDate { get; set; }
        public string R_status { get; set; }
        public int R_quantity { get; set; }
        public decimal R_totalAmount { get; set; }
        public decimal R_commissionPercentage { get; set; }
        public decimal R_commissionAmount { get; set; }
        public string R_invoiceType { get; set; }
        public string R_GroupNumber { get; set; }
        public string R_LRNumber { get; set; }
        public string R_VehicleNumber { get; set; }
        public bool R_IsLeviApplicable { get; set; }
        public string R_Levi { get; set; }
        public string R_DocketNumber { get; set; }
        public int R_TrollyQuantity { get; set; }
        public decimal R_TrollyAmount { get; set; }
        public string R_invoiceNoSelect { get; set; }
        public string R_invoiceNoDateSelect { get; set; }
        public int createdBy { get; set; }
        public string username { get; set; }
        public string createdAt { get; set; }
    }
}
