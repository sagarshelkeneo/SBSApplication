namespace Client.Application.Features.SalaryDetails.Dtos
{
    public class UpdateSalaryDetailsDto
    {
        public int Id { get; set; }
        public string ToliNo { get; set; }
        public decimal Amount { get; set; }
        //public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int CompanyId { get; set; }
        public int BankId { get; set; }
        public int UpdatedBy { get; set; }
    }

}
