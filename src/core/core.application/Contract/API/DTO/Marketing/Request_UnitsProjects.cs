using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Request_UnitsProjects
    {
        public List<ProjectUnitsDetails> ProjectUnitsDetails {  get; set; }
    }
    public class ProjectUnitsDetails
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public string UnitFloor { get; set; }
        public string UnitBlock { get; set; }
        public int UnitArea { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitType { get; set; }
        public string UnitStatus { get; set; }
        public List<string> UnitFeatures { get; set; }
    }
}
