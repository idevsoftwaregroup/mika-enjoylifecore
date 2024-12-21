namespace OrganizationChart.API.Models;

public class Employee
{
    public int Id { get;  }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PersonalCode { get; set; }
    public Employee? Manager { get; set; }
    public Department Department { get; set; }
    public string? PictureURL { get; set; }
    public string? ActivityLocation {  get; set; }
    public int TreeLevel { get; set; }
    public string Role { get; set; }
    public DateTime EmploymentDate { get; set; }
    public bool? IsDelete { get; set; } = false;

    private Employee()
    {

    }

    public Employee(Employee? manager, Department department)
    {
        if (manager is not null)
        {
            if (manager.Department.Id != department.Id) throw new ArgumentException("deparment of employee must be the same as its manager");
        }

        Manager = manager;
        Department = department;
    }
}
