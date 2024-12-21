using core.application.Contract.infrastructure;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.infrastructure.Data.persist;
using core.application.Framework;
using Microsoft.EntityFrameworkCore;
using core.domain.DomainModelDTOs.LogDTOs.CommonDTOs;
using core.domain.entity.log;

namespace core.infrastructure.Data.repository
{
    public class ActionRepository : IActionRepository
    {
        private readonly EnjoyLifeContext _context;
        public ActionRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<GetActionLogDTO>> GetActionLogs(Filter_GetActionLogDTO filter, CancellationToken cancellationToken = default)
        {
            OperationResult<GetActionLogDTO> Op = new("GetActionLogs");
            if (filter != null)
            {
                if (filter.UserId != null && filter.UserId <= 0)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
                }
                if (filter.ModuleId != null && filter.ModuleId <= 0)
                {
                    return Op.Failed("شناسه ماژول بدرستی ارسال نشده است");
                }
            }
            try
            {
                if (filter != null && filter.ModuleId != null && !await _context.Modules.AnyAsync(x => x.Id == filter.ModuleId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ماژول بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                }
                long count = Convert.ToInt64((from module in await _context.Modules.ToListAsync(cancellationToken: cancellationToken)
                                              join log in await _context.ActionLookups.ToListAsync(cancellationToken: cancellationToken)
                                              on module.Id equals log.ModuleId
                                              join activity in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                              on log.ActivityLookupId equals activity.Id
                                              where filter == null ||
                                                 (
                                                     (filter.UserId == null || activity.UserCoreId == filter.UserId) &&
                                                     (filter.LoginDate == null || activity.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                                     (filter.LogoutDate == null || activity.LogoutDate != null && activity.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime()) &&
                                                     (filter.ModuleId == null || log.ModuleId == filter.ModuleId) &&
                                                     (!filter.Search.IsNotNull() || log.ActionName.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.ActionDescription.IsNotNull() && log.ActionDescription.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower())) || log.Key.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.Value.IsNotNull() && log.Value.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower()))) &&
                                                     (!filter.Key.IsNotNull() || log.Key.ToLower().Contains(filter.Key.AllSpaceRemover().ToLower())) &&
                                                     (filter.LogDate == null || log.LogDate.ResetTime() == filter.LogDate.ResetTime())
                                                 )
                                              select log.Id).ToList().Count);
                if (count <= 0)
                {
                    return Op.Succeed("دریافت لیست لاگ فعالیت کاربران با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", System.Net.HttpStatusCode.NoContent);
                }
                List<ActionLogSingleDTO> data = new();

                if (filter != null && filter.PageSize != null && filter.PageSize > 0)
                {
                    if (filter.PageNumber == null || filter.PageNumber <= 0)
                    {
                        filter.PageNumber = 1;
                    }

                    data.AddRange((from module in await _context.Modules.ToListAsync(cancellationToken: cancellationToken)
                                   join log in await _context.ActionLookups.ToListAsync(cancellationToken: cancellationToken)
                                   on module.Id equals log.ModuleId
                                   join activity in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                   on log.ActivityLookupId equals activity.Id
                                   where filter == null ||
                                      (
                                          (filter.UserId == null || activity.UserCoreId == filter.UserId) &&
                                          (filter.LoginDate == null || activity.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                          (filter.LogoutDate == null || activity.LogoutDate != null && activity.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime()) &&
                                          (filter.ModuleId == null || log.ModuleId == filter.ModuleId) &&
                                          (!filter.Search.IsNotNull() || log.ActionName.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.ActionDescription.IsNotNull() && log.ActionDescription.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower())) || log.Key.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.Value.IsNotNull() && log.Value.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower()))) &&
                                          (!filter.Key.IsNotNull() || log.Key.ToLower().Contains(filter.Key.AllSpaceRemover().ToLower())) &&
                                          (filter.LogDate == null || log.LogDate.ResetTime() == filter.LogDate.ResetTime())
                                      )
                                   select new ActionLogSingleDTO
                                   {
                                       ActionDescription = log.ActionDescription,
                                       ActionName = log.ActionName,
                                       Id = log.Id,
                                       Key = log.Key,
                                       LogDate = log.LogDate,
                                       LoginDate = activity.LoginDate,
                                       LogoutDate = activity.LogoutDate,
                                       ModuleDisplayName = module.DisplayName,
                                       ModuleId = log.ModuleId,
                                       ModuleName = module.Name,
                                       UserId = activity.UserCoreId,
                                       Value = log.Value,
                                   }).OrderByDescending(x => x.LogDate).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList());
                }
                else
                {
                    data.AddRange((from module in await _context.Modules.ToListAsync(cancellationToken: cancellationToken)
                                   join log in await _context.ActionLookups.ToListAsync(cancellationToken: cancellationToken)
                                   on module.Id equals log.ModuleId
                                   join activity in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                   on log.ActivityLookupId equals activity.Id
                                   where filter == null ||
                                      (
                                          (filter.UserId == null || activity.UserCoreId == filter.UserId) &&
                                          (filter.LoginDate == null || activity.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                          (filter.LogoutDate == null || activity.LogoutDate != null && activity.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime()) &&
                                          (filter.ModuleId == null || log.ModuleId == filter.ModuleId) &&
                                          (!filter.Search.IsNotNull() || log.ActionName.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.ActionDescription.IsNotNull() && log.ActionDescription.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower())) || log.Key.ToLower().Contains(filter.Search.AllSpaceRemover().ToLower()) || (log.Value.IsNotNull() && log.Value.ToLower().Contains(filter.Search.MultipleSpaceRemover().ToLower()))) &&
                                          (!filter.Key.IsNotNull() || log.Key.ToLower().Contains(filter.Key.AllSpaceRemover().ToLower())) &&
                                          (filter.LogDate == null || log.LogDate.ResetTime() == filter.LogDate.ResetTime())
                                      )
                                   select new ActionLogSingleDTO
                                   {
                                       ActionDescription = log.ActionDescription,
                                       ActionName = log.ActionName,
                                       Id = log.Id,
                                       Key = log.Key,
                                       LogDate = log.LogDate,
                                       LoginDate = activity.LoginDate,
                                       LogoutDate = activity.LogoutDate,
                                       ModuleDisplayName = module.DisplayName,
                                       ModuleId = log.ModuleId,
                                       ModuleName = module.Name,
                                       UserId = activity.UserCoreId,
                                       Value = log.Value,
                                   }).OrderByDescending(x => x.LogDate).ToList());
                }
                return Op.Succeed("دریافت لیست لاگ فعالیت کاربران با موفقیت انجام شد", new GetActionLogDTO
                {
                    ListData = data,
                    PaginationData = new PageResponseDTO
                    {
                        Count = count,
                        PageNumber = filter?.PageNumber,
                        PageSize = filter?.PageSize,
                    }
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست لاگ فعالیت کاربران با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<GetModuleDTO>> GetModules(CancellationToken cancellationToken = default)
        {
            OperationResult<GetModuleDTO> Op = new("GetModules");
            try
            {
                var data = await _context.Modules.ToListAsync(cancellationToken: cancellationToken);
                if (data == null || !data.Any())
                {
                    return Op.Succeed("دریافت لیست ماژول ها با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", System.Net.HttpStatusCode.NotFound);
                }
                return Op.Succeed("دریافت لیست ماژول ها با موفقیت انجام شد", data.Select(x => new GetModuleDTO
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id,
                    Name = x.Name
                }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست ماژول ها با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> SetLog_Action(int userId, SetLogDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("SetLog");
            if (userId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت ثبت لاگ ارسال نشده است");
            }
            if (!model.Module.IsNotNull())
            {
                return Op.Failed("ارسال ماژول برای ثبت لاگ اجباری است");
            }
            if (!model.ActionName.IsNotNull())
            {
                return Op.Failed("ارسال نام عملیات برای ثبت لاگ اجباری است");
            }
            if (model.ActionName.AllSpaceRemover().Length > 200)
            {
                return Op.Failed("حداکثر کاراکتر مجاز برای نام عملیات، 200 کاراکتر است");
            }
            if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemover().Length > 2000)
            {
                return Op.Failed("حداکثر کاراکتر مجاز برای توضیحات عملیات، 2000 کاراکتر است");
            }
            if (!model.Key.IsNotNull())
            {
                return Op.Failed("ارسال کلید عملیات برای ثبت لاگ اجباری است");
            }
            if (model.Key.AllSpaceRemover().Length > 150)
            {
                return Op.Failed("حداکثر کاراکتر مجاز برای کلید عملیات، 150 کاراکتر است");
            }
            if (model.Value.IsNotNull() && model.Value.MultipleSpaceRemover().Length > 1500)
            {
                return Op.Failed("حداکثر کاراکتر مجاز برای مقدار عملیات، 1500 کاراکتر است");
            }
            try
            {
                var openActiveLog = await _context.ActivityLookups.FirstOrDefaultAsync(x => x.UserCoreId == userId && x.LogoutDate == null, cancellationToken: cancellationToken);
                if (openActiveLog == null)
                {
                    return Op.Failed("لاگ ورودی از کاربر با شناسه ارسال شده وجود ندارد", System.Net.HttpStatusCode.NotFound);
                }
                var module = await _context.Modules.FirstOrDefaultAsync(x => x.Name.ToUpper() == model.Module.AllSpaceRemover().ToUpper(), cancellationToken: cancellationToken);
                if (module == null)
                {
                    return Op.Failed("ماژول ارسال شده در سیستم وجود ندارد", System.Net.HttpStatusCode.NotFound);
                }
                await _context.ActionLookups.AddAsync(new ActionLookup
                {
                    ActionDescription = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemover() : null,
                    ActionName = model.ActionName.AllSpaceRemover(),
                    ActivityLookupId = openActiveLog.Id,
                    Key = model.Key.AllSpaceRemover(),
                    LogDate = Op.OperationDate,
                    ModuleId = module.Id,
                    Value = model.Value.IsNotNull() ? model.Value.MultipleSpaceRemover() : null,
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت لاگ عملیاتی با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت لاگ عملیاتی با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
