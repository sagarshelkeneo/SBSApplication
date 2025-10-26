using Client.Application.Features.Bank.Dtos;

namespace Client_WebApp.Models.Master
{
    public class BankViewModel
    {
        public int CompanyId { get; set; }
        public BankMasterDto Bank { get; set; }
        public List<BankMasterDto> Banks { get; set; }

        public BankViewModel()
        {
            Bank = new BankMasterDto();
            Banks = new List<BankMasterDto>();
        }
    }

    public class Bank
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
    }
}
