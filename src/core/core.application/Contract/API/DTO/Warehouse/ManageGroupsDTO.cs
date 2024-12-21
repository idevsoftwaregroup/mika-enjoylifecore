using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Warehouse
{
    public class ManageGroupsDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
