using core.application.Contract.infrastructure;
using core.infrastructure.Data.persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs;
using Microsoft.EntityFrameworkCore;
using core.domain.DomainModelDTOs.LogDTOs.CommonDTOs;
using core.domain.entity.log;

namespace core.infrastructure.Data.repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly EnjoyLifeContext _context;
        public ActivityRepository(EnjoyLifeContext context)
        {
            this._context = context;
        }

        public async Task<OperationResult<GetActivityLogDTO>> GetActivityLogs(Filter_GetActivityLogDTO filter, CancellationToken cancellationToken = default)
        {
            OperationResult<GetActivityLogDTO> Op = new("GetActivityLogs");
            if (filter != null && filter.UserId != null && filter.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                long count = Convert.ToInt64((from log in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                              where
                                                 filter == null || (
                                                     (filter.UserId == null || log.UserCoreId == filter.UserId) &&
                                                     (filter.LoginDate == null || log.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                                     (filter.LogoutDate == null || log.LogoutDate != null && log.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime())
                                                 )
                                              select log.Id).ToList().Count);
                if (count <= 0)
                {
                    return Op.Succeed("دریافت لیست لاگ ورود و خروج ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", System.Net.HttpStatusCode.NoContent);
                }
                List<ActivityLogSingleDTO> data = new();
                if (filter != null && filter.PageSize != null && filter.PageSize > 0)
                {
                    if (filter.PageNumber == null || filter.PageNumber <= 0)
                    {
                        filter.PageNumber = 1;
                    }
                    data.AddRange((from log in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                   where
                                      filter == null || (
                                          (filter.UserId == null || log.UserCoreId == filter.UserId) &&
                                          (filter.LoginDate == null || log.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                          (filter.LogoutDate == null || log.LogoutDate != null && log.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime())
                                      )
                                   select new ActivityLogSingleDTO
                                   {
                                       Id = log.Id,
                                       LoginDate = log.LoginDate,
                                       LogoutDate = log.LogoutDate,
                                       UserId = log.UserCoreId
                                   }).OrderByDescending(x => x.LoginDate).ThenBy(x => x.LogoutDate).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList());
                }
                else
                {
                    data.AddRange((from log in await _context.ActivityLookups.ToListAsync(cancellationToken: cancellationToken)
                                   where
                                      filter == null || (
                                          (filter.UserId == null || log.UserCoreId == filter.UserId) &&
                                          (filter.LoginDate == null || log.LoginDate.ResetTime() == filter.LoginDate.ResetTime()) &&
                                          (filter.LogoutDate == null || log.LogoutDate != null && log.LogoutDate.ResetTime() == filter.LogoutDate.ResetTime())
                                      )
                                   select new ActivityLogSingleDTO
                                   {
                                       Id = log.Id,
                                       LoginDate = log.LoginDate,
                                       LogoutDate = log.LogoutDate,
                                       UserId = log.UserCoreId
                                   }).OrderByDescending(x => x.LoginDate).ThenBy(x => x.LogoutDate).ToList());
                }
                return Op.Succeed("دریافت لیست لاگ ورود و خروج ها با موفقیت انجام شد", new GetActivityLogDTO
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
                return Op.Failed("دریافت لیست لاگ ورود و خروج ها با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> SetLog_Login(UserIdDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("SetLog_Login");
            if (model == null || model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            try
            {
                var existsLog = await _context.ActivityLookups.FirstOrDefaultAsync(x => x.UserCoreId == model.UserId && x.LogoutDate == null, cancellationToken: cancellationToken);
                if (existsLog == null)
                {
                    await _context.ActivityLookups.AddAsync(new ActivityLookup
                    {
                        LoginDate = Op.OperationDate,
                        UserCoreId = model.UserId,
                    }, cancellationToken);
                }
                else
                {
                    existsLog.LoginDate = Op.OperationDate;
                    _context.Entry<ActivityLookup>(existsLog).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت لاگ ورود با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت لاگ ورود با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> SetLog_Logout(UserIdDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("SetLog_Logout");
            if (model == null || model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            try
            {
                var existsLog = await _context.ActivityLookups.FirstOrDefaultAsync(x => x.UserCoreId == model.UserId && x.LogoutDate == null, cancellationToken: cancellationToken);
                if (existsLog != null)
                {
                    existsLog.LogoutDate = Op.OperationDate;
                    _context.Entry<ActivityLookup>(existsLog).State = EntityState.Modified;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return Op.Succeed("ثبت لاگ خروج با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت لاگ خروج با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
