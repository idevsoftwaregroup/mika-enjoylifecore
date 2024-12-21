using OrganizationChart.API.Models;

namespace OrganizationChart.API.Contracts.DTOs;

public class GetDepartmentTreeResponseDTO
{
    public List<Employee> employees { get; set; }

    //public int DepartmentId { get; set; }
    //public string DepartmentName { get; set; }
    //public List<Node> Managers {  get; set; }

    //public class Node
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public List<Node> Children { get; set; } = new List<Node>();

    //    public static explicit operator Node(Employee employee)
    //    {
    //        return new Node
    //        {
    //            Id = employee.Id,
    //            Name = employee.Name,
    //        };
    //    }

        
    //}
}
