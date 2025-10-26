using Client.Application.Features.Product.Dtos;

namespace Client_WebApp.Models.Config
{

    public class RoleDetails
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
    public class RoleViewModel
    {
        public int CompanyId { get; set; }

        public RoleDetails? RoleDto { get; set; }
        public List<RoleDetails>? RoleDtos { get; set; }

        public RoleViewModel()
        {
            RoleDto = new RoleDetails();
            RoleDtos = new List<RoleDetails>();
        }
    }
}
