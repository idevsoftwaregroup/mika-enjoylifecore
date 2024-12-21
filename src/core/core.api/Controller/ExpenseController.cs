using ClosedXML.Excel;
using core.application.Contract.API.DTO.Expense;
using core.application.Contract.API.Interfaces;
using core.application.Framework;
using core.application.Services;
using core.domain.DomainModelDTOs.ExpenseDTOs;
using core.domain.entity.financialModels.valueObjects;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Security.Claims;

namespace core.api.Controller;

[CustomAuthorize]
[Route("api/[controller]")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _financialService;
    private readonly IConfiguration _configuration;
    public ExpenseController(IExpenseService financialService, IConfiguration configuration)
    {
        _financialService = financialService;
        _configuration = configuration;
    }
    [HttpPost("CreateExpense")]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO expenseValue)
    {
        try
        {
            long expenseId = _financialService.CreateExpense(expenseValue);
            return Ok(expenseId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("GetExpense/{expenseId}")]
    public async Task<IActionResult> GetExpense(long expenseId)
    {
        try
        {
            GetExpenseResponseDTO expense = _financialService.GetExpense(expenseId);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("GetExpensesByUserId")]
    public async Task<IActionResult> GetExpensesByUserId()
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            List<GetExpenseResponseDTO> expenses = _financialService
                .GetExpenseByFilter(new GetExpenseRequestDTO() { UserId = userId });
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("GetExpensesByUserUnits")]
    public async Task<IActionResult> GetExpensesByUserUnits()
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            List<GetExpenseResponseDTO> expenses = await _financialService.GetExpenseByFilterUnits(new GetExpenseRequestDTO() { UserId = userId });
            return Ok(expenses);
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new List<GetExpenseResponseDTO>());
        }
    }


    [HttpPost("GetExpensesByFilter")]
    public async Task<IActionResult> GetExpensesByFilter([FromBody] GetExpenseRequestDTO filter)
    {
        try
        {
            List<GetExpenseResponseDTO> expenses = _financialService.GetExpenseByFilter(filter);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("GetExpensesByAdmin")]
    public async Task<IActionResult> GetExpensesByAdmin([FromBody] GetExpenseFilterRequestDTO dto)
    {
        var result = _financialService.GetExpensesByAdmin(dto);
        return Ok(new { Expenses = result.Expenses, TotalCount = result.TotalCount });
    }


    [HttpGet("GetTotalExpense")]
    public async Task<IActionResult> GetTotalExpense()
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            GetTotalExpenseResponseDTO expenses = _financialService.GetTotalExpense(userId);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("GetTotalExpenseUnits")]
    public async Task<IActionResult> GetTotalExpenseUnits()
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            GetTotalExpenseResponseDTO expenses = _financialService.GetTotalExpenseUnits(userId);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GetTotalExpenseUnits
    [HttpPost("GetTotalExpenseWithDetials")]
    public async Task<IActionResult> GetTotalExpenseWithDetials([FromBody] GetExpenseRequestDTO filter)
    {
        try
        {
            GetTotalExpenseWithDetailsResponseDTO expenses = _financialService.GetTotalExpenseWithDetails(filter);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPost("UploadExpensesExcel")]
    public async Task<ActionResult<OperationResult<object>>> UploadExpensesExcel([FromForm] ExcelFileDTO model, CancellationToken cancellationToken = default)
    {
        var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
        if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "FINANCE"))
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UploadExpensesExcel").Failed("دسترسی ثبت گروهی هزینه برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
        }
        var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        OperationResult<object> Op = new("UploadExpensesExcel");
        if (model.File == null || model.File.Length == 0)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, Op.Failed("فایلی برای بارگزاری ارسال نشده است"));
        }
        if (Path.GetFileName(model.File.FileName).ToLower().Contains(".asax") || Path.GetFileName(model.File.FileName).ToLower().Contains(".ascx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".ashx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".asmx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".aspx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".axd") || Path.GetFileName(model.File.FileName).ToLower().Contains(".browser") || Path.GetFileName(model.File.FileName).ToLower().Contains(".cd") || Path.GetFileName(model.File.FileName).ToLower().Contains(".php") || Path.GetFileName(model.File.FileName).ToLower().Contains(".compile") || Path.GetFileName(model.File.FileName).ToLower().Contains(".config") || Path.GetFileName(model.File.FileName).ToLower().Contains(".cs") || Path.GetFileName(model.File.FileName).ToLower().Contains(".jsl") || Path.GetFileName(model.File.FileName).ToLower().Contains(".vb") || Path.GetFileName(model.File.FileName).ToLower().Contains(".csproj") || Path.GetFileName(model.File.FileName).ToLower().Contains(".vbproj") || Path.GetFileName(model.File.FileName).ToLower().Contains(".vjsproj") || Path.GetFileName(model.File.FileName).ToLower().Contains(".disco") || Path.GetFileName(model.File.FileName).ToLower().Contains(".vsdisco") || Path.GetFileName(model.File.FileName).ToLower().Contains(".dsdgm") || Path.GetFileName(model.File.FileName).ToLower().Contains(".dsprototype") || Path.GetFileName(model.File.FileName).ToLower().Contains(".dll") || Path.GetFileName(model.File.FileName).ToLower().Contains(".licx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".webinfo") || Path.GetFileName(model.File.FileName).ToLower().Contains(".master") || Path.GetFileName(model.File.FileName).ToLower().Contains(".mdb") || Path.GetFileName(model.File.FileName).ToLower().Contains(".ldb") || Path.GetFileName(model.File.FileName).ToLower().Contains(".mdf") || Path.GetFileName(model.File.FileName).ToLower().Contains(".msgx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".svc") || Path.GetFileName(model.File.FileName).ToLower().Contains(".rem") || Path.GetFileName(model.File.FileName).ToLower().Contains(".resources") || Path.GetFileName(model.File.FileName).ToLower().Contains(".resx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".sdm") || Path.GetFileName(model.File.FileName).ToLower().Contains(".sdmDocument") || Path.GetFileName(model.File.FileName).ToLower().Contains(".sitemap") || Path.GetFileName(model.File.FileName).ToLower().Contains(".skin") || Path.GetFileName(model.File.FileName).ToLower().Contains(".sln") || Path.GetFileName(model.File.FileName).ToLower().Contains(".soap") || Path.GetFileName(model.File.FileName).ToLower().Contains(".asa") || Path.GetFileName(model.File.FileName).ToLower().Contains(".asp") || Path.GetFileName(model.File.FileName).ToLower().Contains(".cdx") || Path.GetFileName(model.File.FileName).ToLower().Contains(".cer") || Path.GetFileName(model.File.FileName).ToLower().Contains(".idc") || Path.GetFileName(model.File.FileName).ToLower().Contains(".shtm") || Path.GetFileName(model.File.FileName).ToLower().Contains(".shtml") || Path.GetFileName(model.File.FileName).ToLower().Contains(".stm") || Path.GetFileName(model.File.FileName).ToLower().Contains(".css") || Path.GetFileName(model.File.FileName).ToLower().Contains(".htm") || Path.GetFileName(model.File.FileName).ToLower().Contains(".html") || Path.GetFileName(model.File.FileName).ToLower().Contains(".perl") || Path.GetExtension(model.File.FileName).ToLower().Contains(".asax") || Path.GetExtension(model.File.FileName).ToLower().Contains(".ascx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".ashx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".asmx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".aspx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".axd") || Path.GetExtension(model.File.FileName).ToLower().Contains(".browser") || Path.GetExtension(model.File.FileName).ToLower().Contains(".cd") || Path.GetExtension(model.File.FileName).ToLower().Contains(".php") || Path.GetExtension(model.File.FileName).ToLower().Contains(".compile") || Path.GetExtension(model.File.FileName).ToLower().Contains(".config") || Path.GetExtension(model.File.FileName).ToLower().Contains(".cs") || Path.GetExtension(model.File.FileName).ToLower().Contains(".jsl") || Path.GetExtension(model.File.FileName).ToLower().Contains(".vb") || Path.GetExtension(model.File.FileName).ToLower().Contains(".csproj") || Path.GetExtension(model.File.FileName).ToLower().Contains(".vbproj") || Path.GetExtension(model.File.FileName).ToLower().Contains(".vjsproj") || Path.GetExtension(model.File.FileName).ToLower().Contains(".disco") || Path.GetExtension(model.File.FileName).ToLower().Contains(".vsdisco") || Path.GetExtension(model.File.FileName).ToLower().Contains(".dsdgm") || Path.GetExtension(model.File.FileName).ToLower().Contains(".dsprototype") || Path.GetExtension(model.File.FileName).ToLower().Contains(".dll") || Path.GetExtension(model.File.FileName).ToLower().Contains(".licx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".webinfo") || Path.GetExtension(model.File.FileName).ToLower().Contains(".master") || Path.GetExtension(model.File.FileName).ToLower().Contains(".mdb") || Path.GetExtension(model.File.FileName).ToLower().Contains(".ldb") || Path.GetExtension(model.File.FileName).ToLower().Contains(".mdf") || Path.GetExtension(model.File.FileName).ToLower().Contains(".msgx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".svc") || Path.GetExtension(model.File.FileName).ToLower().Contains(".rem") || Path.GetExtension(model.File.FileName).ToLower().Contains(".resources") || Path.GetExtension(model.File.FileName).ToLower().Contains(".resx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".sdm") || Path.GetExtension(model.File.FileName).ToLower().Contains(".sdmDocument") || Path.GetExtension(model.File.FileName).ToLower().Contains(".sitemap") || Path.GetExtension(model.File.FileName).ToLower().Contains(".skin") || Path.GetExtension(model.File.FileName).ToLower().Contains(".sln") || Path.GetExtension(model.File.FileName).ToLower().Contains(".soap") || Path.GetExtension(model.File.FileName).ToLower().Contains(".asa") || Path.GetExtension(model.File.FileName).ToLower().Contains(".asp") || Path.GetExtension(model.File.FileName).ToLower().Contains(".cdx") || Path.GetExtension(model.File.FileName).ToLower().Contains(".cer") || Path.GetExtension(model.File.FileName).ToLower().Contains(".idc") || Path.GetExtension(model.File.FileName).ToLower().Contains(".shtm") || Path.GetExtension(model.File.FileName).ToLower().Contains(".shtml") || Path.GetExtension(model.File.FileName).ToLower().Contains(".stm") || Path.GetExtension(model.File.FileName).ToLower().Contains(".css") || Path.GetExtension(model.File.FileName).ToLower().Contains(".htm") || Path.GetExtension(model.File.FileName).ToLower().Contains(".html") || Path.GetExtension(model.File.FileName).ToLower().Contains(".perl"))
        {
            return StatusCode((int)HttpStatusCode.Forbidden, Op.Failed("فرمت مجاز فایل xls ، xlsx ، xltx ، xlsb و xlsm است"));
        }
        if (Path.GetExtension(model.File.FileName) != ".xls" && Path.GetExtension(model.File.FileName) != ".xlsx" && Path.GetExtension(model.File.FileName) != ".xltx" && Path.GetExtension(model.File.FileName) != ".xlsb" && Path.GetExtension(model.File.FileName) != ".xlsm")
        {
            return StatusCode((int)HttpStatusCode.Forbidden, Op.Failed("فرمت مجاز فایل xls ، xlsx ، xltx ، xlsb و xlsm است"));
        }
        try
        {
            using MemoryStream stream = new();
            var requestList = new List<Request_CreateExpenseDomainDTO>();

            await model.File.CopyToAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1); // Assuming the data is in the first worksheet

            // Define the start and end row indexes based on your Excel sheet structure
            int startRow = 2; // Assuming the first row contains headers
            int endRow = worksheet.RowsUsed().Count();

            // Iterate through each row in the range
            for (int rowNum = startRow; rowNum <= endRow; rowNum++)
            {
                // Read data from Excel row
                var row = worksheet.Row(rowNum);

                // Extract values from the Excel row, with error handling for types
                string? Title = row.Cell(1).GetValue<string?>();
                decimal Amount = row.Cell(2).GetValue<decimal>();
                string? RegisterNO = row.Cell(3).GetValue<string?>();
                string? Description = row.Cell(4).GetValue<string?>();
                string DueDateString = row.Cell(5).GetValue<string>();
                int Type = row.Cell(6).GetValue<int>();
                string UnitName = row.Cell(7).GetValue<string>();
                //string PhoneNumber = row.Cell(8).GetValue<string>(); // disabled by demand of Mr. Chegini in date 27/11/2024
                int IsPaid = row.Cell(8).GetValue<int>();

                requestList.Add(new Request_CreateExpenseDomainDTO
                {
                    Amount = Amount,
                    Description = Description,
                    DueDate = string.IsNullOrEmpty(DueDateString) || string.IsNullOrWhiteSpace(DueDateString) ? null : DueDateString.ToGregorianDate().ResetTime(),
                    IsPaid = IsPaid == 1 ? true : IsPaid == 0 ? false : null,
                    RegisterNO = RegisterNO,
                    Row = rowNum,
                    Title = Title,
                    Type = (ExpenseType)Type,
                    UnitName = UnitName,
                    //PhoneNumber = PhoneNumber,
                });
            }

            if (requestList.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, Op.Failed("اطلاعاتی برای ثبت ارسال نشده است"));
            }
            var operation = await _financialService.CreateExpenses(userId, requestList, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, Op.Failed("ثبت گروهی هزینه ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError));
        }
    }

}

