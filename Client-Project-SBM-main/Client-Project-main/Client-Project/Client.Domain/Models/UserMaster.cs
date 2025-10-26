using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Client.Domain.Models
{
    public class UserMaster
    {
        [Key]
        public int Id { get; set; }
        [NotNull]
        public int RoleMasterId { get; set; }
        [NotNull, MaxLength(50, ErrorMessage = "UserName length exceeding max length 50")]
        public string UserName { get; set; }
        [NotNull, MaxLength(255, ErrorMessage = "Password length exceeding max length 255")]
        public string Password { get; set; }
        [NotNull]
        public int CreatedBy { get; set; }
        [NotNull]
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public int? UpdatedAt { get; set; }
        [NotNull]
        public int IsActive { get; set; } = 1;
        [NotNull]
        public int IsDeleted { get; set; } = 0;
    }
}
