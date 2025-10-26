using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [NotNull, MaxLength(255, ErrorMessage = "Description length exceeding max length 255")]
        public string Description { get; set; }
        [NotNull]
        public decimal UnitPrice { get; set; }
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
