namespace Client.Application.Features.SalaryDetails.Dtos
{
    public class SalaryDetailsDto
    {
        public int R_id { get; set; }
        public string R_toliNo { get; set; }
        public decimal R_amount { get; set; }
        public int R_bankId { get; set; }
        public DateTime R_date { get; set; }
        public int R_companyId { get; set; }
        public string R_bankName { get; set; }
    }
}
