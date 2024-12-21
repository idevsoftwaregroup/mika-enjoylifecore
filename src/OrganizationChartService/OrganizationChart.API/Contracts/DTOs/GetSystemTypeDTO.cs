using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetSystemTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
