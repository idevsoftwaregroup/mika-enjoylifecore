using news.domain.Models.Aggregates.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.UserDTOs
{
    public class ViewerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Guid BuildingId { get; set; }

        public static explicit operator ViewerDTO(User user)
        {
            return new ViewerDTO()
            {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber.number,
                BuildingId = user.BuildingId
            };
        }
    }
}
