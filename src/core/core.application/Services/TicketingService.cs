using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.API.DTO.Ticket.Message;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Contract.Infrastructure;
using core.application.Exceptions;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using common.defination.Utility;
using core.application.Contract.API.Mapper;
using System.Linq;
using System.Globalization;
using core.application.Contract.infrastructure.Services;
using core.application.Contract.API.DTO.Messaging;
using core.domain.entity.enums;
using core.application.Helper;
using core.domain.displayEntities.ticketingModels;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs;
using core.application.Framework;

namespace core.application.Services;

public class TicketingService : ITicketingService
{
    private readonly ITicketingRepository _ticketingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMessagingService _messagingService;
    public TicketingService(ITicketingRepository ticketingRepository, IUserRepository userRepository, IUnitRepository unitRepository, IRepository repository, IFileStorageService fileStorageService, IMessagingService messagingService)
    {
        _ticketingRepository = ticketingRepository;
        _userRepository = userRepository;
        _unitRepository = unitRepository;
        _repository = repository;
        _fileStorageService = fileStorageService;
        _messagingService = messagingService;

    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> CreateTicket(int UserId, CreateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        var operation = await _ticketingRepository.CreateTicket(UserId, new Request_CreateTicketDomainDTO
        {
            Description = requestDTO.Description,
            Title = requestDTO.Title,
            UnitId = requestDTO.UnitId,
            Urgent = requestDTO.Urgent,
            Url = requestDTO.Url,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CommonOperationTicketDomainDTO result = operation.Object;
            _messagingService.SendTicketAdminSMS(new TicketAdminSMSDTO
            {
                Title = result.Title,
                CreateDate = result.CreatedAt.ToString("yy/MM/dd - HH:mm", new CultureInfo("fa-IR")),
                CreatedBy = result.CreatedBy,
                AttchmentUrl = string.IsNullOrEmpty(result.AttachmentUrl) || string.IsNullOrWhiteSpace(result.AttachmentUrl) ? string.Empty : $"https://app.enjoylife.ir/filestorage/{result.AttachmentUrl}",
                Description = result.Description,
                UnitName = result.UnitName,
            }, cancellationToken);

            _messagingService.SendTicketUserSMS(new List<string>() { result.CreatedByPhoneNumber }, cancellationToken);
        }
        return operation;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> ActivateTicket(int UserId, ActivateTicketRequestDTO model, CancellationToken cancellationToken = default)
    {
        var operation = await _ticketingRepository.ActivateTicket(UserId, new Request_ActivateTicketDomainDTO
        {
            Id = model.Id,
            TicketNumber = model.TicketNumber,
            TrackingCode = model.TrackingCode,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CommonOperationTicketDomainDTO result = operation.Object;
            _messagingService.SendTicektActivationSMS(new List<string>() { result.CreatedByPhoneNumber }, new Contract.API.DTO.Messaging.TicketActivationSMSDTO
            {
                TicketId = result.TicketId.ToString(),
                Title = result.Title,
                TrackingCode = result.TrackingCode
            }, cancellationToken);
        }
        return operation;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SucceedTicket(int UserId, ModifyTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        return await _ticketingRepository.SucceedTicket(UserId, new Request_ModifyTicketDomainDTO
        {
            Message = requestDTO.Message,
            TicketId = requestDTO.TicketId,
        }, cancellationToken);
    }

    public async Task<OperationResult<Response_CloseTicketDomainDTO>> CloseTicket(int UserId, bool isAdmin, ModifyTicketRequestDTO model, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(model.Message) && !string.IsNullOrWhiteSpace(model.Message))
        {
            await _ticketingRepository.CreateMessage(UserId, isAdmin, new Request_CreateTicketMessageDomainDTO
            {
                TicketId = model.TicketId,
                Text = model.Message,
                Url = null
            }, cancellationToken);
        }

        var operation = await _ticketingRepository.CloseTicket(UserId, isAdmin, new Request_ModifyTicketDomainDTO
        {
            Message = model.Message,
            TicketId = model.TicketId,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CloseTicketDomainDTO result = operation.Object;
            _messagingService.SendTicketClosingToAdminsSMS(new TicketUpdateToCloseAdminSMSDTO
            {
                CancelationType = isAdmin ? "لغو توسط ادمین" : "لغو توسط کاربر",
                ClosedDate = operation.OperationDate.GetPersianShortDate().En2Fa(),
                CloseMessage = result.CloseMessage,
                CreatedDate = result.CreatedAt.GetPersianFullDate().En2Fa(),
                TicketNumber = result.TicketNumber,
                Title = result.Title,
                TrackingCode = string.IsNullOrEmpty(result.TrackingCode) || string.IsNullOrWhiteSpace(result.TrackingCode) ? "ثبت نشده" : result.TrackingCode.Trim(),
                UnitName = result.UnitName
            }, cancellationToken);

            _messagingService.SendTicketClosingToCustomerSMS(result.CreatedByPhoneNumber, new TicketUpdateToCloseCustomerSMSDTO
            {
                CancelationType = isAdmin ? "لغو توسط ادمین" : "لغو توسط کاربر",
                ClosedDate = operation.OperationDate.GetPersianShortDate().En2Fa(),
                CloseMessage = result.CloseMessage,
                CreatedDate = result.CreatedAt.GetPersianFullDate().En2Fa(),
                TicketNumber = result.TicketNumber,
                Title = result.Title,
                TrackingCode = string.IsNullOrEmpty(result.TrackingCode) || string.IsNullOrWhiteSpace(result.TrackingCode) ? "ثبت نشده" : result.TrackingCode.Trim(),
                UnitName = result.UnitName,
                GenderType = result.CreatedByGenderType switch
                {
                    null => "آقای/خانم",
                    GenderType.ALL => "آقای/خانم",
                    GenderType.MALE => "آقای",
                    GenderType.FAMALE => "خانم",
                    _ => "آقای/خانم"
                },
                UserFullName = result.CreatedBy
            }, cancellationToken);
        }
        return operation;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTrackCode(int UserId, UpdateTrackingCodeDTO model, CancellationToken cancellationToken = default)
    {
        var operation = await _ticketingRepository.UpdateTrackCode(UserId, new Request_UpdateTrackCodeDomainDTO
        {
            Id = model.Id,
            TrackingCode = model.TrackingCode,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CommonOperationTicketDomainDTO result = operation.Object;
            _messagingService.SendTicektUpdateSMS(new List<string>() { result.CreatedByPhoneNumber }, new Contract.API.DTO.Messaging.TicketActivationSMSDTO
            {
                TicketId = result.TicketId.ToString(),
                Title = result.Title,
                TrackingCode = result.TrackingCode,
            }, cancellationToken);
        }
        return operation;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> UpdateTicketNumber(int UserId, UpdateTicketNumberDTO requestDTO, CancellationToken cancellationToken = default)
    {
        var operation = await _ticketingRepository.UpdateTicketNumber(UserId, new Request_UpdateTicketNumberDomainDTO
        {
            Id = requestDTO.Id,
            TicketNumber = requestDTO.TicketNumber,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CommonOperationTicketDomainDTO result = operation.Object;
            _messagingService.SendTicketUpdateNumberSMS(new List<string>() { result.CreatedByPhoneNumber }, new Contract.API.DTO.Messaging.TicketUpdateNumberSMSDTO
            {
                TicketId = result.TicketId.ToString(),
                Title = result.Title,
                TicketNumber = result.TicketNumber,
            }, cancellationToken);
        }
        return operation;
    }

    public async Task<OperationResult<Response_CommonOperationTicketDomainDTO>> SeenTicket(int UserId, SeenTicketDTO requestDTO, CancellationToken cancellationToken = default)
    {
        return await _ticketingRepository.SeenTicket(UserId, new Request_TicketId
        {
            TicketId = requestDTO.TicketId,
        }, cancellationToken);
    }

    public async Task<OperationResult<Response_CreateTicketMessageDomainDTO>> CreateMessage(int UserId, bool isAdmin, CreateMessageRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        var operation = await _ticketingRepository.CreateMessage(UserId, isAdmin, new Request_CreateTicketMessageDomainDTO
        {
            Text = requestDTO.Text,
            TicketId = requestDTO.TicketId,
            Url = requestDTO.Url,
        }, cancellationToken);
        if (operation.Success)
        {
            Response_CreateTicketMessageDomainDTO result = operation.Object;
            if (isAdmin)
            {
                _messagingService.SendTicketMessageSMS(result.CreatedByPhoneNumber, new TicketMessageSMSDTO()
                {
                    GenderType = result.CreatedByGenderType switch
                    {
                        null => "آقای/خانم",
                        GenderType.ALL => "آقای/خانم",
                        GenderType.MALE => "آقای",
                        GenderType.FAMALE => "خانم",
                        _ => "آقای/خانم",
                    },
                    TicketId = result.TicketId.ToString(),
                    TicketTitle = result.Title,
                    TrackingCode = string.IsNullOrEmpty(result.TrackingCode) || string.IsNullOrWhiteSpace(result.TrackingCode) ? "ثبت نشده" : result.TrackingCode,
                    UserFullName = result.CreatedBy
                }, cancellationToken);
            }
            else
            {
                _messagingService.SendTicketMessageAdminSMS(new TicketMessageAdminDTO
                {
                    Title = result.Title,
                    Text = string.IsNullOrEmpty(result.MessageText) || string.IsNullOrWhiteSpace(result.MessageText) ? "بدون متن" : result.MessageText.Trim(),
                    CreatedBy = result.CreatedBy,
                    CreatedDate = result.CreatedAt.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
                    TrackingCode = string.IsNullOrEmpty(result.TrackingCode) || string.IsNullOrWhiteSpace(result.TrackingCode) ? "ثبت نشده" : result.TrackingCode,
                    UnitName = result.UnitName,
                    AttachmentUrl = string.IsNullOrEmpty(result.AttachmentUrl) || string.IsNullOrWhiteSpace(result.AttachmentUrl) ? "بدون تصویر" : $"https://app.enjoylife.ir/filestorage/{result.AttachmentUrl}"
                }, cancellationToken);
            }
        }
        return operation;
    }

    public async Task<OperationResult<Response_GetTicketDomainDTO>> GetTicketsByAdmin(int UserId, Filter_GetTicketAdminDomainDTO filter, CancellationToken cancellationToken = default)
    {
        return await _ticketingRepository.GetTicketsByAdmin(UserId, filter, cancellationToken);
    }
    public async Task<OperationResult<Response_GetTicketDomainDTO>> GetTickets(int UserId, Filter_GetTicketUserDomainDTO filter, CancellationToken cancellationToken = default)
    {
        return await _ticketingRepository.GetTickets(UserId, filter, cancellationToken);
    }

    public async Task<OperationResult<Response_GetSingleTicketDomainDTO>> GetTicketMessages(int UserId, bool isAdmin, long TicketId, CancellationToken cancellationToken = default)
    {
        return await _ticketingRepository.GetTicketMessages(UserId, isAdmin, TicketId, cancellationToken);
    }

    public async Task<List<string>> GetAllConnections()
    {
        return await _userRepository.GetAllConnections();
    }

    public async Task<List<string>> GetOtherConnections(int userId)
    {
        return await _userRepository.GetOtherConnections(userId);
    }










    //public async Task<TicketDetailResponseDTO> CreateTicket(CreateTicketRequestDTO requestDTO,
    //    int userId,
    //    CancellationToken cancellationToken = default)
    //{
    //    UserModel userModel = await _userRepository.GetUserAsync(userId) ?? throw new Exception($"user with id {userId} does not exist");
    //    UnitModel unitModel = await _unitRepository.GetByIdAsync(requestDTO.UnitId) ?? throw new Exception($"unit with id {requestDTO.UnitId} does not exist");

    //    if (!_repository.Residents.Where(r => r.User.Id == userId && r.Unit.Id == requestDTO.UnitId).Any() &&
    //        !_repository.Owners.Where(r => r.User.Id == userId && r.Unit.Id == requestDTO.UnitId).Any())
    //        throw new Exception("this user is not related to this unit");

    //    TicketModel ticketModel = new()
    //    {
    //        Title = requestDTO.Title,
    //        TicketStatus = TicketStatus.PENDING,
    //        Unit = unitModel,
    //        User = userModel,
    //        CreatedAt = DateTime.Now,
    //        ModifyDate = DateTime.Now,
    //        Description = requestDTO.Description,
    //        Urgent = requestDTO.Urgent,
    //        Attachment = requestDTO.Url == null ? null : new Attachment(requestDTO.Url) // should be removed later,
    //    };

    //    await _ticketingRepository.SaveTicketAsync(ticketModel, cancellationToken);

    //    var result = new TicketDetailResponseDTO()
    //    {
    //        Id = ticketModel.Id,
    //        CreatedAt = ticketModel.CreatedAt.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
    //        ModifyDate = ticketModel.ModifyDate?.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
    //        UserId = ticketModel.User.Id,
    //        CreatedBy = ticketModel.User.FirstName + " " + ticketModel.User.LastName,
    //        Title = ticketModel.Title,
    //        Description = ticketModel.Description,

    //        TicketStatus = ticketModel.TicketStatus,
    //        Attachment = ticketModel.Attachment,
    //        UnitId = ticketModel.Unit.Id,
    //        UnitName = ticketModel.Unit.Name,
    //        VisitTime = ticketModel.VisitTime?.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
    //        Urgent = ticketModel.Urgent,
    //        Messages = new(),

    //    };
    //    result.TechnicianName = ticketModel.Technician is null ? null : ticketModel.Technician.FirstName + " " + ticketModel.Technician.LastName;
    //    var messageModels = await _ticketingRepository.GetTicketMessages(ticketModel.Id, cancellationToken);
    //    if (messageModels.IsNullOrEmpty())
    //    {
    //        result.LastResponseDate = null;
    //    }
    //    else
    //    {
    //        result.LastResponseDate = messageModels.First().CreatedAt.ToString("yy/MM/dd", new CultureInfo("fa-IR"));
    //    }
    //    result.Messages = messageModels.Select(messageModel => new MessageResponseDTO()
    //    {
    //        Id = messageModel.Id,
    //        CreatedAt = messageModel.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
    //        Description = messageModel.Text,
    //        FromOperator = messageModel.Author.Id != ticketModel.User.Id,
    //        Attachment = messageModel.Attachment,
    //        UserName = messageModel.Author.FirstName + " " + messageModel.Author.LastName,

    //    }).ToList();

    //    //Task.Run();

    //    _messagingService.SendTicketAdminSMS(new TicketAdminSMSDTO
    //    {
    //        Title = result.Title,
    //        CreateDate = result.CreatedAt,
    //        CreatedBy = result.CreatedBy,
    //        AttchmentUrl = result.Attachment is null ? string.Empty : "https://app.enjoylife.ir/filestorage/" + result.Attachment.Url,
    //        Description = result.Description,
    //        UnitName = result.UnitName,
    //    }, cancellationToken);

    //    _messagingService.SendTicketUserSMS(new List<string>() { userModel.PhoneNumber }, cancellationToken);

    //    return result;
    //}
    //public async Task<TicketModel> ActivateTicket(ActivateTicketRequestDTO requestDTO, int userId, CancellationToken cancellationToken = default)
    //{
    //    //UserModel userModel = await _userRepository.GetUserAsync(userId) ?? throw new Exception($"user with id {userId} does not exist");
    //    //if user is admin
    //    var ticketModel = await _ticketingRepository.GetTicketByIdAsync(requestDTO.Id, 0, cancellationToken) ?? throw new Exception($"ticket with id {requestDTO.Id} was not found");
    //    UserModel userModel = await _userRepository.GetUserAsync(userId) ?? throw new Exception($"user with id {userId} does not exist");
    //    if (ticketModel.TicketStatus != TicketStatus.PENDING && ticketModel.TrackingCode >= 0)
    //    {
    //        throw new Exception($"ticket with id {requestDTO.Id} is active");
    //    }
    //    ticketModel.TrackingCode = requestDTO.TrackingCode;
    //    if (!string.IsNullOrEmpty(requestDTO.TicketNumber) && !string.IsNullOrWhiteSpace(requestDTO.TicketNumber))
    //    {
    //        ticketModel.TicketNumber = requestDTO.TicketNumber.Trim();
    //    }
    //    ticketModel.TicketStatus = TicketStatus.ONGOING;
    //    ticketModel.ModifyDate = DateTime.Now;
    //    var result = await _ticketingRepository.UpdateTicketAsync(ticketModel, true, null, cancellationToken);



    //    return result;

    //}
    //public async Task<TicketModel> UpdateTrackCode(UpdateTrackingCodeDTO requestDTO, int userId, CancellationToken cancellationToken = default)
    //{

    //    var ticketModel = await _ticketingRepository.GetTicketByIdAsync(requestDTO.Id, 0, cancellationToken) ?? throw new Exception($"ticket with id {requestDTO.Id} was not found");
    //    UserModel userModel = await _userRepository.GetUserAsync(userId) ?? throw new Exception($"user with id {userId} does not exist");

    //    ticketModel.TrackingCode = requestDTO.TrackingCode;
    //    ticketModel.ModifyDate = DateTime.Now;
    //    var result = await _ticketingRepository.UpdateTicketAsync(ticketModel, true, null, cancellationToken);

    //    _messagingService.SendTicektUpdateSMS(new List<string>() { userModel.PhoneNumber }, new Contract.API.DTO.Messaging.TicketActivationSMSDTO
    //    {
    //        TicketId = ticketModel.Id.ToString(),
    //        Title = ticketModel.Title,
    //        TrackingCode = ticketModel.TrackingCode.ToString(),
    //    }, cancellationToken);

    //    return result;
    //}

    //public async Task<TicketModel> UpdateTicketNumber(UpdateTicketNumberDTO requestDTO, int userId, CancellationToken cancellationToken = default)
    //{
    //    if (string.IsNullOrEmpty(requestDTO.TicketNumber) || string.IsNullOrWhiteSpace(requestDTO.TicketNumber))
    //    {
    //        throw new Exception("TicketNumber is required");
    //    }
    //    if (requestDTO.TicketNumber.Length < 1 || requestDTO.TicketNumber.Length > 80)
    //    {
    //        throw new Exception("TicketNumber must be between 1 to 80 characters");
    //    }
    //    var ticketModel = await _ticketingRepository.GetTicketByIdAsync(requestDTO.Id, 0, cancellationToken) ?? throw new Exception($"ticket with id {requestDTO.Id} was not found");
    //    UserModel userModel = await _userRepository.GetUserAsync(ticketModel.User.Id) ?? throw new Exception($"user with id {userId} does not exist");
    //    ticketModel.TicketNumber = requestDTO.TicketNumber;
    //    ticketModel.ModifyDate = DateTime.Now;
    //    var result = await _ticketingRepository.UpdateTicketAsync(ticketModel, true, null, cancellationToken);

    //    _messagingService.SendTicketUpdateNumberSMS(new List<string>() { userModel.PhoneNumber }, new Contract.API.DTO.Messaging.TicketUpdateNumberSMSDTO
    //    {
    //        TicketId = ticketModel.Id.ToString(),
    //        Title = ticketModel.Title,
    //        TicketNumber = ticketModel.TicketNumber,
    //    }, cancellationToken);

    //    return result;

    //}

    //public async Task<MessageResponseDTO> CreateMessage(CreateMessageRequestDTO requestDTO,
    //    int userId,
    //    bool isAdmin,
    //    CancellationToken cancellationToken = default)
    //{

    //    UserModel author = await _userRepository.GetUserAsync(userId) ?? throw new Exception($"user with id {userId} does not exist");

    //    TicketModel ticketModel = await _ticketingRepository.GetTicketByIdAsync(requestDTO.TicketId) ?? throw new Exception($"ticket with id {requestDTO.TicketId} was not found");

    //    if (ticketModel.TicketStatus == TicketStatus.ADMINCLOSED || ticketModel.TicketStatus == TicketStatus.USERCLOSED ||
    //        ticketModel.TicketStatus == TicketStatus.SUCCESS)
    //        throw new Exception("this ticket is closed");

    //    if (!isAdmin)
    //    {
    //        if (ticketModel.User.Id != userId) throw new Exception("You cant message in this ticket");

    //        //var last = await _repository.TicketMessages.Where(m => m.Ticket.Id == requestDTO.TicketId).Include(m=>m.Author).OrderByDescending(m => m.CreatedAt).FirstOrDefaultAsync(cancellationToken);
    //        //if(last is null)
    //        //{
    //        //    //throw new WaitForOperatorException("wait for operator to answer last request");                
    //        //}
    //        //else
    //        //{
    //        //    //if (last.Author.Id == userId) throw new WaitForOperatorException("wait for operator to answer last request");
    //        //}



    //        //if (ticketModel.User.Id == userId)
    //        //{
    //        //    ticketModel.TicketStatus = TicketStatus.PENDING;
    //        //}
    //        //else ticketModel.TicketStatus = TicketStatus.ONGOING;


    //    }


    //    TicketMessageModel ticketMessageModel = new()
    //    {
    //        Author = author,
    //        CreatedAt = DateTime.Now,
    //        Text = requestDTO.Text,
    //        Ticket = ticketModel,

    //    };

    //    //this should be removed later
    //    if (!string.IsNullOrEmpty(requestDTO.Url))
    //    {
    //        ticketMessageModel.Attachment = new Attachment(requestDTO.Url);
    //    }



    //    //save attachments
    //    //if(attachments is not null)
    //    //{

    //    //foreach (var attachment in attachments)
    //    //{
    //    //    using (Stream stream = attachment.OpenReadStream())
    //    //    {
    //    //        string url = await _fileStorageService.UploadTicketingAttachment(stream, attachment.FileName, attachment.ContentType, ticketMessageModel.Ticket.Id, ticketMessageModel.Id, cancellationToken);
    //    //        ticketMessageModel.Attachments.Add(new AttachMent(url));
    //    //    }
    //    //}

    //    //await _ticketingRepository.UpdateMessageAsync(ticketMessageModel, cancellationToken);

    //    //}
    //    ticketModel.ModifyDate = DateTime.Now;
    //    await _ticketingRepository.UpdateTicketAsync(ticketModel, isAdmin, null, cancellationToken);

    //    await _ticketingRepository.SaveTicketMessageAsync(ticketMessageModel, cancellationToken);

    //    var result = new MessageResponseDTO()
    //    {
    //        Id = ticketMessageModel.Id,
    //        Description = ticketMessageModel.Text,
    //        UserName = isAdmin ? "اپراتور" : author.FirstName + " " + author.LastName,
    //        FromOperator = isAdmin,
    //        CreatedAt = ticketMessageModel.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
    //        Attachment = ticketMessageModel.Attachment,
    //    };

    //    if (!isAdmin)
    //        _messagingService.SendTicketMessageAdminSMS(new TicketMessageAdminDTO
    //        {
    //            Title = ticketModel.Title,
    //            Text = ticketMessageModel.Text,
    //            CreatedBy = result.UserName,
    //            CreatedDate = ticketMessageModel.CreatedAt.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
    //            TrackingCode = ticketModel.TrackingCode > 0 ? ticketModel.TrackingCode.ToString() : "ثبت نشده",
    //            UnitName = ticketModel.Unit.Name,
    //            AttachmentUrl = ticketMessageModel.Attachment is null ? string.Empty : "https://app.enjoylife.ir/filestorage/" + ticketMessageModel.Attachment.Url,
    //        }, cancellationToken);
    //    else
    //        _messagingService.SendTicketMessageSMS(ticketModel.User.PhoneNumber, new TicketMessageSMSDTO()
    //        {
    //            GenderType = ticketModel.User.Gender switch
    //            {
    //                null => throw new Exception("gender type is null"),
    //                GenderType.ALL => "آقای/خانم",
    //                GenderType.MALE => "آقای",
    //                GenderType.FAMALE => "خانم",
    //                _ => throw new NotImplementedException(),
    //            },
    //            TicketId = ticketModel.Id.ToString(),
    //            TicketTitle = ticketModel.Title,
    //            TrackingCode = ticketModel.TrackingCode.ToString(),
    //            UserFullName = ticketModel.User.FirstName + " " + ticketModel.User.LastName,
    //        }, cancellationToken);

    //    return result;

    //}

    //public async Task<List<TicketPreviewResponseDTO>> GetTicketsByUser(int userId,
    //                                                                      GetTicketListQueryParams queryParams,
    //                                                                      CancellationToken cancellationToken = default)
    //{
    //    //if (!await _repository.Users.Where(u => u.Id == userId).AnyAsync(cancellationToken)) throw new Exception($"user with id {userId} was not found");
    //    var user = await _repository.Users.Where(u => u.Id == userId).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"user with id {userId} was not found");
    //    var ttt = (await _ticketingRepository.GetTicketsByUser(userId, queryParams, cancellationToken));

    //    var seens = await _repository.TicketSeens.Where(x => x.UserId == userId).ToListAsync(cancellationToken: cancellationToken);
    //    return ttt
    //        .Select(ticketModel => new
    //        {
    //            TicketModel = ticketModel,
    //            LastResponseDate = _repository.TicketMessages
    //                .Where(m => m.Ticket.Id == ticketModel.Id)
    //                .OrderBy(m => m.CreatedAt)
    //                .Select(m => m.CreatedAt)
    //                .LastOrDefault()
    //        }).Select(x => x.TicketModel.ConvertTicketModelToTicketResponseDTOwithDetail(
    //            x.LastResponseDate,
    //            seens.Any(y => y.TicketId == x.TicketModel.Id),
    //            seens.FirstOrDefault(y => y.TicketId == x.TicketModel.Id)?.SeenDate))
    //        .ToList();
    //}
    //public async Task<List<TicketPreviewResponseDTO>> GetAllTickets(int userID, GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default)
    //{
    //    var query = _repository.Tickets;


    //    if (queryParams.Open is not null)
    //        if ((bool)queryParams.Open) query = query.Where(t => t.TicketStatus != TicketStatus.ADMINCLOSED && t.TicketStatus != TicketStatus.USERCLOSED);
    //        else query = query.Where(t => t.TicketStatus == TicketStatus.ADMINCLOSED || t.TicketStatus == TicketStatus.USERCLOSED);

    //    if (queryParams.Urgent is not null) query = query.Where(t => t.Urgent == queryParams.Urgent);

    //    if (queryParams.Pending is not null)
    //        if ((bool)queryParams.Pending) query = query.Where(t => t.TicketStatus == TicketStatus.PENDING);
    //        else query = query.Where(t => t.TicketStatus == TicketStatus.ONGOING);

    //    var data = await query.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken: cancellationToken);
    //    var seens = await _repository.TicketSeens.Where(x => x.UserId == userID).ToListAsync(cancellationToken: cancellationToken);

    //    return data
    //        .Select(ticketModel => new
    //        {
    //            TicketModel = ticketModel,
    //            LastResponseDate = _repository.TicketMessages
    //                .Where(m => m.Ticket.Id == ticketModel.Id)
    //                .OrderBy(m => m.CreatedAt)
    //                .Select(m => m.CreatedAt)
    //                .LastOrDefault()
    //        })
    //        .Select(x => x.TicketModel.ConvertTicketModelToTicketResponseDTOwithDetail(
    //            x.LastResponseDate,
    //            seens.Any(y => y.TicketId == x.TicketModel.Id),
    //            seens.FirstOrDefault(y => y.TicketId == x.TicketModel.Id)?.SeenDate))
    //        .ToList();

    //}

    //public async Task<(List<TicketModelDisplayDTO> Tickets, int TotalCount)> GetTickets(int userId, GetAdminTicketSearchDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    return await _ticketingRepository.GetAllTicketAsync(userId, requestDTO, cancellationToken);
    //}

    //public async Task<GetMessageResponseDTO> UploadAttachment(long messageId, int userId, string fileName, string contentType, Stream stream, CancellationToken cancellationToken = default)
    //{
    //    TicketMessageModel ticketMessageModel = await _ticketingRepository.GetMessageByIdAsync(messageId) ?? throw new Exception($"message with id {messageId} was not found");

    //    string url = await _fileStorageService.UploadTicketingAttachment(stream, fileName, contentType, ticketMessageModel.Ticket.Id, messageId, cancellationToken);
    //    ticketMessageModel.Attachment = new Attachment(url);
    //    await _ticketingRepository.UpdateMessageAsync(ticketMessageModel);
    //    return new GetMessageResponseDTO()
    //    {
    //        Id = ticketMessageModel.Id,
    //        Text = ticketMessageModel.Text,
    //        AuthorUserId = ticketMessageModel.Author.Id,
    //        TicketId = ticketMessageModel.Ticket.Id,
    //        CreatedAt = ticketMessageModel.CreatedAt.ToString("t", new CultureInfo("fa-IR")),
    //        Attachment = ticketMessageModel.Attachment

    //    };
    //}
    //public async Task<TicketDetailResponseDTO?> GetTicketById(int userId, int ticketId, CancellationToken cancellationToken = default, string sortOrder = "desc")
    //{
    //    var ticketModel = await _ticketingRepository.GetTicketByIdAsync(ticketId, userId, cancellationToken) ?? throw new Exception($"ticket with id {ticketId} was not found");
    //    if (ticketModel != null)
    //    {
    //        var seens = await _repository.TicketSeens.FirstOrDefaultAsync(x => x.TicketId == ticketModel.Id && x.UserId == userId, cancellationToken: cancellationToken);
    //        var result = ticketModel.ConvertTicketModelToTicketResponseDTO(seens != null, seens?.SeenDate);
    //        result.TechnicianName = ticketModel.Technician is null ? null : ticketModel.Technician.FirstName + " " + ticketModel.Technician.LastName;
    //        var messageModels = await _ticketingRepository.GetTicketMessages(ticketModel.Id, cancellationToken, sortOrder);
    //        if (messageModels.IsNullOrEmpty())
    //        {
    //            result.LastResponseDate = null;
    //        }
    //        else
    //        {
    //            result.LastResponseDate = messageModels.First().CreatedAt.ToString("yy/MM/dd", new CultureInfo("fa-IR"));
    //        }
    //        result.Messages = messageModels.Select(messageModel => new MessageResponseDTO()
    //        {
    //            Id = messageModel.Id,
    //            CreatedAt = messageModel.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
    //            Description = messageModel.Text,
    //            FromOperator = messageModel.Author.Id != ticketModel.User.Id,
    //            Attachment = messageModel.Attachment,
    //            UserName = messageModel.Author.Id != ticketModel.User.Id ? "اپراتور" : messageModel.Author.FirstName + " " + messageModel.Author.LastName,
    //        }).ToList();


    //        return result;
    //    }
    //    return null;

    //}

    //public async Task<List<TicketVisitTimeModel>?> GetTicketsVisitTimes(GetTicketVisitSearchDTO requestDTO, CancellationToken cancellationToken = default, string sortOrder = "desc")
    //{
    //    var result = await _ticketingRepository.GetTicketsVisitTimesAsync(requestDTO, cancellationToken);
    //    return result;
    //}

    //public async Task<int> DeleteVisitTimeById(int visitId, CancellationToken cancellationToken = default)
    //{
    //    return await _ticketingRepository.DeleteVisitTimeByIdAsync(visitId, cancellationToken);
    //}

    //public async Task<TicketDetailResponseDTO?> GetTicketByIdAdmin(int userId, int ticketId, CancellationToken cancellationToken = default, string sortOrder = "desc")
    //{
    //    var ticketModel = await _ticketingRepository.GetTicketByIdAsync(ticketId, userId, cancellationToken) ?? throw new Exception($"ticket with id {ticketId} was not found");
    //    if (ticketModel != null)
    //    {
    //        var seens = await _repository.TicketSeens.FirstOrDefaultAsync(x => x.TicketId == ticketModel.Id && x.UserId == userId, cancellationToken: cancellationToken);
    //        var result = ticketModel.ConvertTicketModelToTicketResponseDTO(seens != null, seens?.SeenDate);
    //        result.TechnicianName = ticketModel.Technician is null ? null : ticketModel.Technician.FirstName + " " + ticketModel.Technician.LastName;
    //        var messageModels = await _ticketingRepository.GetTicketMessages(ticketModel.Id, cancellationToken, sortOrder);
    //        if (messageModels.IsNullOrEmpty())
    //        {
    //            result.LastResponseDate = null;
    //        }
    //        else
    //        {
    //            result.LastResponseDate = messageModels.First().CreatedAt.ToString("yy/MM/dd", new CultureInfo("fa-IR"));
    //        }
    //        result.Messages = messageModels.Select(messageModel => new MessageResponseDTO()
    //        {
    //            Id = messageModel.Id,
    //            CreatedAt = messageModel.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
    //            Description = messageModel.Text,
    //            FromOperator = messageModel.Author.Id != ticketModel.User.Id,
    //            Attachment = messageModel.Attachment,
    //            UserName = messageModel.Author.Id != ticketModel.User.Id ? "اپراتور" : messageModel.Author.FirstName + " " + messageModel.Author.LastName,
    //        }).ToList();


    //        return result;
    //    }
    //    return null;

    //}

    //public async Task<TicketDetailResponseDTO> UpdateTicket(int ticketId, bool isAdmin, UpdateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    TicketModel ticketModel = await _ticketingRepository.GetTicketByIdAsync(ticketId, 0, cancellationToken) ?? throw new Exception($"ticket with id {ticketId} was not found");
    //    ticketModel.TicketStatus = requestDTO.TicketStatus;
    //    if (requestDTO.TechnicianId is not null)
    //    {
    //        ticketModel.Technician = await _repository.Users.Where(x => x.Id == requestDTO.TechnicianId).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("user not found");
    //    }
    //    if (requestDTO.VisitTime is not null)
    //    {
    //        ticketModel.VisitTime = requestDTO.VisitTime;
    //    }
    //    await _ticketingRepository.UpdateTicketAsync(ticketModel, isAdmin, requestDTO.CloseMessage, cancellationToken);

    //    var result = new TicketDetailResponseDTO()
    //    {
    //        Id = ticketModel.Id,
    //        CreatedAt = ticketModel.CreatedAt.ToString("HH:mm-yy/MM/dd", new CultureInfo("fa-IR")),
    //        ModifyDate = ticketModel.ModifyDate?.ToString("HH:mm-yy/MM/dd", new CultureInfo("fa-IR")),
    //        UserId = ticketModel.User != null ? ticketModel.User.Id : 0,
    //        CreatedBy = ticketModel.User == null ? "" : ticketModel.User.FirstName + " " + ticketModel.User.LastName,
    //        Title = ticketModel.Title,
    //        Description = ticketModel.Description,
    //        TicketStatus = ticketModel.TicketStatus,
    //        Attachment = ticketModel.Attachment,
    //        UnitId = ticketModel.Unit == null ? 0 : ticketModel.Unit.Id,
    //        UnitName = ticketModel.Unit == null ? "" : ticketModel.Unit.Name,
    //        VisitTime = ticketModel.VisitTime?.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
    //        Urgent = ticketModel.Urgent,
    //        Messages = new(),

    //    };
    //    result.TechnicianName = ticketModel.Technician is null ? null : ticketModel.Technician.FirstName + " " + ticketModel.Technician.LastName;
    //    var messageModels = await _ticketingRepository.GetTicketMessages(ticketModel.Id, cancellationToken);
    //    if (messageModels.IsNullOrEmpty())
    //    {
    //        result.LastResponseDate = null;
    //    }
    //    else
    //    {
    //        result.LastResponseDate = messageModels.First().CreatedAt.ToString("HH:mm-yy/MM/dd", new CultureInfo("fa-IR"));
    //    }
    //    result.Messages = messageModels.Select(messageModel => new MessageResponseDTO()
    //    {
    //        Id = messageModel.Id,
    //        CreatedAt = messageModel.CreatedAt.ToString("HH:mm", new CultureInfo("fa-IR")),
    //        Description = messageModel.Text,
    //        FromOperator = messageModel.Author.Id != ticketModel.User.Id,
    //        Attachment = messageModel.Attachment,
    //        UserName = messageModel.Author.Id != ticketModel.User.Id ? "اپراتور" : messageModel.Author.FirstName + " " + messageModel.Author.LastName,

    //    }).ToList();

    //    if (requestDTO.TicketStatus == TicketStatus.ADMINCLOSED || requestDTO.TicketStatus == TicketStatus.USERCLOSED)
    //    {
    //        _messagingService.SendTicketClosingToAdminsSMS(new TicketUpdateToCloseAdminSMSDTO
    //        {
    //            CancelationType = isAdmin ? "لغو توسط ادمین" : "لغو توسط کاربر",
    //            ClosedDate = DateTime.Now.GetPersianShortDate().En2Fa(),
    //            CloseMessage = !string.IsNullOrEmpty(requestDTO.CloseMessage) && !string.IsNullOrWhiteSpace(requestDTO.CloseMessage) ? requestDTO.CloseMessage.Trim() : null,
    //            CreatedDate = ticketModel.CreatedAt.GetPersianFullDate().En2Fa(),
    //            TicketNumber = ticketModel.TicketNumber,
    //            Title = ticketModel.Title,
    //            TrackingCode = ticketModel.TrackingCode < 0 ? "ثبت نشده" : $"{ticketModel.TrackingCode}",
    //            UnitName = ticketModel.Unit == null ? "" : ticketModel.Unit.Name
    //        }, cancellationToken);

    //        _messagingService.SendTicketClosingToCustomerSMS(ticketModel.User.PhoneNumber, new TicketUpdateToCloseCustomerSMSDTO
    //        {
    //            CancelationType = isAdmin ? "لغو توسط ادمین" : "لغو توسط کاربر",
    //            ClosedDate = DateTime.Now.GetPersianShortDate().En2Fa(),
    //            CloseMessage = !string.IsNullOrEmpty(requestDTO.CloseMessage) && !string.IsNullOrWhiteSpace(requestDTO.CloseMessage) ? requestDTO.CloseMessage.Trim() : null,
    //            CreatedDate = ticketModel.CreatedAt.GetPersianFullDate().En2Fa(),
    //            TicketNumber = ticketModel.TicketNumber,
    //            Title = ticketModel.Title,
    //            TrackingCode = ticketModel.TrackingCode < 0 ? "ثبت نشده" : $"{ticketModel.TrackingCode}",
    //            UnitName = ticketModel.Unit == null ? "" : ticketModel.Unit.Name,
    //            GenderType = ticketModel.User == null ? "" : ticketModel.User.Gender switch
    //            {
    //                null => "آقای/خانم",
    //                GenderType.ALL => "آقای/خانم",
    //                GenderType.MALE => "آقای",
    //                GenderType.FAMALE => "خانم",
    //                _ => "آقای/خانم"
    //            },
    //            UserFullName = ticketModel.User == null ? "" : $"{ticketModel.User.FirstName} {ticketModel.User.LastName}"
    //        }, cancellationToken);

    //    }

    //    return result;
    //}
    //public async Task<int> CreateTicketVisitTime(CreateTicketVisitTimeRequestDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    TicketModel ticketModel = await _ticketingRepository.GetTicketByIdAsync(requestDTO.TicketId, 0, cancellationToken) ?? throw new Exception($"ticket with id {requestDTO.TicketId} was not found");

    //    TicketVisitTimeModel model = new TicketVisitTimeModel();
    //    model.Ticket = ticketModel;
    //    model.Title = requestDTO.Title != null ? requestDTO.Title : "";
    //    model.VisitTime = requestDTO.VisitTime;
    //    model.Order = requestDTO.Order;
    //    return await _ticketingRepository.SaveTicketVisitTimeAsync(model, cancellationToken);
    //}

    //public async Task<string> SeenTicket(int userId, SeenTicketDTO ticketIdDTO, CancellationToken cancellationToken = default)
    //{
    //    if (ticketIdDTO == null)
    //    {
    //        return "ticket id is not defined";
    //    }
    //    return await _ticketingRepository.SeenTicket(userId, ticketIdDTO.TicketId, cancellationToken);
    //}

    //public async Task<List<TicketSeen>> GetSeenedByUserId(int userId, CancellationToken cancellationToken = default)
    //{
    //    return await _ticketingRepository.GetSeenedByUserId(userId, cancellationToken);
    //}
}
