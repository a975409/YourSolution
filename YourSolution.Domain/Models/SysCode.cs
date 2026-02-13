using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.Models
{
    [Index(nameof(CodeKind))]
    public class SysCode
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200), Unicode(false)]
        public string CodeKind { get; set; }

        [Required, MaxLength(200)]
        public string CodeName { get; set; }

        [Required, MaxLength(20)]
        public string CodeValue { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public DateTime UpdateTime { get; set; }
    }
}
