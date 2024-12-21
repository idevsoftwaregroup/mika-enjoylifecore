using ClosedXML.Excel;
using core.application.contract.api.DTO.Payment;
using core.application.Contract.API.DTO.Expense;
using core.application.Contract.API.DTO.Payment;
using core.application.Contract.API.Interfaces;
using core.application.Services;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Claims;
namespace core.api.Controller;

[CustomAuthorize]
[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IViewRenderService _viewRenderService;
    //private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaymentService _conext;


    private readonly PaymentModel _paymentModel;

    private readonly IConfiguration _configuration;

    //private readonly PaymentZarinGetURL _paymentZarinGetURL;
    public PaymentController(IPaymentService financialService, IViewRenderService viewRenderService, IPaymentService context, IConfiguration configuration)
    {
        _paymentService = financialService;
        _viewRenderService = viewRenderService;
        _configuration = configuration;
        
        //_httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("CreatePayment")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO paymentDetial)
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            if (userId > 0)
            {
                paymentDetial.createBy = userId;
                long paymentId = _paymentService.createPayment(paymentDetial);
                return Ok(paymentId);
            }
            else
            {
                return BadRequest("user not found");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetPayment/{paymentId}")]
    public async Task<IActionResult> GetPayment(long paymentId)
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            GetPaymentResponseDTO expense = _paymentService.getPayment(paymentId, userId);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("GetPaymentAdmin/{paymentId}")]
    [CustomAuthorize("FINANCE")]
    public async Task<IActionResult> GetPaymentAdmin(long paymentId)
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            GetPaymentResponseDTO expense = _paymentService.getPaymentAdmin(paymentId);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("GetPayments")]
    public async Task<IActionResult> GetPayments()
    {
        try
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            List<GetPaymentsDTO> expense = _paymentService.getPayments(userId);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("GetPaymentsAdmin")]
    [CustomAuthorize("FINANCE")]
    public async Task<IActionResult> GetPaymentsAdmin([FromQuery] GetAllPaymentsDTO dto)
    {
        var (Payments, TotalCount) = await _paymentService.GetPaymentsAdminAsync(dto);
        return Ok(new { Payments, TotalCount });
    }

    [AllowAnonymous]
    [HttpGet("GenerateExcel")]
    public async Task<IActionResult> GenerateExcel([FromQuery] GetAllPaymentsDTO dto)
    {
        var result = await _paymentService.GetExcelPaymentsAsync(dto);

        // Flatten the list of expenses and remove duplicates
        var expenses = result.Payments.SelectMany(payment => payment.expenses).Distinct().ToList();

        // Create a new Excel workbook
        using (var workbook = new XLWorkbook())
        {
            var paymentSheet = workbook.Worksheets.Add("Payments");
            var expenseSheet = workbook.Worksheets.Add("Expenses");

            // Add column headers for payments
            paymentSheet.Cell("A1").Value = "شناسه تراکنش";
            paymentSheet.Cell("B1").Value = "مبلغ کل";
            paymentSheet.Cell("C1").Value = "وضعیت";
            paymentSheet.Cell("D1").Value = "تاریخ پرداخت";
            paymentSheet.Cell("E1").Value = "ساعت پرداخت";
            paymentSheet.Cell("F1").Value = "پرداخت کننده";
            paymentSheet.Cell("G1").Value = "نوع پرداخت";
            paymentSheet.Cell("H1").Value = "کد پیگیری بانک";
            paymentSheet.Cell("I1").Value = "فایل آپلود شده";

            // Add data rows for payments
            int paymentRow = 2;
            foreach (var payment in result.Payments)
            {
                paymentSheet.Cell($"A{paymentRow}").Value = payment.Id;
                paymentSheet.Cell($"B{paymentRow}").Value = payment.totalAmount;
                paymentSheet.Cell($"C{paymentRow}").Value = payment.state.ToString();
                paymentSheet.Cell($"D{paymentRow}").Value = payment.paymentDate;
                paymentSheet.Cell($"E{paymentRow}").Value = payment.paymentTime ?? "";
                paymentSheet.Cell($"F{paymentRow}").Value = payment.PaymentBy ?? "";
                paymentSheet.Cell($"G{paymentRow}").Value = payment.paymentType.ToString();
                paymentSheet.Cell($"H{paymentRow}").Value = payment.bankVoucherId;
                paymentSheet.Cell($"I{paymentRow}").Value = payment.bankReciveImagePath != "" ? "https://app.enjoylife.ir/filestorage/" + payment.bankReciveImagePath : "";
                paymentRow++;
            }     // Add data rows for payments
            // (Code for payments omitted for brevity)

            // Add column headers for expenses
            expenseSheet.Cell("A1").Value = "شناسه هزینه";
            expenseSheet.Cell("B1").Value = "عنوان";
            expenseSheet.Cell("C1").Value = "مبلغ";
            expenseSheet.Cell("D1").Value = "شماره ثبت";
            expenseSheet.Cell("E1").Value = "توضیحات";
            expenseSheet.Cell("F1").Value = "تاریخ صدور";
            expenseSheet.Cell("G1").Value = "تاریخ سررسید";
            expenseSheet.Cell("H1").Value = "کد واحد";
            expenseSheet.Cell("I1").Value = "نام واحد";
            expenseSheet.Cell("J1").Value = "نوع هزینه";

            // Add data rows for expenses
            int expenseRow = 2;
            foreach (var expense in expenses)
            {

                expenseSheet.Cell($"A{expenseRow}").Value = expense.Id;
                expenseSheet.Cell($"B{expenseRow}").Value = expense.Title;
                expenseSheet.Cell($"C{expenseRow}").Value = expense.Amount;
                expenseSheet.Cell($"D{expenseRow}").Value = expense.RegisterNO;
                expenseSheet.Cell($"E{expenseRow}").Value = expense.Description;
                expenseSheet.Cell($"F{expenseRow}").Value = expense.IssueDateTime;
                expenseSheet.Cell($"G{expenseRow}").Value = expense.DueDate;
                expenseSheet.Cell($"H{expenseRow}").Value = expense.UnitId ?? 0;
                expenseSheet.Cell($"I{expenseRow}").Value = expense.UnitName;
                expenseSheet.Cell($"J{expenseRow}").Value = expense.Type switch
                {
                    ExpenseType.Escrow => "احتیاطی",
                    ExpenseType.Routin => "جاری",
                    ExpenseType.EnjoyLife => "انجوی لایف",
                    _ => "",// Handle default case if necessary
                };
                expenseRow++;
            }

            // Generate a unique file name
            var fileName = $"Excel_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            // Save workbook to memory stream
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Return Excel file as an IActionResult
            //return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            return this.File(
                    fileContents: memoryStream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                    // By setting a file download name the framework will
                    // automatically add the attachment Content-Disposition header
                    fileDownloadName: fileName
);
        }
    }


    [AllowAnonymous]
    [HttpPost("UploadExcelPayments")]
    public async Task<IActionResult> UploadExcelPayments(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded or file is empty.");
        }

        try
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var workbook = new XLWorkbook(stream))
                {
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
                        int createById = row.Cell(1).GetValue<int>();
                        DateTime createDate;
                        if (!DateTime.TryParse(row.Cell(2).GetString(), out createDate))
                        {
                            // Handle invalid date value
                            continue; // Skip this row and move to the next one
                        }
                        string paymentType = row.Cell(3).GetString();
                        DateTime paymentDate;
                        if (!DateTime.TryParse(row.Cell(4).GetString(), out paymentDate))
                        {
                            // Handle invalid date value
                            continue; // Skip this row and move to the next one
                        }
                        DateTime lastUpdateDate;
                        if (!DateTime.TryParse(row.Cell(5).GetString(), out lastUpdateDate))
                        {
                            // Handle invalid date value
                            continue; // Skip this row and move to the next one
                        }
                        string paymentState = row.Cell(6).GetString();
                        string bankVoucherId = row.Cell(7).GetString();
                        string bankReciveImagePath = row.Cell(8).GetString();
                        string description = row.Cell(9).GetString();
                        string accountId = row.Cell(10).GetString();

                        // Construct SQL INSERT command
                        string insertCommand = @"
                        INSERT INTO Payments (
                            createById, createDate, paymentType, paymentDate, lastUpdateDate, 
                            paymentState, bankVoucherId, bankReciveImagePath, Description, 
                             AccountId
                        ) VALUES (
                            @CreateById, @CreateDate, @PaymentType, @PaymentDate, @LastUpdateDate, 
                            @PaymentState, @BankVoucherId, @BankReciveImagePath, @Description, @AccountId
                        );";

                        // Execute the SQL command using your database connection
                        string connectionString = _configuration.GetConnectionString("default");
                        using (var connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();
                            using (var command = new SqlCommand(insertCommand, connection))
                            {
                                // Bind parameters
                                command.Parameters.AddWithValue("@CreateById", createById);
                                command.Parameters.AddWithValue("@CreateDate", createDate);
                                command.Parameters.AddWithValue("@PaymentType", paymentType);
                                command.Parameters.AddWithValue("@PaymentDate", paymentDate);
                                command.Parameters.AddWithValue("@LastUpdateDate", lastUpdateDate);
                                command.Parameters.AddWithValue("@PaymentState", paymentState);
                                command.Parameters.AddWithValue("@BankVoucherId", bankVoucherId);
                                command.Parameters.AddWithValue("@BankReciveImagePath", bankReciveImagePath);
                                command.Parameters.AddWithValue("@Description", description);
                                command.Parameters.AddWithValue("@AccountId", accountId);

                                // Execute the command
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }

            return Ok("File uploaded and data inserted into the database successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }




    [AllowAnonymous]
    [HttpGet("GenerateExcel2")]
    public IActionResult GenerateExcel2()
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Cell("A1").Value = "Name";
            worksheet.Cell("B1").Value = "Age";

            worksheet.Cell("A2").Value = "John";
            worksheet.Cell("B2").Value = 30;

            worksheet.Cell("A3").Value = "Alice";
            worksheet.Cell("B3").Value = 25;

            var fileName = $"Excel_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }

    [HttpPost]
    [Route("RegisterVoucherPayment")]
    public async Task<IActionResult> RegisterVoucherPayment([FromBody] RegisterPaymentVoucherDTO paymentDetial)
    {
        try
        {
            bool result = _paymentService.RegisterVoucherPayment(paymentDetial);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Route("RequestZarinPayment")]
    public async Task<ActionResult<string>> RequestZarinPayment([FromBody] PaymentZarinRequestDTO transactionDetial)
    {
        var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        transactionDetial.RequestBy = userId;
        var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        var configurations = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        transactionDetial.CallbackURL = baseUri + configurations["PaymentZarinGetURL:CallbackURL"];
        string transaction = _paymentService.RequestZarinPayment(transactionDetial);
        if (transaction != null)
        {
            return Ok(transaction);
        }
        else
        {
            return BadRequest("error ");
        }
    }

    [HttpGet]
    [Route("VerifyZarinPayment")]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyZarinPayment([FromQuery] string Authority)
    {
        PaymentZarinVerifyResponseDTO transaction = _paymentService.VerifyZarinPayment(Authority);
        if (transaction != null)
        {
            try
            {
                if (transaction.IsSuccess)
                {
                    var result = await _viewRenderService.RenderToStringAsync("~/Views/PaymentController/SuccessPaymentView.cshtml", transaction);
                    return Content(result, "text/html");
                }
                else
                {
                    var result = await _viewRenderService.RenderToStringAsync("~/Views/PaymentController/ErrorPaymentView.cshtml", transaction);
                    return Content(result, "text/html");
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return Content(message);
            }
        }
        else
        {
            return BadRequest("error ");
        }
    }

    [HttpGet("AllNotApproved")]
    public async Task<ActionResult<List<GetPaymentResponseDTO>>> GetNotApprovedPayments(bool? hasVoucher = null, bool? hasImage = null)
    {
        return Ok(await _paymentService.GetNotApprovedPayments(hasVoucher, hasImage));
    }

    //[HttpGet("GetByVoucherId")]
    //public async Task<ActionResult<GetPaymentDetailResponseDTO>> GetPaymentByVoucherId(string voucherId)
    //{

    //}
    [HttpPut("UpdatePayment/{paymentId}")]
    public async Task<ActionResult<GetPaymentResponseDTO>> UpdatePayment(long paymentId, UpdatePaymentRequestDTO requestDTO)
    {
        return Ok(_paymentService.UpdatePayment(paymentId, requestDTO));
    }
    [HttpPost("upload")]
    public async Task<ActionResult> UploadFile([FromForm] IFormFile formFile)
    {
        if (formFile == null || formFile == null)
        {
            return BadRequest("No file attached or uploaded !");
        }
         if (Path.GetExtension(formFile.FileName).ToLower() != ".xlsx")
        {
            return BadRequest("The Uploaded file is not Excel .");
        }

         return Ok("The Processed File is Excel .");
    }
}





