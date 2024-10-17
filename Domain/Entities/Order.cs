using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Amount { get; set; }

        public int CustomerId { get; set; } // Foreign key

        // Navigation property
        public virtual Customer Customer { get; set; }
    }
}
