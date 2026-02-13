using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.DTOs.Response
{
    /// <summary>
    /// HttpStatusCode，API回傳的固定格式
    /// </summary>
    public class ResultDto
    {
        public bool IsOK { get; set; }
        public string Message { get; set; }
        public object Data { get; set; } = null;
    }
}
