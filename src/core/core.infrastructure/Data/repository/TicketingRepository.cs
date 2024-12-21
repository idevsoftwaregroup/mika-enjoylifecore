using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.displayEntities.ticketingModels;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs;
using core.domain.entity.enums;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using core.domain.entity.TicketModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace core.infrastructure.Data.repository;

public class TicketingRepository : ITicketingRepository
{
    private readonly EnjoyLifeContext _context;

    public TicketingRepository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> CreateTicket(int UserId, Request_CreateTicketDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("CreateTicket");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title))
        {
            return Op.Failed("ارسال موضوع درخواست اجباری است");
        }
        if (string.IsNullOrEmpty(model.Description) || string.IsNullOrWhiteSpace(model.Description))
        {
            return Op.Failed("ارسال توضیحات درخواست اجباری است");
        }
        if (model.UnitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if ((!await _context.Residents.AnyAsync(x => x.User == user && x.Unit == unit, cancellationToken: cancellationToken)) && (!await _context.Owners.AnyAsync(x => x.User == user && x.Unit == unit, cancellationToken: cancellationToken)))
            {
                return Op.Failed("اطلاعات کاربری با اطلاعات واحد ارسال شده همخوانی ندارد", HttpStatusCode.Forbidden);
            }

            long? attachmentId = null;
            if (!string.IsNullOrEmpty(model.Url) && !string.IsNullOrWhiteSpace(model.Url))
            {
                await _context.Attachments.AddAsync(new Attachment(model.Url), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                attachmentId = (from attachment in await _context.Attachments.Where(x => x.Url == model.Url).ToListAsync(cancellationToken)
                                select attachment.Id).FirstOrDefault();
                if (attachmentId == null)
                {
                    return Op.Failed("دریافت شناسه فایل درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
            }
            var pendingStatus = await _context.TicketsStatus.FirstOrDefaultAsync(x => x.Title.ToUpper() == "PENDING", cancellationToken: cancellationToken);
            if (pendingStatus == null)
            {
                return Op.Failed("دریافت وضعیت تیکت با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            await _context.Tickets.AddAsync(new domain.entity.TicketModels.TicketModel
            {
                CreatedAt = Op.OperationDate,
                Description = model.Description.Trim(),
                StatusId = pendingStatus.Id,
                Title = model.Title,
                UnitId = unit.Id,
                Urgent = model.Urgent != null && Convert.ToBoolean(model.Urgent),
                UserId = UserId,
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            long? addedTicketId = (from ticket in await _context.Tickets.Where(x => x.CreatedAt == Op.OperationDate).ToListAsync(cancellationToken)
                                   select ticket.Id).FirstOrDefault();
            if (addedTicketId == null)
            {
                return Op.Failed("دریافت شناسه درخواست ثبتن شده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (attachmentId != null)
            {
                await _context.TicketMessages.AddAsync(new domain.entity.TicketModels.TicketMessageModel
                {
                    AttachmentId = attachmentId,
                    AuthorId = UserId,
                    CreatedAt = Op.OperationDate,
                    TicketId = Convert.ToInt64(addedTicketId),
                }, cancellationToken);
            }
            await _context.TicketsLog.AddAsync(new domain.entity.TicketModels.TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = "ارسال درخواست",
                ModifiedByAdmin = false,
                StatusId = pendingStatus.Id,
                TicketId = Convert.ToInt64(addedTicketId)
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت درخواست با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = Convert.ToInt64(addedTicketId),
                AttachmentUrl = attachmentId != null ? model.Url : null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{user.FirstName} {user.LastName}",
                Description = model.Description,
                Title = model.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = user.PhoneNumber,
                TrackingCode = null,
                TicketNumber = null
            });

        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> ActivateTicket(int UserId, Request_ActivateTicketDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("ActivateTicket");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای فعال سازی درخواست ارسال نشده است");
        }
        if (model.Id <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.TrackingCode) || string.IsNullOrWhiteSpace(model.TrackingCode))
        {
            return Op.Failed("ارسال کد پیگیری درخواست اجباری است");
        }
        if (!Regex.IsMatch(model.TrackingCode, "^[0-9]*$", RegexOptions.IgnoreCase))
        {
            return Op.Failed("کد پیگیری بدرستی وارد نشده است");
        }
        //if (string.IsNullOrEmpty(model.TicketNumber) || string.IsNullOrWhiteSpace(model.TicketNumber))
        //{
        //    return Op.Failed("ارسال شماره درخواست اجباری است");
        //}
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var pendingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "PENDING");
            var onGoingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ONGOING");
            if (ticketStatuses == null || !ticketStatuses.Any() || pendingStatus == null || onGoingStatus == null)
            {
                return Op.Failed("دریافت وضعیت تیکت با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (ticket.StatusId != pendingStatus.Id)
            {
                return Op.Failed($"فعال سازی تنها برای درخواست با وضعیت '{pendingStatus.DisplayTitle}' امکانپذیر است", HttpStatusCode.Forbidden);
            }
            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (await _context.Tickets.AnyAsync(x => !string.IsNullOrEmpty(x.TrackingCode) && !string.IsNullOrWhiteSpace(x.TrackingCode) && x.TrackingCode.ToUpper() == model.TrackingCode.Trim().ToUpper() && x.Id != ticket.Id, cancellationToken: cancellationToken))
            {
                return Op.Failed("کد پیگیری ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            if (await _context.Tickets.AnyAsync(x => !string.IsNullOrEmpty(x.TicketNumber) && !string.IsNullOrWhiteSpace(x.TicketNumber) && x.TicketNumber.ToUpper() == model.TicketNumber.Trim().ToUpper() && x.Id != ticket.Id, cancellationToken: cancellationToken))
            {
                return Op.Failed("شماره درخواست ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            ticket.TrackingCode = model.TrackingCode.Trim();
            if (!string.IsNullOrEmpty(model.TicketNumber) && !string.IsNullOrWhiteSpace(model.TicketNumber))
            {
                ticket.TicketNumber = model.TicketNumber.Trim();
            }
            ticket.StatusId = onGoingStatus.Id;
            ticket.ModifyDate = Op.OperationDate;
            ticket.TechnicianId = UserId;

            _context.Entry<TicketModel>(ticket).State = EntityState.Modified;

            await _context.TicketsLog.AddAsync(new TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = "فعال سازی درخواست",
                ModifiedByAdmin = true,
                StatusId = onGoingStatus.Id,
                TicketId = ticket.Id,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("فعال سازی درخواست با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = ticket.Id,
                AttachmentUrl = null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber,
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("فعال سازی درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SucceedTicket(int UserId, Request_ModifyTicketDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("SucceedTicket");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی تغییر وضعییت درخواست به انجام شده ارسال نشده است");
        }
        if (model.TicketId <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var onGoingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ONGOING");
            var successStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "SUCCESS");

            if (ticketStatuses == null || onGoingStatus == null || successStatus == null)
            {
                return Op.Failed("دریافت وضعیت درخواست ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.TicketId, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (ticket.StatusId != onGoingStatus.Id)
            {
                return Op.Failed("تغییر به انجام شده تنها برای درخواست های دارای وضعیت 'در دست اقدام' امکان پذیر است", HttpStatusCode.Forbidden);
            }
            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            ticket.StatusId = successStatus.Id;
            ticket.ModifyDate = Op.OperationDate;
            _context.Entry<TicketModel>(ticket).State = EntityState.Modified;

            await _context.TicketsLog.AddAsync(new TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = string.IsNullOrEmpty(model.Message) || string.IsNullOrWhiteSpace(model.Message) ? "تغییر به وضعیت انجام شده" : model.Message.Trim(),
                ModifiedByAdmin = true,
                StatusId = ticket.StatusId,
                TicketId = ticket.Id,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("تغییر وضعیت درخواست به انجام شده با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = ticket.Id,
                AttachmentUrl = null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber,
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("تغییر وضعیت درخواست به انجام شده با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }
    public async Task<OperationResult<Response_CloseTicketDomainDTO>> CloseTicket(int UserId, bool isAdmin, Request_ModifyTicketDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CloseTicketDomainDTO> Op = new("CloseTicket");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای لغو درخواست ارسال نشده است");
        }
        if (model.TicketId <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var successStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "SUCCESS");
            var closedByAdminStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ADMINCLOSED");
            var closedByUserStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "USERCLOSED");
            if (successStatus == null || closedByAdminStatus == null || closedByUserStatus == null)
            {
                return Op.Failed("دریافت اطلاعات وضعیت درخواست ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.TicketId, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            if (!isAdmin && ticket.UserId != UserId)
            {
                return Op.Failed("امکان لغو این درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
            }

            if (ticket.StatusId == successStatus.Id || ticket.StatusId == closedByAdminStatus.Id || ticket.StatusId == closedByUserStatus.Id)
            {
                return Op.Failed($"لغو درخواست با وضعیت '{ticketStatuses.FirstOrDefault(x => x.Id == ticket.StatusId).DisplayTitle}' امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
            }

            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            ticket.StatusId = isAdmin ? closedByAdminStatus.Id : closedByUserStatus.Id;
            ticket.ModifyDate = Op.OperationDate;
            _context.Entry<TicketModel>(ticket).State = EntityState.Modified;

            await _context.TicketsLog.AddAsync(new TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = string.IsNullOrEmpty(model.Message) || string.IsNullOrWhiteSpace(model.Message) ? isAdmin ? "لغو درخواست توسط ادمین" : "لغو درخواست توسط کاربر" : model.Message.Trim(),
                ModifiedByAdmin = isAdmin,
                StatusId = ticket.StatusId,
                TicketId = ticket.Id,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("لغو درخواست با موفقیت انجام شد", new Response_CloseTicketDomainDTO
            {
                CloseMessage = string.IsNullOrEmpty(model.Message) || string.IsNullOrWhiteSpace(model.Message) ? null : model.Message.Trim(),
                CreatedAt = ticket.CreatedAt,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                CreatedByGenderType = customer.Gender,
                CreatedByPhoneNumber = customer.PhoneNumber,
                Description = ticket.Description,
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                TrackingCode = ticket.TrackingCode,
                UnitName = unit.Name
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("لغو درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTrackCode(int UserId, Request_UpdateTrackCodeDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("UpdateTrackCode");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی کد پیگیری ارسال نشده است");
        }
        if (model.Id <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.TrackingCode) || string.IsNullOrWhiteSpace(model.TrackingCode))
        {
            return Op.Failed("ارسال کد پیگیری درخواست اجباری است");
        }
        if (!Regex.IsMatch(model.TrackingCode, "^[0-9]*$", RegexOptions.IgnoreCase))
        {
            return Op.Failed("کد پیگیری بدرستی وارد نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var pendingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "PENDING");
            var onGoingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ONGOING");
            if (ticketStatuses == null || !ticketStatuses.Any() || pendingStatus == null || onGoingStatus == null)
            {
                return Op.Failed("دریافت وضعیت درخواست ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (ticket.StatusId != pendingStatus.Id && ticket.StatusId != onGoingStatus.Id)
            {
                return Op.Failed($"بروزرسانی کد پیگیری برای درخواست با وضعیت '{ticketStatuses.FirstOrDefault(x => x.Id == ticket.StatusId).DisplayTitle}' امکانپذیر نیست", HttpStatusCode.Forbidden);
            }
            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (await _context.Tickets.AnyAsync(x => !string.IsNullOrEmpty(x.TrackingCode) && !string.IsNullOrWhiteSpace(x.TrackingCode) && x.TrackingCode.ToUpper() == model.TrackingCode.Trim().ToUpper() && x.Id != ticket.Id, cancellationToken: cancellationToken))
            {
                return Op.Failed("کد پیگیری ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            ticket.TrackingCode = model.TrackingCode.Trim();
            ticket.ModifyDate = Op.OperationDate;
            _context.Entry<TicketModel>(ticket).State = EntityState.Modified;

            await _context.TicketsLog.AddAsync(new TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = "بروزرسانی کد پیگیری",
                ModifiedByAdmin = true,
                StatusId = ticket.StatusId,
                TicketId = ticket.Id,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("بروزرسانی کد پیگیری درخواست با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = ticket.Id,
                AttachmentUrl = null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی کد پیگیری درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTicketNumber(int UserId, Request_UpdateTicketNumberDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("UpdateTicketNumber");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی شماره درخواست ارسال نشده است");
        }
        if (model.Id <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.TicketNumber) || string.IsNullOrWhiteSpace(model.TicketNumber))
        {
            return Op.Failed("ارسال شماره درخواست اجباری است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var pendingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "PENDING");
            var onGoingStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ONGOING");
            if (ticketStatuses == null || !ticketStatuses.Any() || pendingStatus == null || onGoingStatus == null)
            {
                return Op.Failed("دریافت وضعیت درخواست ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (ticket.StatusId != pendingStatus.Id && ticket.StatusId != onGoingStatus.Id)
            {
                return Op.Failed($"بروزرسانی شماره درخواست برای درخواست با وضعیت '{ticketStatuses.FirstOrDefault(x => x.Id == ticket.StatusId).DisplayTitle}' امکانپذیر نیست", HttpStatusCode.Forbidden);
            }
            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (await _context.Tickets.AnyAsync(x => !string.IsNullOrEmpty(x.TicketNumber) && !string.IsNullOrWhiteSpace(x.TicketNumber) && x.TicketNumber.ToUpper() == model.TicketNumber.Trim().ToUpper() && x.Id != ticket.Id, cancellationToken: cancellationToken))
            {
                return Op.Failed("شماره درخواست ارسال شده تکراری است", HttpStatusCode.Conflict);
            }

            ticket.TicketNumber = model.TicketNumber.Trim();
            ticket.ModifyDate = Op.OperationDate;
            _context.Entry<TicketModel>(ticket).State = EntityState.Modified;

            await _context.TicketsLog.AddAsync(new TicketLogModel
            {
                LogDate = Op.OperationDate,
                Message = "بروزرسانی شماره درخواست",
                ModifiedByAdmin = true,
                StatusId = ticket.StatusId,
                TicketId = ticket.Id,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("بروزرسانی شماره درخواست با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = ticket.Id,
                AttachmentUrl = null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber,
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی شماره درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SeenTicket(int UserId, Request_TicketId model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CommonOperationTicketDomainDTO> Op = new("SeenTicket");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("شناسه درخواست ارسال نشده است");
        }
        if (model.TicketId <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.TicketId, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            if (!await _context.TicketsSeen.AnyAsync(x => x.TicketId == ticket.Id && x.SeenBy == UserId, cancellationToken: cancellationToken))
            {
                await _context.TicketsSeen.AddAsync(new TicketSeenModel
                {
                    SeenBy = UserId,
                    SeenDate = Op.OperationDate,
                    TicketId = ticket.Id
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            return Op.Succeed("ثبت 'خوانده شده' برای درخواست با موفقیت انجام شد", new Response_CommonOperationTicketDomainDTO
            {
                TicketId = ticket.Id,
                AttachmentUrl = null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت 'خوانده شده' برای درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_CreateTicketMessageDomainDTO>> CreateMessage(int UserId, bool isAdmin, Request_CreateTicketMessageDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CreateTicketMessageDomainDTO> Op = new("CreateMessage");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت پیام درخواست ارسال نشده است");
        }
        if (model.TicketId <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.Text) && string.IsNullOrWhiteSpace(model.Text) && string.IsNullOrEmpty(model.Url) && string.IsNullOrWhiteSpace(model.Url))
        {
            return Op.Failed("ارسال فایل یا متن پیام اجباری است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == model.TicketId, cancellationToken: cancellationToken);
            if (ticket == null)
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            if (!isAdmin && ticket.UserId != UserId)
            {
                return Op.Failed("امکان ارسال پیام مربوط به این درخواست برای کاربری وجود ندارد", HttpStatusCode.Forbidden);
            }

            var ticketStatuses = await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken);
            var successStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "SUCCESS");
            var closedByAdminStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "ADMINCLOSED");
            var closedByUserStatus = ticketStatuses.FirstOrDefault(x => x.Title.ToUpper() == "USERCLOSED");
            if (successStatus == null || closedByAdminStatus == null || closedByUserStatus == null)
            {
                return Op.Failed("دریافت اطلاعات وضعیت درخواست ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (ticket.StatusId == successStatus.Id || ticket.StatusId == closedByAdminStatus.Id || ticket.StatusId == closedByUserStatus.Id)
            {
                return Op.Failed($"ارسال پیام برای درخواست با وضعیت '{ticketStatuses.FirstOrDefault(x => x.Id == ticket.StatusId).DisplayTitle}' امکان پذیر نمیباشد", HttpStatusCode.Forbidden);
            }

            var customer = await _context.Users.FirstOrDefaultAsync(x => x.Id == ticket.UserId, cancellationToken: cancellationToken);
            if (customer == null)
            {
                return Op.Failed("دریافت اطلاعات ثبت کننده درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == ticket.UnitId, cancellationToken: cancellationToken);
            if (unit == null)
            {
                return Op.Failed("دریافت اطلاعات واحد درخواست کننده با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }

            long? attachmentId = null;
            if (!string.IsNullOrEmpty(model.Url) && !string.IsNullOrWhiteSpace(model.Url))
            {
                await _context.Attachments.AddAsync(new Attachment(model.Url), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                attachmentId = (from attachment in await _context.Attachments.Where(x => x.Url == model.Url).ToListAsync(cancellationToken)
                                select attachment.Id).FirstOrDefault();
                if (attachmentId == null)
                {
                    return Op.Failed("دریافت شناسه فایل درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
            }

            await _context.TicketMessages.AddAsync(new TicketMessageModel
            {
                AttachmentId = attachmentId,
                AuthorId = UserId,
                CreatedAt = Op.OperationDate,
                Text = string.IsNullOrEmpty(model.Text) || string.IsNullOrWhiteSpace(model.Text) ? null : model.Text.Trim(),
                TicketId = ticket.Id,
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("ارسال پیام با موفقیت انجام شد", new Response_CreateTicketMessageDomainDTO
            {

                TicketId = ticket.Id,
                AttachmentUrl = attachmentId != null ? model.Url : null,
                CreatedAt = Op.OperationDate,
                CreatedBy = $"{customer.FirstName} {customer.LastName}",
                Description = ticket.Description,
                Title = ticket.Title,
                UnitName = unit.Name,
                CreatedByPhoneNumber = customer.PhoneNumber,
                TrackingCode = ticket.TrackingCode,
                TicketNumber = ticket.TicketNumber,
                CreatedByGenderType = customer.Gender,
                MessageText = string.IsNullOrEmpty(model.Text) || string.IsNullOrWhiteSpace(model.Text) ? null : model.Text.Trim()
            });

        }
        catch (Exception ex)
        {
            return Op.Failed("ارسال پیام با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }
    public async Task<OperationResult<Response_GetTicketDomainDTO>> GetTicketsByAdmin(int UserId, Filter_GetTicketAdminDomainDTO filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetTicketDomainDTO> Op = new("GetTicketsByAdmin");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        try
        {
            int? filteredStatusId = null;
            if (filter.StatusCode != null)
            {
                var acceptableStatusCodes = new List<int>
                {
                    0,1,2,3,4
                };
                if (!acceptableStatusCodes.Any(x => x == filter.StatusCode))
                {
                    return Op.Failed("کد وضعیت درخواست بدرستی وارد نشده است");
                }
                var filteredStatus = await _context.TicketsStatus.FirstOrDefaultAsync(x => x.StatusCode == filter.StatusCode, cancellationToken: cancellationToken);
                if (filteredStatus == null)
                {
                    return Op.Failed("کد وضعیت درخواست بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                filteredStatusId = filteredStatus.Id;

            }
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            long totalCount = Convert.ToInt64((from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                                               join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                                               on ticket.StatusId equals status.Id
                                               join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                                               on ticket.Id equals seen.TicketId
                                               into tickets
                                               from seen in tickets.DefaultIfEmpty()
                                               where
                                               (filter.TicketId == null || ticket.Id == Convert.ToInt64(filter.TicketId)) &&
                                               (filter.TrackingCode == null || (!string.IsNullOrEmpty(ticket.TrackingCode) && !string.IsNullOrWhiteSpace(ticket.TrackingCode) && ticket.TrackingCode.ToUpper().StartsWith(filter.TrackingCode.ToUpper()))) &&
                                               (filter.TicketNumber == null || (!string.IsNullOrEmpty(ticket.TicketNumber) && !string.IsNullOrWhiteSpace(ticket.TicketNumber) && ticket.TicketNumber.ToUpper().StartsWith(filter.TicketNumber.ToUpper()))) &&
                                               (filter.Title == null || ticket.Title.ToUpper().StartsWith(filter.Title.ToUpper())) &&
                                               (filter.CreatedAt == null || ticket.CreatedAt.ResetTime() == filter.CreatedAt.ResetTime()) &&
                                               (filteredStatusId == null || ticket.StatusId == filteredStatusId) &&
                                               (filter.UserId == null || ticket.UserId == filter.UserId) &&
                                               (filter.UnitId == null || ticket.UnitId == filter.UnitId) &&
                                               (filter.Seen == null || (filter.Seen == true ? seen != null : seen == null))
                                               select ticket.Id).ToList().Count);
            if (totalCount == 0)
            {
                return Op.Succeed("دریافت لیست درخواست ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            List<TicketDomainDTO> list = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                                          join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                                          on ticket.StatusId equals status.Id
                                          join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                          on ticket.UserId equals user.Id
                                          join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                          on ticket.UnitId equals unit.Id
                                          join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                                          on ticket.Id equals seen.TicketId
                                          into tickets
                                          from seen in tickets.DefaultIfEmpty()
                                          join technician in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                          on ticket.TechnicianId equals technician.Id
                                          into ticketsFull
                                          from technicianUser in ticketsFull.DefaultIfEmpty()
                                          where
                                              (filter.TicketId == null || ticket.Id == Convert.ToInt64(filter.TicketId)) &&
                                              (filter.TrackingCode == null || (!string.IsNullOrEmpty(ticket.TrackingCode) && !string.IsNullOrWhiteSpace(ticket.TrackingCode) && ticket.TrackingCode.ToUpper().StartsWith(filter.TrackingCode.ToUpper()))) &&
                                              (filter.TicketNumber == null || (!string.IsNullOrEmpty(ticket.TicketNumber) && !string.IsNullOrWhiteSpace(ticket.TicketNumber) && ticket.TicketNumber.ToUpper().StartsWith(filter.TicketNumber.ToUpper()))) &&
                                              (filter.Title == null || ticket.Title.ToUpper().StartsWith(filter.Title.ToUpper())) &&
                                              (filter.CreatedAt == null || ticket.CreatedAt.ResetTime() == filter.CreatedAt.ResetTime()) &&
                                              (filteredStatusId == null || ticket.StatusId == filteredStatusId) &&
                                              (filter.UserId == null || ticket.UserId == filter.UserId) &&
                                              (filter.UnitId == null || ticket.UnitId == filter.UnitId) &&
                                              (filter.Seen == null || (filter.Seen == true ? seen != null : seen == null))
                                          select new TicketDomainDTO
                                          {
                                              CreatedAt = ticket.CreatedAt,
                                              Description = ticket.Description,
                                              Id = ticket.Id,
                                              ModifyDate = ticket.ModifyDate,
                                              Seen = seen != null,
                                              SeenDateDisplay = seen?.SeenDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                                              SeenDateTime = seen?.SeenDate,
                                              SeenTimeDisplay = seen?.SeenDate.ToString("HH:mm", new CultureInfo("fa-IR")),
                                              StatusCode = status.StatusCode,
                                              StatusDisplayTitle = status.DisplayTitle,
                                              StatusTitle = status.Title,
                                              TicketNumber = ticket.TicketNumber,
                                              Title = ticket.Title,
                                              TrackingCode = ticket.TrackingCode,
                                              Urgent = ticket.Urgent,
                                              Technician = technicianUser == null ? null : new TicketUserAndTechnicianDTO
                                              {
                                                  Email = technicianUser.Email,
                                                  FirstName = technicianUser.FirstName,
                                                  GenderType = technicianUser.Gender,
                                                  Gender = technicianUser.Gender switch
                                                  {
                                                      null => "آقا/خانم",
                                                      GenderType.ALL => "آقا/خانم",
                                                      GenderType.MALE => "آقا",
                                                      GenderType.FAMALE => "خانم",
                                                      _ => "آقا/خانم",
                                                  },
                                                  Id = technicianUser.Id,
                                                  LastName = technicianUser.LastName,
                                                  PhoneNumber = technicianUser.PhoneNumber,
                                              },
                                              Unit = new TicketUnitDTO
                                              {
                                                  Block = unit.Block,
                                                  ComplexId = unit.ComplexId,
                                                  Floor = unit.Floor,
                                                  Id = unit.Id,
                                                  Name = unit.Name
                                              },
                                              User = new TicketUserAndTechnicianDTO
                                              {
                                                  Email = user.Email,
                                                  FirstName = user.FirstName,
                                                  GenderType = user.Gender,
                                                  Gender = user.Gender switch
                                                  {
                                                      null => "آقا/خانم",
                                                      GenderType.ALL => "آقا/خانم",
                                                      GenderType.MALE => "آقا",
                                                      GenderType.FAMALE => "خانم",
                                                      _ => "آقا/خانم",
                                                  },
                                                  Id = user.Id,
                                                  LastName = user.LastName,
                                                  PhoneNumber = user.PhoneNumber,
                                              }
                                          }).OrderBy(x => x.Seen).ThenByDescending(x => x.CreatedAt).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList();

            return Op.Succeed("دریافت لیست درخواست ها با موفقیت انجام شد", new Response_GetTicketDomainDTO
            {
                TotalCount = totalCount,
                Tickets = list
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست درخواست ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_GetTicketDomainDTO>> GetTickets(int UserId, Filter_GetTicketUserDomainDTO filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetTicketDomainDTO> Op = new("GetTickets");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            int? filteredStatusId = null;
            if (filter.StatusCode != null)
            {
                var acceptableStatusCodes = new List<int>
                {
                    0,1,2,3,4
                };
                if (!acceptableStatusCodes.Any(x => x == filter.StatusCode))
                {
                    return Op.Failed("کد وضعیت درخواست بدرستی وارد نشده است");
                }
                var filteredStatus = await _context.TicketsStatus.FirstOrDefaultAsync(x => x.StatusCode == filter.StatusCode, cancellationToken: cancellationToken);
                if (filteredStatus == null)
                {
                    return Op.Failed("کد وضعیت درخواست بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                filteredStatusId = filteredStatus.Id;

            }

            long totalCount = Convert.ToInt64((from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                                               join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                                               on ticket.StatusId equals status.Id
                                               join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                                               on ticket.Id equals seen.TicketId
                                               into tickets
                                               from seen in tickets.DefaultIfEmpty()
                                               where
                                               ticket.UserId == UserId &&
                                               (filter.TicketId == null || ticket.Id == Convert.ToInt64(filter.TicketId)) &&
                                               (filter.TrackingCode == null || (!string.IsNullOrEmpty(ticket.TrackingCode) && !string.IsNullOrWhiteSpace(ticket.TrackingCode) && ticket.TrackingCode.ToUpper().StartsWith(filter.TrackingCode.ToUpper()))) &&
                                               (filter.TicketNumber == null || (!string.IsNullOrEmpty(ticket.TicketNumber) && !string.IsNullOrWhiteSpace(ticket.TicketNumber) && ticket.TicketNumber.ToUpper().StartsWith(filter.TicketNumber.ToUpper()))) &&
                                               (filter.Title == null || ticket.Title.ToUpper().StartsWith(filter.Title.ToUpper())) &&
                                               (filter.CreatedAt == null || ticket.CreatedAt.ResetTime() == filter.CreatedAt.ResetTime()) &&
                                               (filteredStatusId == null || ticket.StatusId == filteredStatusId) &&
                                               (filter.UnitId == null || ticket.UnitId == filter.UnitId) &&
                                               (filter.Seen == null || (filter.Seen == true ? seen != null : seen == null))
                                               select ticket.Id).ToList().Count);
            if (totalCount == 0)
            {
                return Op.Succeed("دریافت لیست درخواست ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            List<TicketDomainDTO> list = new();
            if (filter.PageSize == null || filter.PageSize <= 0)
            {
                list = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                        join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.StatusId equals status.Id
                        join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.UserId equals user.Id
                        join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.UnitId equals unit.Id
                        join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.Id equals seen.TicketId
                        into tickets
                        from seen in tickets.DefaultIfEmpty()
                        join technician in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.TechnicianId equals technician.Id
                        into ticketsFull
                        from technicianUser in ticketsFull.DefaultIfEmpty()
                        where
                            ticket.UserId == UserId &&
                            (filter.TicketId == null || ticket.Id == Convert.ToInt64(filter.TicketId)) &&
                            (filter.TrackingCode == null || (!string.IsNullOrEmpty(ticket.TrackingCode) && !string.IsNullOrWhiteSpace(ticket.TrackingCode) && ticket.TrackingCode.ToUpper().StartsWith(filter.TrackingCode.ToUpper()))) &&
                            (filter.TicketNumber == null || (!string.IsNullOrEmpty(ticket.TicketNumber) && !string.IsNullOrWhiteSpace(ticket.TicketNumber) && ticket.TicketNumber.ToUpper().StartsWith(filter.TicketNumber.ToUpper()))) &&
                            (filter.Title == null || ticket.Title.ToUpper().StartsWith(filter.Title.ToUpper())) &&
                            (filter.CreatedAt == null || ticket.CreatedAt.ResetTime() == filter.CreatedAt.ResetTime()) &&
                            (filteredStatusId == null || ticket.StatusId == filteredStatusId) &&
                            (filter.UnitId == null || ticket.UnitId == filter.UnitId) &&
                            (filter.Seen == null || (filter.Seen == true ? seen != null : seen == null))
                        select new TicketDomainDTO
                        {
                            CreatedAt = ticket.CreatedAt,
                            Description = ticket.Description,
                            Id = ticket.Id,
                            ModifyDate = ticket.ModifyDate,
                            Seen = seen != null,
                            SeenDateDisplay = seen?.SeenDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                            SeenDateTime = seen?.SeenDate,
                            SeenTimeDisplay = seen?.SeenDate.ToString("HH:mm", new CultureInfo("fa-IR")),
                            StatusCode = status.StatusCode,
                            StatusDisplayTitle = status.DisplayTitle,
                            StatusTitle = status.Title,
                            TicketNumber = ticket.TicketNumber,
                            Title = ticket.Title,
                            TrackingCode = ticket.TrackingCode,
                            Urgent = ticket.Urgent,
                            Technician = technicianUser == null ? null : new TicketUserAndTechnicianDTO
                            {
                                Email = technicianUser.Email,
                                FirstName = technicianUser.FirstName,
                                GenderType = technicianUser.Gender,
                                Gender = technicianUser.Gender switch
                                {
                                    null => "آقا/خانم",
                                    GenderType.ALL => "آقا/خانم",
                                    GenderType.MALE => "آقا",
                                    GenderType.FAMALE => "خانم",
                                    _ => "آقا/خانم",
                                },
                                Id = technicianUser.Id,
                                LastName = technicianUser.LastName,
                                PhoneNumber = technicianUser.PhoneNumber,
                            },
                            Unit = new TicketUnitDTO
                            {
                                Block = unit.Block,
                                ComplexId = unit.ComplexId,
                                Floor = unit.Floor,
                                Id = unit.Id,
                                Name = unit.Name
                            },
                            User = new TicketUserAndTechnicianDTO
                            {
                                Email = user.Email,
                                FirstName = user.FirstName,
                                GenderType = user.Gender,
                                Gender = user.Gender switch
                                {
                                    null => "آقا/خانم",
                                    GenderType.ALL => "آقا/خانم",
                                    GenderType.MALE => "آقا",
                                    GenderType.FAMALE => "خانم",
                                    _ => "آقا/خانم",
                                },
                                Id = user.Id,
                                LastName = user.LastName,
                                PhoneNumber = user.PhoneNumber,
                            }
                        }).OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                if (filter.PageNumber == null || filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1;
                }
                list = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                        join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.StatusId equals status.Id
                        join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.UserId equals user.Id
                        join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.UnitId equals unit.Id
                        join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.Id equals seen.TicketId
                        into tickets
                        from seen in tickets.DefaultIfEmpty()
                        join technician in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                        on ticket.TechnicianId equals technician.Id
                        into ticketsFull
                        from technicianUser in ticketsFull.DefaultIfEmpty()
                        where
                            ticket.UserId == UserId &&
                            (filter.TicketId == null || ticket.Id == Convert.ToInt64(filter.TicketId)) &&
                            (filter.TrackingCode == null || (!string.IsNullOrEmpty(ticket.TrackingCode) && !string.IsNullOrWhiteSpace(ticket.TrackingCode) && ticket.TrackingCode.ToUpper().StartsWith(filter.TrackingCode.ToUpper()))) &&
                            (filter.TicketNumber == null || (!string.IsNullOrEmpty(ticket.TicketNumber) && !string.IsNullOrWhiteSpace(ticket.TicketNumber) && ticket.TicketNumber.ToUpper().StartsWith(filter.TicketNumber.ToUpper()))) &&
                            (filter.Title == null || ticket.Title.ToUpper().StartsWith(filter.Title.ToUpper())) &&
                            (filter.CreatedAt == null || ticket.CreatedAt.ResetTime() == filter.CreatedAt.ResetTime()) &&
                            (filteredStatusId == null || ticket.StatusId == filteredStatusId) &&
                            (filter.UnitId == null || ticket.UnitId == filter.UnitId) &&
                            (filter.Seen == null || (filter.Seen == true ? seen != null : seen == null))
                        select new TicketDomainDTO
                        {
                            CreatedAt = ticket.CreatedAt,
                            Description = ticket.Description,
                            Id = ticket.Id,
                            ModifyDate = ticket.ModifyDate,
                            Seen = seen != null,
                            SeenDateDisplay = seen?.SeenDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                            SeenDateTime = seen?.SeenDate,
                            SeenTimeDisplay = seen?.SeenDate.ToString("HH:mm", new CultureInfo("fa-IR")),
                            StatusCode = status.StatusCode,
                            StatusDisplayTitle = status.DisplayTitle,
                            StatusTitle = status.Title,
                            TicketNumber = ticket.TicketNumber,
                            Title = ticket.Title,
                            TrackingCode = ticket.TrackingCode,
                            Urgent = ticket.Urgent,
                            Technician = technicianUser == null ? null : new TicketUserAndTechnicianDTO
                            {
                                Email = technicianUser.Email,
                                FirstName = technicianUser.FirstName,
                                GenderType = technicianUser.Gender,
                                Gender = technicianUser.Gender switch
                                {
                                    null => "آقا/خانم",
                                    GenderType.ALL => "آقا/خانم",
                                    GenderType.MALE => "آقا",
                                    GenderType.FAMALE => "خانم",
                                    _ => "آقا/خانم",
                                },
                                Id = technicianUser.Id,
                                LastName = technicianUser.LastName,
                                PhoneNumber = technicianUser.PhoneNumber,
                            },
                            Unit = new TicketUnitDTO
                            {
                                Block = unit.Block,
                                ComplexId = unit.ComplexId,
                                Floor = unit.Floor,
                                Id = unit.Id,
                                Name = unit.Name
                            },
                            User = new TicketUserAndTechnicianDTO
                            {
                                Email = user.Email,
                                FirstName = user.FirstName,
                                GenderType = user.Gender,
                                Gender = user.Gender switch
                                {
                                    null => "آقا/خانم",
                                    GenderType.ALL => "آقا/خانم",
                                    GenderType.MALE => "آقا",
                                    GenderType.FAMALE => "خانم",
                                    _ => "آقا/خانم",
                                },
                                Id = user.Id,
                                LastName = user.LastName,
                                PhoneNumber = user.PhoneNumber,
                            }
                        }).OrderByDescending(x => x.CreatedAt).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList();
            }

            return Op.Succeed("دریافت لیست درخواست ها با موفقیت انجام شد", new Response_GetTicketDomainDTO
            {
                TotalCount = totalCount,
                Tickets = list
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست درخواست ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<Response_GetSingleTicketDomainDTO>> GetTicketMessages(int UserId, bool isAdmin, long TicketId, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetSingleTicketDomainDTO> Op = new("GetTicketMessagesByAdmin");
        if (UserId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (TicketId <= 0)
        {
            return Op.Failed("شناسه درخواست بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == UserId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Tickets.AnyAsync(x => x.Id == TicketId && (isAdmin || x.UserId == UserId), cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه درخواست بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            Response_GetSingleTicketDomainDTO? result = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
                                                         join status in await _context.TicketsStatus.ToListAsync(cancellationToken: cancellationToken)
                                                         on ticket.StatusId equals status.Id
                                                         join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                         on ticket.UserId equals user.Id
                                                         join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                                         on ticket.UnitId equals unit.Id
                                                         join seen in await _context.TicketsSeen.ToListAsync(cancellationToken: cancellationToken)
                                                         on ticket.Id equals seen.TicketId
                                                         into tickets
                                                         from seen in tickets.DefaultIfEmpty()
                                                         join technician in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                         on ticket.TechnicianId equals technician.Id
                                                         into ticketsFull
                                                         from technicianUser in ticketsFull.DefaultIfEmpty()
                                                         where ticket.Id == TicketId
                                                         select new Response_GetSingleTicketDomainDTO
                                                         {
                                                             CreatedAt = ticket.CreatedAt,
                                                             FullCreatedAddTitle = ticket.CreatedAt.GetPersianFullDate(),
                                                             Description = ticket.Description,
                                                             Id = ticket.Id,
                                                             ModifyDate = ticket.ModifyDate,
                                                             Seen = seen != null,
                                                             SeenDateDisplay = seen?.SeenDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                                                             SeenDateTime = seen?.SeenDate,
                                                             SeenTimeDisplay = seen?.SeenDate.ToString("HH:mm", new CultureInfo("fa-IR")),
                                                             StatusCode = status.StatusCode,
                                                             StatusDisplayTitle = status.DisplayTitle,
                                                             StatusTitle = status.Title,
                                                             TicketNumber = ticket.TicketNumber,
                                                             Title = ticket.Title,
                                                             TrackingCode = ticket.TrackingCode,
                                                             Urgent = ticket.Urgent,
                                                             Technician = technicianUser == null ? null : new TicketUserAndTechnicianDTO
                                                             {
                                                                 Email = technicianUser.Email,
                                                                 FirstName = technicianUser.FirstName,
                                                                 GenderType = technicianUser.Gender,
                                                                 Gender = technicianUser.Gender switch
                                                                 {
                                                                     null => "آقا/خانم",
                                                                     GenderType.ALL => "آقا/خانم",
                                                                     GenderType.MALE => "آقا",
                                                                     GenderType.FAMALE => "خانم",
                                                                     _ => "آقا/خانم",
                                                                 },
                                                                 Id = technicianUser.Id,
                                                                 LastName = technicianUser.LastName,
                                                                 PhoneNumber = technicianUser.PhoneNumber,
                                                             },
                                                             Unit = new TicketUnitDTO
                                                             {
                                                                 Block = unit.Block,
                                                                 ComplexId = unit.ComplexId,
                                                                 Floor = unit.Floor,
                                                                 Id = unit.Id,
                                                                 Name = unit.Name
                                                             },
                                                             User = new TicketUserAndTechnicianDTO
                                                             {
                                                                 Email = user.Email,
                                                                 FirstName = user.FirstName,
                                                                 GenderType = user.Gender,
                                                                 Gender = user.Gender switch
                                                                 {
                                                                     null => "آقا/خانم",
                                                                     GenderType.ALL => "آقا/خانم",
                                                                     GenderType.MALE => "آقا",
                                                                     GenderType.FAMALE => "خانم",
                                                                     _ => "آقا/خانم",
                                                                 },
                                                                 Id = user.Id,
                                                                 LastName = user.LastName,
                                                                 PhoneNumber = user.PhoneNumber,
                                                             }
                                                         }).FirstOrDefault();
            if (result == null)
            {
                return Op.Failed("دریافت اطلاعات درخواست با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (await _context.TicketMessages.AnyAsync(x => x.TicketId == TicketId, cancellationToken: cancellationToken))
            {
                result.Messages = (from message in await _context.TicketMessages.ToListAsync(cancellationToken: cancellationToken)
                                   join author in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                   on message.AuthorId equals author.Id
                                   join attach in await _context.Attachments.ToListAsync(cancellationToken: cancellationToken)
                                   on message.AttachmentId equals attach.Id
                                   into messageResult
                                   from attachment in messageResult.DefaultIfEmpty()
                                   where message.TicketId == TicketId
                                   select new TicketMessageDomainDTO
                                   {
                                       AttachmentUrl = attachment == null ? null : $"https://app.enjoylife.ir/filestorage/{attachment.Url}",
                                       Author = new TicketUserAndTechnicianDTO
                                       {
                                           Email = author.Email,
                                           FirstName = author.FirstName,
                                           Gender = author.Gender switch
                                           {
                                               null => "آقا/خانم",
                                               GenderType.ALL => "آقا/خانم",
                                               GenderType.MALE => "آقا",
                                               GenderType.FAMALE => "خانم",
                                               _ => "آقا/خانم",
                                           },
                                           GenderType = author.Gender,
                                           Id = author.Id,
                                           LastName = author.LastName,
                                           PhoneNumber = author.PhoneNumber
                                       },
                                       CreatedAt = message.CreatedAt,
                                       CreatedAtDateDisplay = message.CreatedAt.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                                       CreatedAtTimeDisplay = message.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
                                       Id = message.Id,
                                       Text = message.Text

                                   }).OrderBy(x => x.CreatedAt).ToList();
                result.LastResponseDate = result.Messages.Last().CreatedAt;
                result.LastResponseDateDisplay = Convert.ToDateTime(result.LastResponseDate).ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                result.LastResponseTimeDisplay = Convert.ToDateTime(result.LastResponseDate).ToString("HH:mm", new CultureInfo("fa-IR"));
            }
            return Op.Succeed("دریافت اطلاعات درخواست با موفقیت انجام شد", result);

        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات درخواست با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }


    //public async Task<TicketModel> SaveTicketAsync(TicketModel ticket, CancellationToken cancellationToken = default)
    //{
    //    await _context.Tickets.AddAsync(ticket, cancellationToken);
    //    await _context.SaveChangesAsync(cancellationToken);

    //    //add log
    //    var addedTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.CreatedAt == ticket.CreatedAt, cancellationToken: cancellationToken);
    //    if (addedTicket != null)
    //    {
    //        await _context.TicketStatusLogs.AddAsync(new TicketStatusLog
    //        {
    //            MidificationDate = ticket.CreatedAt,
    //            ModifyByAdmin = false,
    //            StatusCode = this.GetTicketStatusCode(TicketStatus.PENDING),
    //            StatusTitle = this.GetTicketStatusName(TicketStatus.PENDING),
    //            StatusDisplayTitle = this.GetTicketStatusDisplayTitle(TicketStatus.PENDING),
    //            TicketID = addedTicket.Id,
    //        }, cancellationToken);
    //        await _context.SaveChangesAsync(cancellationToken);
    //    }
    //    //end add log

    //    return ticket;
    //}
    //public async Task<int> SaveTicketVisitTimeAsync(TicketVisitTimeModel TicketVisitTime, CancellationToken cancellationToken = default)
    //{
    //    await _context.TicketVisitTime.AddAsync(TicketVisitTime, cancellationToken);
    //    await _context.SaveChangesAsync(cancellationToken);
    //    return TicketVisitTime.Id;
    //}

    //public async Task<TicketMessageModel> SaveTicketMessageAsync(TicketMessageModel ticketMessage, CancellationToken cancellationToken = default)
    //{
    //    await _context.TicketMessages.AddAsync(ticketMessage, cancellationToken);
    //    await _context.SaveChangesAsync(cancellationToken);
    //    return ticketMessage;
    //}

    //public async Task<List<TicketMessageModel>> GetTicketMessages(int TicketId, CancellationToken cancellationToken = default, string sortOrder = "desc")
    //{
    //    IQueryable<TicketMessageModel> query = _context.TicketMessages
    //        .Where(m => m.Ticket.Id == TicketId)
    //        .Include(m => m.Author)
    //        .Include(m => m.Attachment);

    //    switch (sortOrder)
    //    {
    //        case "asc":
    //            query = query.OrderBy(m => m.CreatedAt);
    //            break;

    //        case "desc":
    //            query = query.OrderByDescending(m => m.CreatedAt);
    //            break;
    //        default:
    //            query = query.OrderByDescending(m => m.CreatedAt);
    //            break;
    //    }

    //    var result = await query.ToListAsync(cancellationToken);
    //    return result;
    //}

    //public async Task<List<TicketModel>> GetTicketsByUser(int userId, GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default)
    //{

    //    var query = _context.Tickets.Where(t => t.User.Id == userId);

    //    if (queryParams.Open is not null)
    //        if ((bool)queryParams.Open) query = query.Where(t => t.TicketStatus != TicketStatus.ADMINCLOSED && t.TicketStatus != TicketStatus.USERCLOSED);
    //        else query = query.Where(t => t.TicketStatus == TicketStatus.USERCLOSED || t.TicketStatus == TicketStatus.ADMINCLOSED);

    //    if (queryParams.Urgent is not null) query = query.Where(t => t.Urgent == queryParams.Urgent);

    //    if (queryParams.Pending is not null)
    //        if ((bool)queryParams.Pending) query = query.Where(t => t.TicketStatus == TicketStatus.PENDING);
    //        else query = query.Where(t => t.TicketStatus == TicketStatus.ONGOING);

    //    return await query.OrderByDescending(t => t.CreatedAt).Include(t => t.Technician).Include(m => m.Attachment).Include(t => t.Unit).Include(t => t.User).ToListAsync(cancellationToken);
    //}

    //public async Task<TicketModel?> GetTicketByIdAsync(int ticketId, int userId = 0, CancellationToken cancellationToken = default)
    //{
    //    if (await _context.Tickets.AnyAsync(t => t.Id == ticketId && (userId <= 0 || t.User == null || t.User.Id == userId), cancellationToken: cancellationToken))
    //    {
    //        return await _context.Tickets
    //            .Include(x => x.User)
    //            .Include(x => x.Unit)
    //            .Include(x => x.Technician)
    //            .Include(x => x.Attachment)
    //            .FirstOrDefaultAsync(t => t.Id == ticketId && (userId <= 0 || t.User == null || t.User.Id == userId), cancellationToken: cancellationToken);
    //    }
    //    return null;
    //}

    //public async Task<List<TicketVisitTimeModel>>? GetTicketsVisitTimesAsync(GetTicketVisitSearchDTO dto, CancellationToken cancellationToken = default)
    //{
    //    //Expression<Func<TicketVisitTimeModel, bool>> xFilter = (bool)dto.OnGoing ? 
    //    //    x => x.Ticket.TicketStatus == TicketStatus.PENDING || x.Ticket.TicketStatus==TicketStatus.ONGOING:
    //    //    x => x.Ticket.TicketStatus != TicketStatus.PENDING || x.Ticket.TicketStatus == TicketStatus.ONGOING;

    //    return await _context.TicketVisitTime.Where(t =>
    //    (dto.TicketId == null || t.Ticket.Id == dto.TicketId) &&
    //    (dto.From == null || (t.VisitTime > dto.From && t.VisitTime <= dto.To))
    //    ).Include(t => t.Ticket).ThenInclude(u => u.User)
    //        .OrderByDescending(o => o.VisitTime)
    //        .ToListAsync(cancellationToken);
    //}

    //public async Task<(List<TicketModelDisplayDTO> Tickets, int TotalCount)> GetAllTicketAsync(int userId, GetAdminTicketSearchDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    int totalCount = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
    //                      join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
    //                      on ticket.User equals user
    //                      join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
    //                      on ticket.Unit equals unit
    //                      where
    //                      (requestDTO.TrackCode == null || ticket.TrackingCode.ToString().Contains(requestDTO.TrackCode.ToString())) &&
    //                      ((string.IsNullOrEmpty(requestDTO.Title) || string.IsNullOrEmpty(requestDTO.Title) || ticket.Title.Contains(requestDTO.Title))) &&
    //                      (requestDTO.UserId == null || user.Id == requestDTO.UserId) &&
    //                      (requestDTO.UnitId == null || unit.Id == requestDTO.UnitId) &&
    //                      (!requestDTO.Status.HasValue || ticket.TicketStatus == requestDTO.Status.Value) &&
    //                      (requestDTO.CreateDate == null || ticket.CreatedAt.Date == requestDTO.CreateDate)
    //                      select ticket.Id).ToList().Count;
    //    if (totalCount == 0)
    //    {
    //        return (new List<TicketModelDisplayDTO>(), 0);
    //    }

    //    var tickets = (from ticket in await _context.Tickets.ToListAsync(cancellationToken: cancellationToken)
    //                   join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
    //                   on ticket.User equals user
    //                   join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
    //                   on ticket.Unit equals unit
    //                   where
    //                   (requestDTO.TrackCode == null || ticket.TrackingCode.ToString().Contains(requestDTO.TrackCode.ToString())) &&
    //                   ((string.IsNullOrEmpty(requestDTO.Title) || string.IsNullOrEmpty(requestDTO.Title) || ticket.Title.Contains(requestDTO.Title))) &&
    //                   (requestDTO.UserId == null || user.Id == requestDTO.UserId) &&
    //                   (requestDTO.UnitId == null || unit.Id == requestDTO.UnitId) &&
    //                   (!requestDTO.Status.HasValue || ticket.TicketStatus == requestDTO.Status.Value) &&
    //                   (requestDTO.CreateDate == null || ticket.CreatedAt.Date == requestDTO.CreateDate)
    //                   select new TicketModelDisplayDTO
    //                   {
    //                       CreatedAt = ticket.CreatedAt,
    //                       Description = ticket.Description,
    //                       Id = ticket.Id,
    //                       ModifyDate = ticket.ModifyDate,
    //                       TicketNumber = ticket.TicketNumber,
    //                       TicketStatus = ticket.TicketStatus,
    //                       Title = ticket.Title,
    //                       TrackingCode = ticket.TrackingCode,
    //                       Unit = new domain.displayEntities.structureModels.UnitModelDisplayDTO
    //                       {
    //                           Block = unit.Block,
    //                           ComplexId = unit.ComplexId,
    //                           Directions = unit.Directions,
    //                           Floor = unit.Floor,
    //                           Id = unit.Id,
    //                           Meterage = unit.Meterage,
    //                           Name = unit.Name,
    //                           Positions = unit.Positions,
    //                           Type = unit.Type,
    //                           UnitElectricityCounterNumber = unit.UnitElectricityCounterNumber,
    //                           UnitPLanMapFileUrl = unit.UnitPLanMapFileUrl,
    //                           UnitUsages = unit.UnitUsages,
    //                       },
    //                       Urgent = ticket.Urgent,
    //                       User = new domain.displayEntities.partyModels.UserModelDisplayDTO
    //                       {
    //                           Address = user.Address,
    //                           Age = user.Age,
    //                           Email = user.Email,
    //                           FirstName = user.FirstName,
    //                           Gender = user.Gender,
    //                           Id = user.Id,
    //                           LastName = user.LastName,
    //                           NationalID = user.NationalID,
    //                           PhoneNumber = user.PhoneNumber,
    //                           PhoneNumberConfirmed = user.PhoneNumberConfirmed,
    //                       },
    //                       VisitTime = ticket.VisitTime,
    //                   }).Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize).Take(requestDTO.PageSize).ToList();

    //    var seens = await this.GetSeenedByUserId(userId, cancellationToken);
    //    for (int i = 0; i < tickets.Count; i++)
    //    {
    //        if (seens.Any(x => x.TicketId == tickets[i].Id))
    //        {
    //            var seen = seens.FirstOrDefault(x => x.TicketId == tickets[i].Id);
    //            tickets[i].Seen = true;
    //            tickets[i].SeenDate = seen.SeenDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
    //            tickets[i].SeenTime = seen.SeenDate.ToString("HH:mm", new CultureInfo("fa-IR"));
    //        }
    //        else
    //        {
    //            tickets[i].Seen = false;
    //        }
    //    }

    //    return (tickets.OrderBy(x => x.Seen).ThenByDescending(x => x.CreatedAt).ToList(), totalCount);

    //}

    //public async Task<TicketModel> UpdateTicketAsync(TicketModel ticketModel, bool modifyByAdmin, string? closeMessage, CancellationToken cancellationToken = default)
    //{
    //    if (await _context.Tickets.AnyAsync(x => x.TrackingCode == ticketModel.TrackingCode && x.Id != ticketModel.Id, cancellationToken: cancellationToken))
    //    {
    //        throw new Exception($"{ticketModel.TrackingCode} have confilict");
    //    }
    //    else
    //    {
    //        _context.Tickets.Update(ticketModel);

    //        //add log

    //        await _context.TicketStatusLogs.AddAsync(new TicketStatusLog
    //        {
    //            Message = ticketModel.TicketStatus == TicketStatus.ADMINCLOSED || ticketModel.TicketStatus == TicketStatus.USERCLOSED ? !string.IsNullOrEmpty(closeMessage) && !string.IsNullOrWhiteSpace(closeMessage) ? closeMessage.Trim() : null : null,
    //            MidificationDate = DateTime.Now,
    //            ModifyByAdmin = modifyByAdmin,
    //            StatusCode = this.GetTicketStatusCode(ticketModel.TicketStatus == TicketStatus.ADMINCLOSED || ticketModel.TicketStatus == TicketStatus.USERCLOSED ? modifyByAdmin ? TicketStatus.ADMINCLOSED : TicketStatus.USERCLOSED : ticketModel.TicketStatus),
    //            StatusTitle = this.GetTicketStatusName(ticketModel.TicketStatus == TicketStatus.ADMINCLOSED || ticketModel.TicketStatus == TicketStatus.USERCLOSED ? modifyByAdmin ? TicketStatus.ADMINCLOSED : TicketStatus.USERCLOSED : ticketModel.TicketStatus),
    //            StatusDisplayTitle = this.GetTicketStatusDisplayTitle(ticketModel.TicketStatus == TicketStatus.ADMINCLOSED || ticketModel.TicketStatus == TicketStatus.USERCLOSED ? modifyByAdmin ? TicketStatus.ADMINCLOSED : TicketStatus.USERCLOSED : ticketModel.TicketStatus),
    //            TicketID = ticketModel.Id,
    //        }, cancellationToken);

    //        //end add log

    //        await _context.SaveChangesAsync(cancellationToken);
    //        return ticketModel;
    //    }

    //}

    //public async Task<TicketMessageModel> UpdateMessageAsync(TicketMessageModel ticketMessageModel, CancellationToken cancellationToken = default)
    //{
    //    _context.TicketMessages.Update(ticketMessageModel);
    //    await _context.SaveChangesAsync(cancellationToken);
    //    return ticketMessageModel;
    //}

    //public async Task<TicketMessageModel?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken = default)
    //{
    //    return await _context.TicketMessages.Where(m => m.Id == messageId).Include(m => m.Author).Include(m => m.Ticket).SingleOrDefaultAsync(cancellationToken);
    //}
    //public async Task<int> DeleteVisitTimeByIdAsync(int visitId, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        //using (_context)
    //        //{
    //        //   cancellationToken.ThrowIfCancellationRequested();
    //        var visit = await _context.TicketVisitTime.SingleOrDefaultAsync(p => p.Id == visitId) ?? throw new Exception("visit not found");
    //        _context.TicketVisitTime.Remove(visit);
    //        return await _context.SaveChangesAsync();
    //        // }
    //    }
    //    //catch (OperationCanceledException ee)
    //    //{
    //    //    var gg = ee.Message;
    //    //    return 0;
    //    //}
    //    catch (Exception ex)
    //    {
    //        var ff = ex.Message;
    //        return 0;
    //    }
    //}

    //public string GetTicketStatusDisplayTitle(TicketStatus status)
    //{
    //    return status switch
    //    {
    //        TicketStatus.PENDING => "در حال بررسی",
    //        TicketStatus.ONGOING => "در دست اقدام",
    //        TicketStatus.SUCCESS => "انجام شده",
    //        TicketStatus.ADMINCLOSED => "لغو شده توسط ادمین",
    //        TicketStatus.USERCLOSED => "لغو شده توسط کاربر",
    //        _ => string.Empty,
    //    };
    //}
    //public string GetTicketStatusName(TicketStatus status)
    //{
    //    return status switch
    //    {
    //        TicketStatus.PENDING => "PENDING",
    //        TicketStatus.ONGOING => "ONGOING",
    //        TicketStatus.SUCCESS => "SUCCESS",
    //        TicketStatus.ADMINCLOSED => "ADMINCLOSED",
    //        TicketStatus.USERCLOSED => "USERCLOSED",
    //        _ => string.Empty,
    //    };
    //}
    //public int GetTicketStatusCode(TicketStatus status)
    //{
    //    return status switch
    //    {
    //        TicketStatus.PENDING => 0,
    //        TicketStatus.ONGOING => 1,
    //        TicketStatus.SUCCESS => 2,
    //        TicketStatus.ADMINCLOSED => 3,
    //        TicketStatus.USERCLOSED => 4,
    //        _ => -1,
    //    };
    //}

    //public async Task<string> SeenTicket(int userId, int ticketId, CancellationToken cancellationToken = default)
    //{
    //    if (ticketId <= 0)
    //    {
    //        return "ticket id is not valid";
    //    }
    //    if (userId <= 0)
    //    {
    //        return "user id is not valid";
    //    }
    //    try
    //    {
    //        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);
    //        if (ticket == null)
    //        {
    //            return "ticket id is not valid";
    //        }
    //        if (!await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken: cancellationToken))
    //        {
    //            return "user id is not valid";
    //        }
    //        if (await _context.TicketSeens.AnyAsync(x => x.TicketId == ticketId && x.UserId == userId, cancellationToken: cancellationToken))
    //        {
    //            return string.Empty;
    //        }
    //        await _context.TicketSeens.AddAsync(new TicketSeen
    //        {
    //            SeenDate = DateTime.Now,
    //            TicketId = ticketId,
    //            UserId = userId
    //        }, cancellationToken);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return string.Empty;
    //    }
    //    catch (Exception ex)
    //    {
    //        return $"operation failed : {ex.Message}";
    //    }

    //}

    //public async Task<List<TicketSeen>> GetSeenedByUserId(int userId, CancellationToken cancellationToken = default)
    //{
    //    if (userId <= 0)
    //    {
    //        return new List<TicketSeen>();
    //    }
    //    try
    //    {
    //        if (!await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken: cancellationToken))
    //        {
    //            return new List<TicketSeen>();
    //        }
    //        var result = await _context.TicketSeens.Where(x => x.UserId == userId).ToListAsync(cancellationToken: cancellationToken);
    //        if (result == null || !result.Any())
    //        {
    //            return new List<TicketSeen>();
    //        }
    //        return result;
    //    }
    //    catch (Exception)
    //    {
    //        return new List<TicketSeen>();
    //    }
    //}
}
