using ClosedXML.Excel;
using core.api.Contract.BillDTO;
using core.application.Contract.API.Interfaces;
using core.application.Framework;
using core.domain.DomainModelDTOs.BillDTOs;
using core.domain.DomainModelDTOs.BillDTOs.Filter;
using core.domain.DomainModelDTOs.TicketingDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        public BillController(IBillService billService)
        {
            this._billService = billService;
        }

        [HttpGet("GetBillInitialFilterData")]
        public async Task<ActionResult<OperationResult<Response_InitialFilterDataDTO>>> GetBillInitialFilterData(CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "FINANCE"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_InitialFilterDataDTO>("GetBillInitialFilterData").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _billService.GetBillInitialFilterData(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }


        [HttpGet("GetBillTotalReport")]
        public async Task<ActionResult<OperationResult<Response_GetBillTotalReportDTO>>> GetBillTotalReport([FromQuery] Filter_GetBillTotalReportDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "FINANCE"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetBillTotalReportDTO>("GetBillTotalReport").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _billService.GetBillTotalReport(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }


        [HttpGet("GetBillDetailReport/{UnitId}")]
        public async Task<ActionResult<OperationResult<Response_GetBillDetailReportDTO>>> GetBillDetailReport(int UnitId, [FromQuery] Filter_GetBillDetailReportDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "FINANCE"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetBillDetailReportDTO>("GetBillDetailReport").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _billService.GetBillDetailReport(UnitId, filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }


        [HttpPut("ModifyBillsByExcelFile")]
        public async Task<ActionResult<OperationResult<Response_ModifyListBillDTO>>> ModifyBillsByExcelFile([FromForm] Request_ModifyBillExcelFileDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_ModifyListBillDTO> Op = new("ModifyBillsByExcelFile");
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "FINANCE"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, Op.Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
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
                await model.File.CopyToAsync(stream, cancellationToken);
                stream.Seek(0, SeekOrigin.Begin);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var requestList = worksheet.RowsUsed()
                    .Where(x => (x.Cell(1).GetValue<string?>()).IsNotNull() &&
                                x.Cell(1).GetValue<string>().IsNumeric(false) &&
                                (x.Cell(2).GetValue<string?>()).IsNotNull() &&
                                x.Cell(2).GetValue<string>().StartsWith("واحد") &&
                                x.Cell(2).GetValue<string>().Split(" ").Length == 4
                                )
                    .Select(row => new Request_ModifyListBillDTO
                    {
                        RowNumber = row.RowNumber(),
                        Year = Convert.ToInt32(row.Cell(1).GetValue<string>()[..4]),
                        Month = Convert.ToInt32(row.Cell(1).GetValue<string>()[^2..]),
                        UnitName = row.Cell(2).GetValue<string>().Split(" ")[1].MultipleSpaceRemoverTrim(),
                        Description = row.Cell(3).GetValue<string?>().IsNotNull() ? row.Cell(3).GetValue<string>() : string.Empty,
                        Debit = row.Cell(4).GetValue<decimal>(),
                        Credit = row.Cell(5).GetValue<decimal>(),
                    }).ToList();
                var operation = await _billService.ModifyBillsByExcelFile(userId, requestList, cancellationToken);
                return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Op.Failed("عملیات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError));
            }
        }

    }
}
