using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;

namespace Client.Domain.Models
{
    public class CompanyMaster
    {
        [Key]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull,MaxLength(4000,ErrorMessage = "Address length exceeding max length 4000")]
        public string Address { get; set; }
        [NotNull,RegularExpression("^[0-9]{10}$")]
        public string Phone { get; set; }
        [NotNull,EmailAddress]
        public string Email { get; set; }
        [NotNull]
        public int CreatedBy { get; set; }
        [NotNull]
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [NotNull]
        public int IsActive { get; set; } = 1;
        [NotNull]
        public int IsDeleted { get; set; } = 0;
    }
}
