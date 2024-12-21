using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Models;

namespace OrganizationChart.API.Contracts.Interfaces
{
    public interface IOrganizationChartService
    {
        Task<Department> CreateDepartmentAsync(string departmentName, CancellationToken cancellationToken =default);
        Task<Employee> CreateEmployeeAsync(CreateEmployeeRequestDTO requestDTO, CancellationToken cancellationToken = default);
        
        Task</*GetDepartmentTreeResponseDTO*/List<Employee>> GetDepartmentTreeAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<Employee> UpdateEmployeeAsync(UpdateEmployeeRequestDTO requestDTO, int employeeId, CancellationToken cancellationToken = default);
        
        Task DeleteDepartmentAsync(int departmentId, bool cascade = false, CancellationToken cancellationToken = default);
        Task DeleteEmployeeAsync(int employeeId, bool cascade = false, CancellationToken cancellationToken = default);
        Task<Employee> UploadProfilePictureAsync(Stream stream, string fileName, string contentType, int employeeId, CancellationToken cancellationToken);
        Task<List<GetAllEmployesResponse>> GetAll(CancellationToken cancellationToken = default);
        Task<(List<Employee> Employees, int TotalCount)> GetEmployeesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<Employee> GetEmployee(int employeeId, CancellationToken cancellationToken = default);
        Task<List<Department>> GetAllDepartment(CancellationToken cancellationToken = default);
        Task<GetEmployeeProfileResponseDTO> GetEmployeeProfile(int employeeId, CancellationToken cancellationToken);
    }
}