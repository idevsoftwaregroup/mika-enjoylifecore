namespace OrganizationChart.API.Contracts.DTOs;

public class UpdateEmployeeRequestDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PersonalCode { get; set; }
    public int ManagerId { get; set; }
    public int Level { get; set; }
    //public int? ManagerId { get; set; }
    public int? DepartmentId { get; set; }
    public int TreeLevel { get; set; }
    public string ActivityLocation { get; set; }
    public string Role { get; set; }
    public string? PictureURL { get; set; }
    public DateTime EmploymentDate { get; set; }
}
