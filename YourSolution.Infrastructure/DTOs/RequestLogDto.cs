using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.DTOs
{
    public class RequestLogDto
    {
        public string Method { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string Body { get; set; }

        public string Headers { get; set; }

        // 新增 Response 欄位
        public int ResponseStatusCode { get; set; }

        public string ResponseHeaders { get; set; }

        public string ResponseBody { get; set; }

        public Guid UserFlowId { get; set; }
    }
}
