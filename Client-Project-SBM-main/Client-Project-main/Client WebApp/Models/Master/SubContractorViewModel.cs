using Client.Application.Features.SubContractor.Dtos;

namespace Client_WebApp.Models.Master
{
    public class SubContractorViewModel
    {
        public int CompanyId { get; set; }

        public SubContractorDto SubContractor { get; set; }
        public List<SubContractorDto> SubContractors { get; set; }

        public SubContractorViewModel()
        {
            SubContractor = new SubContractorDto();
            SubContractors = new List<SubContractorDto>();
        }
    }

    public class SubContractor
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
    }

}
