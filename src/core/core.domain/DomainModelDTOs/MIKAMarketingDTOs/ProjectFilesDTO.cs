using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.MIKAMarketingDTOs
{
    public class ProjectFilesDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
