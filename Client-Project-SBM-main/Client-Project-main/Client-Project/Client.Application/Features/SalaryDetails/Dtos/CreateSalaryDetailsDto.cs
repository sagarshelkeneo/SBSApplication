namespace Client.Application.Features.SalaryDetails.Dtos
{
    public class CreateSalaryDetailsDto
    {
        public string ToliNo { get; set; }
        public decimal Amount { get; set; }
        public int BankId { get; set; }
        public DateTime Date { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
    }
}
