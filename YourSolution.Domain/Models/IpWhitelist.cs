using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.Models
{
    [Index(nameof(IpAddress))]
    public class IpWhitelist
    {
        public int Id { get; set; }

        public string IpAddress { get; set; } = string.Empty;

        public int Mask { get; set; } = 32;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;

        public bool IsEnable { get; set; } = true;
    }
}
