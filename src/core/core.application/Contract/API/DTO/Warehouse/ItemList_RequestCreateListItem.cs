using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Warehouse
{
    public class ItemList_RequestCreateListItem
    {
        public int ItemListCode { get; set; }
        public string Group{ get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}
