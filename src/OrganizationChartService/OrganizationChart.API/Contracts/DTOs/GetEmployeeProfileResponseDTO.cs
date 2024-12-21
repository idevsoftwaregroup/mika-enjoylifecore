using System.Globalization;

namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetEmployeeProfileResponseDTO
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalCode { get; set; }
        public string Role {  get; set; }
        public string EmploymentDate { get; set;}
        public string? PictureUrl { get; set; }
        public string ActivityLocation { get; set; }
        public static string ConvertEmploymentDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
        }
    }
}
