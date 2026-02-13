using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.Models
{
    [Comment("記錄HttpRequest的相關資訊")]
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }

        [Unicode(false), MaxLength(10)]
        public string Method { get; set; }

        [Unicode(false)]
        public string Path { get; set; }

        [Unicode(false)]
        public string QueryString { get; set; }

        [Unicode(false)]
        public string Body { get; set; }

        [Unicode(false)]
        public string Headers { get; set; }
        public DateTime Timestamp { get; set; }

        // 新增 Response 欄位
        public int ResponseStatusCode { get; set; }

        [Unicode(false)]
        public string ResponseHeaders { get; set; }

        [Unicode(false)]
        public string ResponseBody { get; set; }

        public Guid UserFlowId { get; set; }
    }
}
