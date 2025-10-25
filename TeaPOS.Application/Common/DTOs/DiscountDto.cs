using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaPOS.Application.Common.DTOs
{
    public sealed class DiscountDto
    {
        public int DiscountID { get; set; }
        public string Code { get; set; } = "";
        public decimal Percentage { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
