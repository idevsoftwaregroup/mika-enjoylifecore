using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Framework;
using OrganizationChart.API.Infrastructure.FileStorage;
using OrganizationChart.API.Infrastructure.Persistence;
using OrganizationChart.API.Models;
using System.Net;

namespace OrganizationChart.API.Services;

public class GetSystemDetailsAsync : ISystemDetails
{
    private readonly OrganizationChartDataContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetSystemDetailsAsync> _logger;

    public GetSystemDetailsAsync(OrganizationChartDataContext context, IFileStorageService fileStorageService, ILogger<GetSystemDetailsAsync> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }



    //// create single detail of the system :
    //public async Task<OperationResult<object>> CreateSystemsDetail(CreateSystemDetailsDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    OperationResult<object> Op = new("CreateSystemsDetail");
    //    if (requestDTO == null)
    //    {
    //        return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
    //    }
    //    if (string.IsNullOrEmpty(requestDTO.SystemId) || string.IsNullOrWhiteSpace(requestDTO.SystemId))
    //    {
    //        return Op.Failed("ارسال شناسه سیستم اجباری است");
    //    }
    //    if (requestDTO.SystemId.Contains(' '))
    //    {
    //        return Op.Failed("شناسه سیستم بدرستی ارسال نشده است");
    //    }
    //    if (string.IsNullOrEmpty(requestDTO.SystemName) || string.IsNullOrWhiteSpace(requestDTO.SystemName))
    //    {
    //        return Op.Failed("ارسال نام سیستم اجباری است");
    //    }
    //    try
    //    {
    //        if (await _context.Systems.AnyAsync(x => x.SystemId.ToLower() == requestDTO.SystemId.ToLower(), cancellationToken: cancellationToken))
    //        {
    //            return Op.Failed("ثبت سیستم با شناسه تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
    //        }
    //        await _context.Systems.AddAsync(new Systems
    //        {
    //            Description = string.IsNullOrEmpty(requestDTO.Description) || string.IsNullOrWhiteSpace(requestDTO.Description) ? string.Empty : requestDTO.Description.Trim(),
    //            SystemId = requestDTO.SystemId,
    //            Location = string.IsNullOrEmpty(requestDTO.Location) || string.IsNullOrWhiteSpace(requestDTO.Location) ? string.Empty : requestDTO.Location.Trim(),
    //            SystemName = requestDTO.SystemName.Trim(),
    //            SystemPicUrl = string.IsNullOrEmpty(requestDTO.SystemPicUrl) || string.IsNullOrWhiteSpace(requestDTO.SystemPicUrl) ? string.Empty : requestDTO.SystemPicUrl.Trim()
    //        }, cancellationToken);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return Op.Succeed("ثبت سیستم جدید با موفقیت انجام شد");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Op.Failed("ثبت سیستم جدید با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
    //    }
    //}

    //public async Task<OperationResult<object>> DeleteSystem(SystemIdDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    OperationResult<object> Op = new("DeleteSystem");
    //    if (requestDTO == null || string.IsNullOrEmpty(requestDTO.SystemId) || string.IsNullOrWhiteSpace(requestDTO.SystemId))
    //    {
    //        return Op.Failed("اطلاعاتی برای حذف سیستم ارسال نشده است");
    //    }
    //    if (requestDTO.SystemId.Contains(' '))
    //    {
    //        return Op.Failed("شناسه سیستم بدرستی ارسال نشده است");
    //    }
    //    try
    //    {
    //        var system = await _context.Systems.FirstOrDefaultAsync(x => x.SystemId.ToLower() == requestDTO.SystemId.ToLower(), cancellationToken: cancellationToken);
    //        if (system == null)
    //        {
    //            return Op.Failed("شناسه سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
    //        }
    //        _context.Systems.Remove(system);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return Op.Succeed("حذف سیستم با موفقیت انجام شد");
    //    }
    //    catch (Exception ex)
    //    {
    //        return Op.Failed("حذف سیستم با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
    //    }
    //}

    //// get all details of the systems :
    //public async Task<List<Systems>> GetAll(CancellationToken cancellationToken = default)
    //{
    //    var listSystems = _context.Systems.Include(d => d.SystemId).ToList();
    //    return listSystems;
    //}

    //// get single details of the system :
    //public async Task<OperationResult<GetSystemDTO>> GetSystem(string systemId, CancellationToken cancellationToken = default)
    //{
    //    OperationResult<GetSystemDTO> Op = new("GetSystem");
    //    if (string.IsNullOrEmpty(systemId) || string.IsNullOrWhiteSpace(systemId))
    //    {
    //        return Op.Failed("شناسه دستگاه بدرستی ارسال نشده است");
    //    }
    //    try
    //    {
    //        var system = await _context.Systems.FirstOrDefaultAsync(x => x.SystemId.ToLower() == systemId.ToLower(), cancellationToken: cancellationToken);
    //        if (system == null)
    //        {
    //            return Op.Failed("شناسه دستگاه بدرستی ارسال نشده است", HttpStatusCode.NotFound);
    //        }
    //        return Op.Succeed("دریافت اطلاعات سیستم با موفقیت انجام شد", new GetSystemDTO
    //        {
    //            Description = !string.IsNullOrEmpty(system.Description) && !string.IsNullOrWhiteSpace(system.Description) ? system.Description : null,
    //            Id = system.Id,
    //            Location = !string.IsNullOrEmpty(system.Location) && !string.IsNullOrWhiteSpace(system.Location) ? system.Location : null,
    //            SystemId = system.SystemId,
    //            SystemName = system.SystemName,
    //            SystemPicUrl = !string.IsNullOrEmpty(system.SystemPicUrl) && !string.IsNullOrWhiteSpace(system.SystemPicUrl) ? system.SystemPicUrl : null
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return Op.Failed("دریافت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
    //    }
    //}

    //public async Task<OperationResult<GetSystemDTO>> UpdateSystem(UpdateSystemDetailsDTO requestDTO, CancellationToken cancellationToken = default)
    //{
    //    OperationResult<GetSystemDTO> Op = new("UpdateSystem");
    //    if (requestDTO == null)
    //    {
    //        return Op.Failed("اطلاعاتی جهت بروزرسانی سیستم ارسال نشده است");
    //    }
    //    if (string.IsNullOrEmpty(requestDTO.SystemId) || string.IsNullOrWhiteSpace(requestDTO.SystemId))
    //    {
    //        return Op.Failed("ارسال شناسه سیستم اجباری است");
    //    }
    //    if (requestDTO.SystemId.Contains(' '))
    //    {
    //        return Op.Failed("شناسه سیستم بدرستی ارسال نشده است");
    //    }
    //    if (
    //        (string.IsNullOrEmpty(requestDTO.SystemName) || string.IsNullOrWhiteSpace(requestDTO.SystemName)) &&
    //        (string.IsNullOrEmpty(requestDTO.Description) || string.IsNullOrWhiteSpace(requestDTO.Description)) &&
    //        (string.IsNullOrEmpty(requestDTO.SystemPicUrl) || string.IsNullOrWhiteSpace(requestDTO.SystemPicUrl)) &&
    //        (string.IsNullOrEmpty(requestDTO.Location) || string.IsNullOrWhiteSpace(requestDTO.Location))
    //        )
    //    {
    //        return Op.Failed("اطلاعاتی جهت بروزرسانی سیستم ارسال نشده است");
    //    }
    //    try
    //    {
    //        var system = await _context.Systems.FirstOrDefaultAsync(x => x.SystemId.ToLower() == requestDTO.SystemId.ToLower(), cancellationToken: cancellationToken);
    //        if (system == null)
    //        {
    //            return Op.Failed("شناسه سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
    //        }
    //        if (!string.IsNullOrEmpty(requestDTO.SystemName) && !string.IsNullOrWhiteSpace(requestDTO.SystemName))
    //        {
    //            system.SystemName = requestDTO.SystemName.Trim();
    //        }
    //        if (!string.IsNullOrEmpty(requestDTO.Description) && !string.IsNullOrWhiteSpace(requestDTO.Description))
    //        {
    //            system.Description = requestDTO.Description.Trim();
    //        }
    //        if (!string.IsNullOrEmpty(requestDTO.SystemPicUrl) && !string.IsNullOrWhiteSpace(requestDTO.SystemPicUrl))
    //        {
    //            system.SystemPicUrl = requestDTO.SystemPicUrl;
    //        }
    //        if (!string.IsNullOrEmpty(requestDTO.Location) && !string.IsNullOrWhiteSpace(requestDTO.Location))
    //        {
    //            system.Location = requestDTO.Location.Trim();
    //        }
    //        _context.Entry<Systems>(system).State = EntityState.Modified;
    //        await _context.SaveChangesAsync(cancellationToken);

    //        return Op.Succeed("بروزرسانی اطلاعات سیستم با موفقیت انجام شد", new GetSystemDTO
    //        {
    //            Description = system.Description,
    //            Id = system.Id,
    //            Location = system.Location,
    //            SystemId = system.SystemId,
    //            SystemName = system.SystemName,
    //            SystemPicUrl = system.SystemPicUrl,
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return Op.Failed("بروزرسانی اطلاعات سیستم با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
    //    }
    //}

    // Get All Systems details:
    public async Task<OperationResult<List<GetSystemDTO>>> GetAllSystems(CancellationToken cancellationToken = default)
    {
        OperationResult<List<GetSystemDTO>> Op = new("GetAllSystems");
        try
        {
            var systems = await _context.Systems.ToListAsync(cancellationToken: cancellationToken);
            var systemDTOs = systems.Select(system => new GetSystemDTO
            {
                ComputerName = system.ComputerName,
                CPU = system.CPU,
                Location = system.Location,
                Description = system.Description,
                MainBoardModel = system.MainBoardModel,
                MainBoardName = system.MainBoardName,
                OS = system.OS,
                RAM = system.RAM,
                SystemId = system.Id,
                SystemPicUrl = system.SystemPicUrl,
                SystemTypeId = system.SystemTypeId,
                SystemTypeName = (from type in _context.SystemTypes
                                  where type.Id == system.SystemTypeId
                                  select type.Name).FirstOrDefault(),
                User = system.User,
                UserPicUrl = system.UserPicUrl,
                Disks = (from disk in _context.SystemHards
                         where disk.SystemId == system.Id
                         select new GetSystem_DiskDTO
                         {
                             DiskId = disk.Id,
                             DiskName = disk.DiskName,
                             DiskVolume = disk.DiskVolume,
                         }).ToList(),
                Graphics = (from graphic in _context.SystemGraphics
                            where graphic.SystemId == system.Id
                            select new GetSystem_GraphicDTO
                            {
                                GraphicCardId = graphic.Id,
                                GraphicCardName = graphic.GraphicCardName
                            }).ToList()
            }).ToList();

            return Op.Succeed("دریافت اطلاعات سیستم‌ها با موفقیت انجام شد", systemDTOs);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات سیستم‌ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<GetSystemDTO>> GetSystem(string computerName, CancellationToken cancellationToken = default)
    {
        OperationResult<GetSystemDTO> Op = new("GetSystem");
        if (string.IsNullOrEmpty(computerName) || string.IsNullOrWhiteSpace(computerName))
        {
            return Op.Failed("ارسال نام سیستم اجباری است");
        }
        try
        {
            var system = await _context.Systems.FirstOrDefaultAsync(x => x.ComputerName.ToLower() == computerName.Trim().ToLower(), cancellationToken: cancellationToken);
            if (system == null)
            {
                return Op.Failed("نام سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            return Op.Succeed("دریافت اطلاعات سیستم با موفقیت انجام شد", new GetSystemDTO
            {
                ComputerName = system.ComputerName,
                CPU = system.CPU,
                Location = system.Location,
                Description = system.Description,
                MainBoardModel = system.MainBoardModel,
                MainBoardName = system.MainBoardName,
                OS = system.OS,
                RAM = system.RAM,
                SystemId = system.Id,
                SystemPicUrl = system.SystemPicUrl,
                SystemTypeId = system.SystemTypeId,
                SystemTypeName = (from type in await _context.SystemTypes.ToListAsync(cancellationToken: cancellationToken)
                                  where type.Id == system.SystemTypeId
                                  select type.Name).FirstOrDefault(),
                User = system.User,
                UserPicUrl = system.UserPicUrl,
                Disks = (from disk in await _context.SystemHards.ToListAsync(cancellationToken: cancellationToken)
                         where disk.SystemId == system.Id
                         select new GetSystem_DiskDTO
                         {
                             DiskId = disk.Id,
                             DiskName = disk.DiskName,
                             DiskVolume = disk.DiskVolume,
                         }).ToList(),
                Graphics = (from graphic in await _context.SystemGraphics.ToListAsync(cancellationToken: cancellationToken)
                            where graphic.SystemId == system.Id
                            select new GetSystem_GraphicDTO
                            {
                                GraphicCardId = graphic.Id,
                                GraphicCardName = graphic.GraphicCardName
                            }).ToList()
            });


        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات سیستم با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<GetSystemTypeDTO>> GetSystemTypes(CancellationToken cancellationToken = default)
    {
        OperationResult<GetSystemTypeDTO> Op = new("GetSystemTypes");
        try
        {
            return Op.Succeed("دریافت نوع سیستم ها با موفقیت انجام شد", (from type in await _context.SystemTypes.ToListAsync(cancellationToken: cancellationToken)
                                                                         select new GetSystemTypeDTO
                                                                         {
                                                                             Id = type.Id,
                                                                             Name = type.Name
                                                                         }).ToList());
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت نوع سیستم ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateSystem(CreateSystemDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateSystem");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت ثبت سیستم وجود ندارد");
        }
        if (model.SystemTypeId != null && model.SystemTypeId <= 0)
        {
            return Op.Failed("شناسه نوع سیستم بدرستی ارسال نشده است");
        }
        if (!model.ComputerName.IsNotNull())
        {
            return Op.Failed("ثبت نام سیستم اجباری است");
        }
        if (model.ComputerName.Contains(' '))
        {
            return Op.Failed("امکان ثبت نام سیستم با 'فاصله' امکان پذیر نمیباشد");
        }
        if (model.ComputerName.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز نام سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.User.IsNotNull() && model.User.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز کاربر {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.UserPicUrl.IsNotNull() && model.UserPicUrl.Length > 200)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز آدرس تصویر کاربر {200.ToPersianNumber()} کاراکتر است");
        }
        if (!model.OS.IsNotNull())
        {
            return Op.Failed("ثبت سیستم عامل اجباری است");
        }
        if (model.OS.Length > 650)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز سیستم عامل {650.ToPersianNumber()} کاراکتر است");
        }
        if (!model.RAM.IsNotNull())
        {
            return Op.Failed("ثبت حافظه سیستم اجباری است");
        }
        if (!model.RAM.IsNumeric(false))
        {
            return Op.Failed("حافظه سیستم بدرستی ارسال نشده است");
        }
        if (model.RAM.Length > 40)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز حافظه سیستم {40.ToPersianNumber()} کاراکتر است");
        }
        if (!model.MainBoardName.IsNotNull())
        {
            return Op.Failed("ثبت نام Main Board سیستم اجباری است");
        }
        if (model.MainBoardName.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز نام Main Board سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (!model.MainBoardModel.IsNotNull())
        {
            return Op.Failed("ثبت مدل Main Board سیستم اجباری است");
        }
        if (model.MainBoardModel.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز مدل Main Board سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (!model.CPU.IsNotNull())
        {
            return Op.Failed("ثبت اطلاعات پردازنده سیستم اجباری است");
        }
        if (model.CPU.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز اطلاعات پردازنده سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.HardDisks == null || !model.HardDisks.Any())
        {
            return Op.Failed("ارسال اطلاعات هارد دیسک سیستم اجباری است");
        }
        if (model.HardDisks.Any(x => !x.DiskName.IsNotNull()))
        {
            return Op.Failed($"ارسال نام هارد دیسک اجباری است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskName.IsNotNull())) + 1} از لیست هارد دیسک ها");
        }
        if (model.HardDisks.Any(x => x.DiskName.Length > 250))
        {
            return Op.Failed($"حداکثر کاراکتر مجاز نام هارد دیسک {250.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.HardDisks.FindIndex(x => x.DiskName.Length > 250)) + 1} از لیست هارد دیسک ها");
        }
        if (model.HardDisks.Any(x => !x.DiskVolume.IsNotNull()))
        {
            return Op.Failed($"ارسال ظرفیت هارد دیسک اجباری است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskVolume.IsNotNull())) + 1} از لیست هارد دیسک ها");
        }
        if (model.HardDisks.Any(x => !x.DiskVolume.IsNumeric(false)))
        {
            return Op.Failed($"ظرفیت هارد دیسک بدرستی ارسال نشده است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskVolume.IsNumeric(false))) + 1} از لیست هارد دیسک ها");
        }
        if (model.HardDisks.Any(x => x.DiskVolume.Length > 50))
        {
            return Op.Failed($"حداکثر کاراکتر مجاز ظرفیت هارد دیسک {50.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.HardDisks.FindIndex(x => x.DiskVolume.Length > 50)) + 1} از لیست هارد دیسک ها");
        }
        if (model.GraphicCards == null || !model.GraphicCards.Any())
        {
            return Op.Failed("ارسال اطلاعات کارت گرافیک سیستم اجباری است");
        }
        if (model.GraphicCards.Any(x => !x.GraphicCardName.IsNotNull()))
        {
            return Op.Failed($"ارسال نام کارت گرافیک اجباری است ، خطا در ردیف {(model.GraphicCards.FindIndex(x => !x.GraphicCardName.IsNotNull())) + 1} از لیست کارت های گرافیک");
        }
        if (model.GraphicCards.Any(x => x.GraphicCardName.Length > 250))
        {
            return Op.Failed($"حداکثر کاراکتر مجاز نام کارت گرافیک {250.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.GraphicCards.FindIndex(x => x.GraphicCardName.Length > 250)) + 1} از لیست کارت های گرافیک");
        }
        if (model.Description.IsNotNull() && model.Description.Length > 1450)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز توضیحات {1450.ToPersianNumber()} کاراکتر است");
        }
        if (model.Location.IsNotNull() && model.Location.Length > 150)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز محل قرارگیری سیستم {150.ToPersianNumber()} کاراکتر است");
        }
        if (model.SystemPicUrl.IsNotNull() && model.SystemPicUrl.Length > 200)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز آدرس تصویر سیستم {200.ToPersianNumber()} کاراکتر است");
        }
        try
        {
            var systemTypes = await _context.SystemTypes.ToListAsync(cancellationToken: cancellationToken);
            if (systemTypes == null || !systemTypes.Any() || !systemTypes.Any(x => x.Name.ToUpper() == "DESKTOP"))
            {
                return Op.Failed("اطلاعات انواع سیستم در سامانه ثبت نشده است", HttpStatusCode.NotFound);
            }
            if (model.SystemTypeId != null && !systemTypes.Any(x => x.Id == model.SystemTypeId))
            {
                return Op.Failed("شناسه نوع سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (await _context.Systems.AnyAsync(x => x.ComputerName.ToLower() == model.ComputerName.ToLower(), cancellationToken: cancellationToken))
            {
                return Op.Failed("ثبت سیستم با نام تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
            }
            await _context.Systems.AddAsync(new Systems
            {
                ComputerName = model.ComputerName,
                CPU = model.CPU.MultipleSpaceRemoverTrim(),
                Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                Location = model.Location.IsNotNull() ? model.Location.MultipleSpaceRemoverTrim() : null,
                MainBoardModel = model.MainBoardModel.MultipleSpaceRemoverTrim(),
                MainBoardName = model.MainBoardName.MultipleSpaceRemoverTrim(),
                OS = model.OS.MultipleSpaceRemoverTrim(),
                RAM = model.RAM.MultipleSpaceRemoverTrim(),
                SystemPicUrl = model.SystemPicUrl.IsNotNull() ? model.SystemPicUrl.Trim() : null,
                SystemTypeId = Convert.ToInt32(model.SystemTypeId != null ? model.SystemTypeId : systemTypes.FirstOrDefault(x => x.Name.ToUpper() == "DESKTOP").Id),
                User = model.User.IsNotNull() ? model.User.MultipleSpaceRemoverTrim() : string.Empty,
                UserPicUrl = model.User.IsNotNull() && model.UserPicUrl.IsNotNull() ? model.UserPicUrl.Trim() : null
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var system = await _context.Systems.FirstOrDefaultAsync(x => x.ComputerName.ToLower() == model.ComputerName.ToLower(), cancellationToken: cancellationToken);
            if (system == null)
            {
                return Op.Failed("ثبت سیستم جدید با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            await _context.SystemHards.AddRangeAsync(model.HardDisks.Select(x => new SystemHard
            {
                DiskName = x.DiskName.MultipleSpaceRemoverTrim(),
                DiskVolume = x.DiskVolume.ToEnglishNumber(),
                SystemId = system.Id
            }).ToList(), cancellationToken);

            await _context.SystemGraphics.AddRangeAsync(model.GraphicCards.Select(x => new SystemGraphic
            {
                GraphicCardName = x.GraphicCardName.MultipleSpaceRemoverTrim(),
                SystemId = system.Id
            }).ToList(), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("ثبت سیستم جدید با موفقیت انجام شد", new
            {
                system.Id,
                system.ComputerName,
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سیستم جدید با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<GetSystemDTO>> UpdateSystem(UpdateSystemDetailsDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<GetSystemDTO> Op = new("UpdateSystem");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت بروزرسانی سیستم وجود ندارد");
        }
        if (
            !model.ComputerName.IsNotNull() &&
            model.SystemTypeId == null &&
            !model.User.IsNotNull() &&
            !model.UserPicUrl.IsNotNull() &&
            !model.OS.IsNotNull() &&
            !model.RAM.IsNotNull() &&
            !model.MainBoardName.IsNotNull() &&
            !model.MainBoardModel.IsNotNull() &&
            !model.CPU.IsNotNull() &&
            (model.HardDisks == null || !model.HardDisks.Any()) &&
            (model.GraphicCards == null || !model.GraphicCards.Any()) &&
            !model.Description.IsNotNull() &&
            !model.Location.IsNotNull() &&
            !model.SystemPicUrl.IsNotNull()
          )
        {
            return Op.Failed("اطلاعاتی جهت بروزرسانی سیستم وجود ندارد");
        }
        if (model.SystemId <= 0)
        {
            return Op.Failed("شناسه سیستم بدرستی ارسال نشده است");
        }
        if (model.SystemTypeId != null && model.SystemTypeId <= 0)
        {
            return Op.Failed("شناسه نوع سیستم بدرستی ارسال نشده است");
        }
        if (model.ComputerName.IsNotNull())
        {
            if (model.ComputerName.Contains(' '))
            {
                return Op.Failed("امکان ثبت نام سیستم با 'فاصله' امکان پذیر نمیباشد");
            }
            if (model.ComputerName.Length > 300)
            {
                return Op.Failed($"حداکثر کاراکتر مجاز نام سیستم {300.ToPersianNumber()} کاراکتر است");
            }
        }
        if (model.User.IsNotNull() && model.User.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز کاربر {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.UserPicUrl.IsNotNull() && model.UserPicUrl.Length > 200)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز آدرس تصویر کاربر {200.ToPersianNumber()} کاراکتر است");
        }
        if (model.OS.IsNotNull() && model.OS.Length > 650)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز سیستم عامل {650.ToPersianNumber()} کاراکتر است");
        }
        if (model.RAM.IsNotNull())
        {
            if (!model.RAM.IsNumeric(false))
            {
                return Op.Failed("حافظه سیستم بدرستی ارسال نشده است");
            }
            if (model.RAM.Length > 40)
            {
                return Op.Failed($"حداکثر کاراکتر مجاز حافظه سیستم {40.ToPersianNumber()} کاراکتر است");
            }
        }
        if (model.MainBoardName.IsNotNull() && model.MainBoardName.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز نام Main Board سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.MainBoardModel.IsNotNull() && model.MainBoardModel.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز مدل Main Board سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.CPU.IsNotNull() && model.CPU.Length > 300)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز اطلاعات پردازنده سیستم {300.ToPersianNumber()} کاراکتر است");
        }
        if (model.HardDisks != null && model.HardDisks.Any())
        {
            if (model.HardDisks.Any(x => !x.DiskName.IsNotNull()))
            {
                return Op.Failed($"ارسال نام هارد دیسک اجباری است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskName.IsNotNull())) + 1} از لیست هارد دیسک ها");
            }
            if (model.HardDisks.Any(x => x.DiskName.Length > 250))
            {
                return Op.Failed($"حداکثر کاراکتر مجاز نام هارد دیسک {250.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.HardDisks.FindIndex(x => x.DiskName.Length > 250)) + 1} از لیست هارد دیسک ها");
            }
            if (model.HardDisks.Any(x => !x.DiskVolume.IsNotNull()))
            {
                return Op.Failed($"ارسال ظرفیت هارد دیسک اجباری است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskVolume.IsNotNull())) + 1} از لیست هارد دیسک ها");
            }
            if (model.HardDisks.Any(x => !x.DiskVolume.IsNumeric(false)))
            {
                return Op.Failed($"ظرفیت هارد دیسک بدرستی ارسال نشده است ، خطا در ردیف {(model.HardDisks.FindIndex(x => !x.DiskVolume.IsNumeric(false))) + 1} از لیست هارد دیسک ها");
            }
            if (model.HardDisks.Any(x => x.DiskVolume.Length > 50))
            {
                return Op.Failed($"حداکثر کاراکتر مجاز ظرفیت هارد دیسک {50.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.HardDisks.FindIndex(x => x.DiskVolume.Length > 50)) + 1} از لیست هارد دیسک ها");
            }
        }
        if (model.GraphicCards != null && model.GraphicCards.Any())
        {
            if (model.GraphicCards.Any(x => !x.GraphicCardName.IsNotNull()))
            {
                return Op.Failed($"ارسال نام کارت گرافیک اجباری است ، خطا در ردیف {(model.GraphicCards.FindIndex(x => !x.GraphicCardName.IsNotNull())) + 1} از لیست کارت های گرافیک");
            }
            if (model.GraphicCards.Any(x => x.GraphicCardName.Length > 250))
            {
                return Op.Failed($"حداکثر کاراکتر مجاز نام کارت گرافیک {250.ToPersianNumber()} کاراکتر میباشد ، خطا در ردیف {(model.GraphicCards.FindIndex(x => x.GraphicCardName.Length > 250)) + 1} از لیست کارت های گرافیک");
            }
        }
        if (model.Description.IsNotNull() && model.Description.Length > 1450)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز توضیحات {1450.ToPersianNumber()} کاراکتر است");
        }
        if (model.Location.IsNotNull() && model.Location.Length > 150)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز محل قرارگیری سیستم {150.ToPersianNumber()} کاراکتر است");
        }
        if (model.SystemPicUrl.IsNotNull() && model.SystemPicUrl.Length > 200)
        {
            return Op.Failed($"حداکثر کاراکتر مجاز آدرس تصویر سیستم {200.ToPersianNumber()} کاراکتر است");
        }
        try
        {
            var systemTypes = await _context.SystemTypes.ToListAsync(cancellationToken: cancellationToken);
            if (systemTypes == null || !systemTypes.Any() || !systemTypes.Any(x => x.Name.ToUpper() == "DESKTOP"))
            {
                return Op.Failed("اطلاعات انواع سیستم در سامانه ثبت نشده است", HttpStatusCode.NotFound);
            }
            if (model.SystemTypeId != null && !systemTypes.Any(x => x.Id == model.SystemTypeId))
            {
                return Op.Failed("شناسه نوع سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            var system = await _context.Systems.FirstOrDefaultAsync(x => x.Id == model.SystemId, cancellationToken: cancellationToken);
            if (system == null)
            {
                return Op.Failed("شناسه سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (model.ComputerName.IsNotNull() && await _context.Systems.AnyAsync(x => x.ComputerName.ToLower() == model.ComputerName.ToLower() && x.Id != system.Id, cancellationToken: cancellationToken))
            {
                return Op.Failed("بروزرسانی سیستم با نام تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.ComputerName.IsNotNull())
            {
                system.ComputerName = model.ComputerName;
            }
            if (model.SystemTypeId != null)
            {
                system.SystemTypeId = Convert.ToInt32(model.SystemTypeId);
            }
            system.User = model.User.IsNotNull() ? model.User.MultipleSpaceRemoverTrim() : string.Empty;
            system.UserPicUrl = system.User.IsNotNull() ? model.UserPicUrl.IsNotNull() ? model.UserPicUrl.Trim() : system.UserPicUrl : null;
            if (model.OS.IsNotNull())
            {
                system.OS = model.OS.MultipleSpaceRemoverTrim();
            }
            if (model.RAM.IsNotNull())
            {
                system.RAM = model.RAM.MultipleSpaceRemoverTrim();
            }
            if (model.MainBoardName.IsNotNull())
            {
                system.MainBoardName = model.MainBoardName.MultipleSpaceRemoverTrim();
            }
            if (model.MainBoardModel.IsNotNull())
            {
                system.MainBoardModel = model.MainBoardModel.MultipleSpaceRemoverTrim();
            }
            if (model.CPU.IsNotNull())
            {
                system.CPU = model.CPU.MultipleSpaceRemoverTrim();
            }
            if (model.Description.IsNotNull())
            {
                system.Description = model.Description.MultipleSpaceRemoverTrim();
            }
            if (model.Location.IsNotNull())
            {
                system.Location = model.Location.MultipleSpaceRemoverTrim();
            }
            if (model.SystemPicUrl.IsNotNull())
            {
                system.SystemPicUrl = model.SystemPicUrl.Trim();
            }

            _context.Entry<Systems>(system).State = EntityState.Modified;

            if (model.HardDisks != null && model.HardDisks.Any())
            {
                _context.SystemHards.RemoveRange(await _context.SystemHards.Where(x => x.SystemId == system.Id).ToListAsync(cancellationToken: cancellationToken));
                await _context.SystemHards.AddRangeAsync(model.HardDisks.Select(x => new SystemHard
                {
                    DiskName = x.DiskName.MultipleSpaceRemoverTrim(),
                    DiskVolume = x.DiskVolume.ToEnglishNumber(),
                    SystemId = system.Id
                }).ToList(), cancellationToken);
            }
            if (model.GraphicCards != null && model.GraphicCards.Any())
            {
                _context.SystemGraphics.RemoveRange(await _context.SystemGraphics.Where(x => x.SystemId == system.Id).ToListAsync(cancellationToken: cancellationToken));
                await _context.SystemGraphics.AddRangeAsync(model.GraphicCards.Select(x => new SystemGraphic
                {
                    GraphicCardName = x.GraphicCardName.MultipleSpaceRemoverTrim(),
                    SystemId = system.Id
                }).ToList(), cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Op.Succeed("بروزرسانی سیستم با موفقیت انجام شد", new GetSystemDTO
            {
                ComputerName = system.ComputerName,
                CPU = system.CPU,
                Location = system.Location,
                Description = system.Description,
                MainBoardModel = system.MainBoardModel,
                MainBoardName = system.MainBoardName,
                OS = system.OS,
                RAM = system.RAM,
                SystemId = system.Id,
                SystemPicUrl = system.SystemPicUrl,
                SystemTypeId = system.SystemTypeId,
                SystemTypeName = (from type in await _context.SystemTypes.ToListAsync(cancellationToken: cancellationToken)
                                  where type.Id == system.SystemTypeId
                                  select type.Name).FirstOrDefault(),
                User = system.User,
                UserPicUrl = system.UserPicUrl,
                Disks = (from disk in await _context.SystemHards.ToListAsync(cancellationToken: cancellationToken)
                         where disk.SystemId == system.Id
                         select new GetSystem_DiskDTO
                         {
                             DiskId = disk.Id,
                             DiskName = disk.DiskName,
                             DiskVolume = disk.DiskVolume,
                         }).ToList(),
                Graphics = (from graphic in await _context.SystemGraphics.ToListAsync(cancellationToken: cancellationToken)
                            where graphic.SystemId == system.Id
                            select new GetSystem_GraphicDTO
                            {
                                GraphicCardId = graphic.Id,
                                GraphicCardName = graphic.GraphicCardName
                            }).ToList()
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی سیستم با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteSystem(SystemIdDTO requestDTO, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteSystem");
        if (requestDTO == null || requestDTO.SystemId <= 0)
        {
            return Op.Failed("شناسه سیستم بدرستی ارسال نشده است");
        }
        try
        {
            var system = await _context.Systems.FirstOrDefaultAsync(x => x.Id == requestDTO.SystemId, cancellationToken: cancellationToken);
            if (system == null)
            {
                return Op.Failed("شناسه سیستم بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            _context.SystemHards.RemoveRange(await _context.SystemHards.Where(x => x.SystemId == requestDTO.SystemId).ToListAsync(cancellationToken: cancellationToken));
            _context.SystemGraphics.RemoveRange(await _context.SystemGraphics.Where(x => x.SystemId == requestDTO.SystemId).ToListAsync(cancellationToken: cancellationToken));
            _context.Systems.Remove(system);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("حذف سیستم با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("حذف سیستم با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }
}

