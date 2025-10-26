using System.Xml;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Client.Domain.Models
{
    public class RoleMaster
    {
        [Key]
        public int Id { get; set; }
        [NotNull, MaxLength(50, ErrorMessage = "RoleName length exceeding max length 50")]
        public string RoleName { get; set; }
        [NotNull, MaxLength(4000, ErrorMessage = "Description length exceeding max length 4000")]
        public string Description { get; set; }
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
