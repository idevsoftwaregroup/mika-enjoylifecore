using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Models;

namespace OrganizationChart.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationChartController : ControllerBase
{
    private readonly IOrganizationChartService _organizationChartService;
    

    public OrganizationChartController(IOrganizationChartService organizationChartService)
    {
        _organizationChartService = organizationChartService;
        
    }

    [HttpGet("Employee/{employeeId}/Profile")]
    public async Task<ActionResult<GetEmployeeProfileResponseDTO>> GetEmployeeProfile(int employeeId, CancellationToken cancellationToken = default)
    {
        if (employeeId == 0)
        {
            return BadRequest("No Employye ID Found !");
        }
        return Ok(await _organizationChartService.GetEmployeeProfile(employeeId, cancellationToken));
    }

    [HttpPost("Department")]
    public async Task<ActionResult<Department>> CreateDepartment(string departmentName, CancellationToken cancellationToken = default)
    {      
        return Ok(await _organizationChartService.CreateDepartmentAsync(departmentName, cancellationToken));
    }

    [HttpPost("Employee")]
    public async Task<ActionResult<Employee>> CreateEmployeeAsync(CreateEmployeeRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        return Ok(await _organizationChartService.CreateEmployeeAsync(requestDTO, cancellationToken));
    }

    [HttpPut("Employee/{employeeId}")]
    public async Task<ActionResult<Employee>> UpdateEmployeeAsync(UpdateEmployeeRequestDTO requestDTO, int employeeId, CancellationToken cancellationToken = default)
    {
        return Ok(await _organizationChartService.UpdateEmployeeAsync(requestDTO,employeeId,cancellationToken));

    }

    [HttpGet("Employee/{departmentId}")]
    public async Task<ActionResult<List<Employee>>> GetDepartmentTreeAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        var result = await _organizationChartService.GetDepartmentTreeAsync(departmentId, cancellationToken);

        return result;
    }

    [HttpGet("Employees")]
    public async Task<ActionResult> GetEmployees(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _organizationChartService.GetEmployeesAsync(pageNumber, pageSize, cancellationToken);
        return Ok(new { Employees = result.Employees, TotalCount = result.TotalCount });
        //return result;
    }

    [HttpGet("All")]
    public async Task<ActionResult<List<GetAllEmployesResponse>>> GetAllEmployee(CancellationToken cancellationToken = default)
    {
        
        return Ok(await _organizationChartService.GetAll(cancellationToken));

    }
    [HttpGet("GetAllDepartment")]
    public async Task<ActionResult<List<Department>>> GetAllDepartment(CancellationToken cancellationToken = default)
    {
        return Ok(await _organizationChartService.GetAllDepartment(cancellationToken));
    }
    [HttpGet("EmployeeDetail/{employeeId}")]
    public async Task<ActionResult<Employee>> GetEmployee(int employeeId,CancellationToken cancellationToken = default)
    {
        return Ok(await _organizationChartService.GetEmployee(employeeId,cancellationToken));
    }

    [HttpPost("UploadProfilePicture")]
    public async Task<ActionResult<Employee>> UploadProfilePicture([FromQuery]int employeeId,IFormFile formFile, CancellationToken cancellationToken = default)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using Stream stream = formFile.OpenReadStream();

        return Ok(await _organizationChartService.UploadProfilePictureAsync(stream, formFile.FileName, formFile.ContentType, employeeId, cancellationToken));
    }

    [HttpDelete("Employee/{employeeId}")]
    public async Task<IActionResult> DeleteEmployeeAsync(int employeeId, [FromQuery] bool cascade = false, CancellationToken cancellationToken = default)
    {
        await _organizationChartService.DeleteEmployeeAsync(employeeId, cascade, cancellationToken);
        return NoContent();
    }

    [HttpDelete("Department/{departmentId}")]
    public async Task<IActionResult> DeleteDepartmentAsync(int departmentId, [FromQuery] bool cascade = false, CancellationToken cancellationToken = default)
    {
        await _organizationChartService.DeleteDepartmentAsync(departmentId, cascade, cancellationToken);

        return NoContent();

    }

}