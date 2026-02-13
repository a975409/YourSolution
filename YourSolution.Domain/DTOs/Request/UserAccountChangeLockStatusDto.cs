using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.DTOs.Request
{
    /// <summary>
    /// 鎖定使用者資料
    /// </summary>
    public class UserAccountChangeLockStatusDto
    {
        public Guid FlowId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
