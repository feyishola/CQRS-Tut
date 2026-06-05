using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Decimal TotalCost { get; set; }
    }
}