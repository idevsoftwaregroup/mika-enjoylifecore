using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Menu
{
    public class MenuDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string? FontIcon { get; set; }
        public int Size { get; set; }
        public List<SubMenuDTO> ListItems { get; set; }
    }
}
