using System;

namespace JwtAuthAspNetCoreApi.Models
{
    public class BaseModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Version { get; set; }
    }
}
