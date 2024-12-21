using Microsoft.EntityFrameworkCore;
using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Infrastructure.FileStorage;
using OrganizationChart.API.Infrastructure.Persistence;
using OrganizationChart.API.Models;

namespace OrganizationChart.API.Services;

public class OrganizationChartService : IOrganizationChartService
{
    private readonly OrganizationChartDataContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<OrganizationChartService> _logger;

    public OrganizationChartService(OrganizationChartDataContext context, IFileStorageService fileStorageService, ILogger<OrganizationChartService> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }
    public async Task<Department> CreateDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        Department department = new Department { Name = departmentName };
        await _context.Departments.AddAsync(department, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return department;

    }
    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {

        Department department = await _context.Departments.Where(d => d.Id == requestDTO.DepartmentId).SingleOrDefaultAsync(cancellationToken)
            ?? throw new Exception($"department with id {requestDTO.DepartmentId} was not found");

        Employee? manager = await _context.Employees.Where(e => e.Id == requestDTO.ManagerId).Include(e => e.Department).SingleOrDefaultAsync(cancellationToken)?? null;

        Employee employee = new(manager, department)
        {
            FirstName = requestDTO.FirstName,
            LastName = requestDTO.LastName,
            TreeLevel = (int)(requestDTO.TreeLevel is null ? (manager is null ? 0 : manager.TreeLevel + 1) : requestDTO.TreeLevel),
            Role = requestDTO.Role,
            PictureURL = requestDTO.PictureURL,
            PersonalCode = requestDTO.PersonalCode,
            EmploymentDate = requestDTO.EmploymentDate,
            ActivityLocation = requestDTO.ActivityLocation,
            Manager = manager,
            Department = department,
        };

        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return employee;

    }


    public async Task<Employee> UpdateEmployeeAsync(UpdateEmployeeRequestDTO requestDTO, int employeeId, CancellationToken cancellationToken = default)
    {
        Employee employee = await _context.Employees.Where(e => e.Id == employeeId).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"employee with id {employeeId} was not found");       

        Department department = await _context.Departments.Where(d => d.Id == requestDTO.DepartmentId).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception($"department with id {requestDTO.DepartmentId} was not found");

        employee.FirstName = requestDTO.FirstName;
        employee.LastName = requestDTO.LastName;
        employee.TreeLevel = requestDTO.TreeLevel;
        employee.Role = requestDTO.Role;
        employee.Manager = await _context.Employees.Where(e => e.Id == requestDTO.ManagerId).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"employee(manager) with id {requestDTO.ManagerId} was not found");
        employee.PictureURL=requestDTO.PictureURL;
        employee.PersonalCode = requestDTO.PersonalCode;
        employee.ActivityLocation = requestDTO.ActivityLocation;
        employee.EmploymentDate = requestDTO.EmploymentDate;
        _context.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee;
    }

    public async Task<Employee> UploadProfilePictureAsync(Stream stream, string fileName, string contentType, int employeeId, CancellationToken cancellationToken = default)
    {
        Employee employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == employeeId, cancellationToken) ?? throw new Exception($"employee with id {employeeId} was not found");

        employee.PictureURL = await _fileStorageService.SendProfilePictureToFileStorage(stream, fileName, contentType, employeeId, cancellationToken);

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee;
    }

    public async Task<List<Employee>> GetDepartmentTreeAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.SingleOrDefaultAsync(d => d.Id == departmentId, cancellationToken) ?? throw new Exception($"department with id {departmentId} was not found");

        var employees = await _context.Employees.Where(e => e.Department.Id == departmentId && e.IsDelete!= true).OrderBy(e => e.TreeLevel).ToListAsync(cancellationToken);

        return employees;

    }

    public async Task<(List<Employee> Employees, int TotalCount)> GetEmployeesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Employees.CountAsync(cancellationToken);

        var employees = await _context.Employees
            .OrderBy(e => e.TreeLevel)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (employees, totalCount);
    }

    public async Task<List<GetAllEmployesResponse>> GetAll(CancellationToken cancellationToken = default)
    {
        var listTempAllEmploy = _context
            .Employees
            .Include(e => e.Department)
            .OrderBy(e => e.Department.Id)
            .ThenBy(e => e.TreeLevel)
            .ToList();

        var grouped = listTempAllEmploy.GroupBy(e => e.Department);

        // Create a list of GetAllEmployesResponse objects
        var response = new List<GetAllEmployesResponse>();

        // Loop through each group
        foreach (var group in grouped)
        {
            // Create a new GetAllEmployesResponse object
            var item = new GetAllEmployesResponse();

            // Set the id and title properties from the department
            item.id = group.Key.Id.ToString();
            item.title = group.Key.Name;

            // Create a list of User objects
            item.users = new List<User>();

            // Loop through each employee in the group
            foreach (var employee in group)
            {
                // Create a new User object
                var user = new User();

                // Set the name, personnelNumber, role and imgUrl properties from the employee
                user.name = employee.FirstName + " " + employee.LastName;
                user.personnelNumber = employee.Id.ToString();
                user.role = employee.Role;
                user.PersonalCode = employee.PersonalCode;
                user.imgUrl = employee.PictureURL;
                //user.ActivityLocation = employee.ActivityLocation;
                user.ActivityLocation = employee.ActivityLocation;

                // Add the user to the list
                item.users.Add(user);
            }

            // Add the item to the response list
            response.Add(item);
        }
        return response;
    }

    public async Task<List<Department>> GetAllDepartment(CancellationToken cancellationToken = default)
    {
        return await _context
             .Departments
             .ToListAsync();
    }

    public async Task<Employee> GetEmployee(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context
            .Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .OrderBy(e => e.Department.Id)
            .ThenBy(e => e.TreeLevel)
            .SingleOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
    }



    public async Task DeleteEmployeeAsync(int employeeId, bool cascade = false, CancellationToken cancellationToken = default)
    {
        //Employee employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == employeeId, cancellationToken) ?? throw new Exception($"employee with id {employeeId} was not found");

        //if (!cascade && _context.Employees.Any(e => e.Manager != null && e.Manager.Id == employeeId)) throw new Exception("employee has reporting employees first delete them or set cascade true");

        //_context.Employees.Remove(employee);
        //await _context.SaveChangesAsync(cancellationToken);
        Employee employee = await _context.Employees.Where(e => e.Id == employeeId).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"employee with id {employeeId} was not found");

        if (!cascade && _context.Employees.Any(e => e.Manager != null && e.Manager.Id == employeeId)) throw new Exception("employee has reporting employees first delete them or set cascade true");

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }



    public async Task DeleteDepartmentAsync(int departmentId, bool cascade = false, CancellationToken cancellationToken = default)
    {
        Department department =
            await _context.Departments.SingleOrDefaultAsync(d => d.Id == departmentId, cancellationToken)
            ?? throw new Exception($"department with id {departmentId} was not found");

        if (!cascade && _context.Employees.Any(e => e.Department.Id == departmentId)) throw new Exception("this department has employees, delete them or set cascade true");

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync(cancellationToken);

    }

    public async Task<GetEmployeeProfileResponseDTO> GetEmployeeProfile(int employeeId, CancellationToken cancellationToken)
    {
        var emp = await _context.Employees.SingleOrDefaultAsync(e => e.Id == employeeId, cancellationToken: cancellationToken) ?? throw new Exception($"emplyee with id {employeeId} was not found");
        
        return new GetEmployeeProfileResponseDTO()
        {
            Id = employeeId,
            EmploymentDate = GetEmployeeProfileResponseDTO.ConvertEmploymentDate(emp.EmploymentDate),
            FirstName = emp.FirstName, LastName = emp.LastName,
            PersonalCode = emp.PersonalCode == "" ? "ندارد" : emp.PersonalCode,
            Role = emp.Role,
            PictureUrl = emp.PictureURL,
            ActivityLocation = emp.ActivityLocation,
        };
    }
}
