using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Request_ProjectBlocks
    {
        public List<ProjectBlockDetail> ProjectBlocksDetails { get; set; }
    }

    public class ProjectBlockDetail
    {
        public int Id { get; set; }
        public string BlockName { get; set; }
        public string BlockId { get; set; }
    }

}
