using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.TicketModels
{
    public class TicketStatusModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StatusCode { get; set; }
        [Required]
        [MaxLength(60)]
        public string Title { get; set; }
        [Required]
        [MaxLength(70)]
        public string DisplayTitle { get; set; }


        public virtual List<TicketModel> Tickets { get; set; }
        public virtual List<TicketLogModel> Logs { get; set; }
    }
}
