using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using news.application.DomainModelDTOs;
using news.application.DomainModelDTOs.FilterDTOs;
using news.application.Framework;
using news.application.Services;
using news.domain.Models;
using System.Net;
using System.Security.Claims;
using System.Threading;

namespace news.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TempNewsArticlesController : ControllerBase
    {
        private readonly ITempNewsServices _tempNewsServices;
        public TempNewsArticlesController(ITempNewsServices tempNewsServices)
        {
            this._tempNewsServices = tempNewsServices;
        }

        [HttpGet("GetNewsTagTypes")]
        public async Task<ActionResult<OperationResult<object>>> GetNewsTagTypes()
        {
            OperationResult<object> Op = new("GetNewsTagTypes");
            try
            {
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", new List<object>() {
               new { id= (int)NewsTagType.MAINTENANCE,title=NewsTagType.MAINTENANCE.ToString(),displayTitle= "فنی و مهندسی"},
               new { id= (int)NewsTagType.CONCIERGE,title=NewsTagType.CONCIERGE.ToString(),displayTitle= "کانسیرژ"},
               new { id= (int)NewsTagType.MANAGMENT,title=NewsTagType.MANAGMENT.ToString(),displayTitle= "مدیر ساختمان"},
               new { id= (int)NewsTagType.EVENT,title=NewsTagType.EVENT.ToString(),displayTitle= "ایونت"},
               new { id= (int)NewsTagType.PERFORMANCE_REPORTS,title=NewsTagType.PERFORMANCE_REPORTS.ToString(),displayTitle= "گزارش عملکرد"},
               new { id= (int)NewsTagType.ETC,title=NewsTagType.ETC.ToString(),displayTitle= "سایر"}
            });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }


        }

        [HttpGet("GetAdminTempNewsArticle")]
        public async Task<ActionResult<OperationResult<Admin_GetTempNewsArticleDomainDTO>>> GetAdminTempNewsArticle([FromQuery] Admin_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.GetAdminTempNewsArticle(filter, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
        [HttpGet("GetAdminSingleTempNewsArticle/{NewsId}")]
        public async Task<ActionResult<OperationResult<Admin_GetSingleTempNewsArticleDomainDTO>>> GetAdminSingleTempNewsArticle(long NewsId, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.GetAdminSingleTempNewsArticle(NewsId, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
        
        [HttpGet("GetCustomerTempNewsArticle")]
        public async Task<ActionResult<OperationResult<Customer_GetTempNewsArticleDomainDTO>>> GetCustomerTempNewsArticle([FromQuery] Customer_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.GetCustomerTempNewsArticle(filter, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
        [HttpGet("GetSingleTempNewsArticle/{NewsId}")]
        public async Task<ActionResult<OperationResult<Customer_GetSingleTempNewsArticleDomainDTO>>> GetSingleTempNewsArticle(long NewsId, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.GetSingleTempNewsArticle(NewsId, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }

        [HttpPost("PostTempNewsArticle/{AdminId}")]
        public async Task<ActionResult<OperationResult<object>>> PostTempNewsArticle(int AdminId, PostTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.PostTempNewsArticle(model, AdminId, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
        [HttpPost("UpdateTempNewsArticle/{AdminId}")]
        public async Task<ActionResult<OperationResult<object>>> UpdateTempNewsArticle(int AdminId, UpdateTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.UpdateTempNewsArticle(model, AdminId, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
        [HttpPost("DeleteTempNewsArticle")]
        public async Task<ActionResult<OperationResult<object>>> DeleteTempNewsArticle(DeleteTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default)
        {
            var result = await _tempNewsServices.DeleteTempNewsArticle(model, cancellationToken);
            return result.Success ? Ok(result) : StatusCode((int)result.Status, result);
        }
    }
}
