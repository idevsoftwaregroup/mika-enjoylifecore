using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.structureModels
{
    public class MenuModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(60)]
        public string Title { get; set; }
        [MaxLength(35)]
        public string Icon { get; set; }
        [MaxLength(80)]
        public string Url { get; set; }
        [MaxLength(35)]
        public string FontIcon { get; set; }
        public int SizeInPixel { get; set; }
        public int? ParentId { get; set; }
        public virtual List<RoleMenuModel> RoleMenus { get; set; }
        public virtual MenuModel? SelfMenu { get; set; }
    }
}
