using Microsoft.EntityFrameworkCore;
using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Framework;
using OrganizationChart.API.Infrastructure.Persistence;
using System.Linq;
using System.Net;

namespace OrganizationChart.API.Services
{
    public class SystemUpsRepository : ISystemUpsRepository
    {
        private readonly OrganizationChartDataContext _context;
        public SystemUpsRepository(OrganizationChartDataContext context)
        {
            this._context = context;
        }

        public async Task<OperationResult<object>> CreateUps(CreateUpsDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateUps");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.ProjectId <= 0)
            {
                return Op.Failed("شناسه پروژه بدرستی ارسال نشده است");
            }
            if (!model.Location.IsNotNull())
            {
                return Op.Failed("ارسال محل نصب اجباری میباشد");
            }
            if (model.Location.MultipleSpaceRemoverTrim().Length < 2 || model.Location.MultipleSpaceRemoverTrim().Length > 250)
            {
                return Op.Failed($"تعداد کاراکتر مجاز محل نصب از {2.ToPersianNumber()} تا {250.ToPersianNumber()} کاراکتر است");
            }
            if (!model.Model.IsNotNull())
            {
                return Op.Failed("ارسال مدل دستگاه اجباری میباشد");
            }
            if (model.Model.MultipleSpaceRemoverTrim().Length < 2 || model.Model.MultipleSpaceRemoverTrim().Length > 210)
            {
                return Op.Failed($"تعداد کاراکتر مجاز مدل دستگاه از {2.ToPersianNumber()} تا {210.ToPersianNumber()} کاراکتر است");
            }
            if (!model.PowerInKVA.IsNotNull())
            {
                return Op.Failed("ارسال توان اجباری میباشد");
            }
            if (model.PowerInKVA.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توان از {1.ToPersianNumber()} تا {35.ToPersianNumber()} کاراکتر است");
            }
            if (model.BatteryCapacityInAH.IsNotNull() && model.BatteryCapacityInAH.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed($"تعداد کاراکتر مجاز ظرفیت باتری از {1.ToPersianNumber()} تا {35.ToPersianNumber()} کاراکتر است");
            }
            if (model.BatteryCount != null && model.BatteryCount < 0)
            {
                return Op.Failed("تعداد باتری بدرستی ارسال نشده است");
            }
            if (model.MaintenanceEnvironment.IsNotNull() && model.MaintenanceEnvironment.MultipleSpaceRemoverTrim().Length > 210)
            {
                return Op.Failed($"تعداد کاراکتر مجاز محیط نگهداری از {1.ToPersianNumber()} تا {210.ToPersianNumber()} کاراکتر است");
            }
            if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 2000)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {1.ToPersianNumber()} تا {2000.ToPersianNumber()} کاراکتر است");
            }
            if (model.Devices != null && model.Devices.Any())
            {
                if (model.Devices.Distinct().Any(x => !x.IsNotNull()))
                {
                    return Op.Failed($"نام دستگاه مصرف کننده بدرستی ارسال نشده است ، خطا در ردیف {model.Devices.Distinct().ToList().FindIndex(x => !x.IsNotNull()) + 1}");
                }
                if (model.Devices.Distinct().Any(x => x.MultipleSpaceRemoverTrim().Length > 150))
                {
                    return Op.Failed($"نام دستگاه مصرف کننده بدرستی ارسال نشده است ، خطا در ردیف {model.Devices.Distinct().ToList().FindIndex(x => x.MultipleSpaceRemoverTrim().Length > 150) + 1}");
                }
            }
            try
            {
                if (!await _context.SystemProjects.AnyAsync(x => x.Id == model.ProjectId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه پروژه بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (await _context.SystemUPSes.AnyAsync(x => x.ProjectId == model.ProjectId && x.Location.ToLower() == model.Location.MultipleSpaceRemoverTrim().ToLower() && x.Model.ToLower() == model.Model.MultipleSpaceRemoverTrim().ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت اطلاعات تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                await _context.SystemUPSes.AddAsync(new Models.SystemUPS
                {
                    BatteryCapacityInAH = model.BatteryCapacityInAH.IsNotNull() ? model.BatteryCapacityInAH.MultipleSpaceRemoverTrim() : null,
                    BatteryCount = model.BatteryCount,
                    BatteryPurchaseDate = model.BatteryPurchaseDate,
                    BatteryUsageDate = model.BatteryUsageDate,
                    Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                    LatestServiceDate = null,
                    Location = model.Location.MultipleSpaceRemoverTrim(),
                    MaintenanceEnvironment = model.MaintenanceEnvironment.IsNotNull() ? model.MaintenanceEnvironment.MultipleSpaceRemoverTrim() : null,
                    Model = model.Model.MultipleSpaceRemoverTrim(),
                    PowerInKVA = model.PowerInKVA.MultipleSpaceRemoverTrim(),
                    ProjectId = model.ProjectId,
                    PurchaseDate = model.PurchaseDate,
                    Devices = model.Devices != null && model.Devices.Any() ? string.Join(",", model.Devices.Where(x => x.IsNotNull()).Distinct().Select(x => x.MultipleSpaceRemoverTrim()).ToList()) : null
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت اطلاعات با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> UpdateUps(UpdateUpsDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateUps");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت بروزرسانی UPS ارسال نشده است");
            }
            if (model.UpsId <= 0)
            {
                return Op.Failed("شناسه UPS بدرستی ارسال نشده است");
            }
            if (
                model.ProjectId == null &&
                !model.Location.IsNotNull() &&
                !model.Model.IsNotNull() &&
                !model.PowerInKVA.IsNotNull() &&
                !model.BatteryCapacityInAH.IsNotNull() &&
                model.BatteryCount == null &&
                !model.MaintenanceEnvironment.IsNotNull() &&
                model.PurchaseDate == null &&
                model.BatteryPurchaseDate == null &&
                model.BatteryUsageDate == null &&
                !model.Description.IsNotNull() &&
                (model.Devices == null || !model.Devices.Any())
              )
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.ProjectId != null && model.ProjectId <= 0)
            {
                return Op.Failed("شناسه پروژه بدرستی ارسال نشده است");
            }
            if (model.Location.IsNotNull() && (model.Location.MultipleSpaceRemoverTrim().Length < 2 || model.Location.MultipleSpaceRemoverTrim().Length > 250))
            {
                return Op.Failed($"تعداد کاراکتر مجاز محل نصب از {2.ToPersianNumber()} تا {250.ToPersianNumber()} کاراکتر است");
            }
            if (model.Model.IsNotNull() && (model.Model.MultipleSpaceRemoverTrim().Length < 2 || model.Model.MultipleSpaceRemoverTrim().Length > 210))
            {
                return Op.Failed($"تعداد کاراکتر مجاز مدل دستگاه از {2.ToPersianNumber()} تا {210.ToPersianNumber()} کاراکتر است");
            }
            if (model.PowerInKVA.IsNotNull() && model.PowerInKVA.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توان از {1.ToPersianNumber()} تا {35.ToPersianNumber()} کاراکتر است");
            }
            if (model.BatteryCapacityInAH.IsNotNull() && model.BatteryCapacityInAH.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed($"تعداد کاراکتر مجاز ظرفیت باتری از {1.ToPersianNumber()} تا {35.ToPersianNumber()} کاراکتر است");
            }
            if (model.BatteryCount != null && model.BatteryCount < 0)
            {
                return Op.Failed("تعداد باتری بدرستی ارسال نشده است");
            }
            if (model.MaintenanceEnvironment.IsNotNull() && model.MaintenanceEnvironment.MultipleSpaceRemoverTrim().Length > 210)
            {
                return Op.Failed($"تعداد کاراکتر مجاز محیط نگهداری از {1.ToPersianNumber()} تا {210.ToPersianNumber()} کاراکتر است");
            }
            if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 2000)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {1.ToPersianNumber()} تا {2000.ToPersianNumber()} کاراکتر است");
            }
            if (model.Devices != null && model.Devices.Any())
            {
                if (model.Devices.Distinct().Any(x => !x.IsNotNull()))
                {
                    return Op.Failed($"نام دستگاه مصرف کننده بدرستی ارسال نشده است ، خطا در ردیف {model.Devices.Distinct().ToList().FindIndex(x => !x.IsNotNull()) + 1}");
                }
                if (model.Devices.Distinct().Any(x => x.MultipleSpaceRemoverTrim().Length > 150))
                {
                    return Op.Failed($"نام دستگاه مصرف کننده بدرستی ارسال نشده است ، خطا در ردیف {model.Devices.Distinct().ToList().FindIndex(x => x.MultipleSpaceRemoverTrim().Length > 150) + 1}");
                }
            }
            try
            {
                var ups = await _context.SystemUPSes.FirstOrDefaultAsync(x => x.Id == model.UpsId, cancellationToken: cancellationToken);
                if (ups == null)
                {
                    return Op.Failed("شناسه UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.ProjectId != null && !await _context.SystemProjects.AnyAsync(x => x.Id == model.ProjectId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه پروژه بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if ((model.Location.IsNotNull() || model.Model.IsNotNull()) && await _context.SystemUPSes.AnyAsync(x => x.Id != model.UpsId && x.ProjectId == (model.ProjectId == null ? ups.ProjectId : Convert.ToInt32(model.ProjectId)) && x.Location.ToLower() == (model.Location.IsNotNull() ? model.Location.MultipleSpaceRemoverTrim().ToLower() : ups.Location.ToLower()) && x.Model.ToLower() == (model.Model.IsNotNull() ? model.Model.MultipleSpaceRemoverTrim().ToLower() : ups.Model.ToLower()), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت اطلاعات تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.ProjectId != null)
                {
                    ups.ProjectId = Convert.ToInt32(model.ProjectId);
                }
                if (model.Location.IsNotNull())
                {
                    ups.Location = model.Location.MultipleSpaceRemoverTrim();
                }
                if (model.Model.IsNotNull())
                {
                    ups.Model = model.Model.MultipleSpaceRemoverTrim();
                }
                if (model.PowerInKVA.IsNotNull())
                {
                    ups.PowerInKVA = model.PowerInKVA.MultipleSpaceRemoverTrim();
                }
                if (model.BatteryCapacityInAH.IsNotNull())
                {
                    ups.BatteryCapacityInAH = model.BatteryCapacityInAH.MultipleSpaceRemoverTrim();
                }
                if (model.BatteryCount != null)
                {
                    ups.BatteryCount = model.BatteryCount;
                }
                if (model.MaintenanceEnvironment.IsNotNull())
                {
                    ups.MaintenanceEnvironment = model.MaintenanceEnvironment.MultipleSpaceRemoverTrim();
                }
                if (model.PurchaseDate != null)
                {
                    ups.PurchaseDate = Convert.ToDateTime(model.PurchaseDate);
                }
                if (model.BatteryPurchaseDate != null)
                {
                    ups.BatteryPurchaseDate = Convert.ToDateTime(model.BatteryPurchaseDate);
                }
                if (model.BatteryUsageDate != null)
                {
                    ups.BatteryUsageDate = Convert.ToDateTime(model.BatteryUsageDate);
                }
                if (model.Description.IsNotNull())
                {
                    ups.Description = model.Description.MultipleSpaceRemoverTrim();
                }
                if (model.Devices != null && model.Devices.Any())
                {
                    ups.Devices = string.Join(",", model.Devices.Where(x => x.IsNotNull()).Distinct().Select(x => x.MultipleSpaceRemoverTrim()).ToList());
                }
                _context.Entry<Models.SystemUPS>(ups).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی اطلاعات UPS با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> CreateUpsService(CreateUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateUpsService");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.UpsId < 0)
            {
                return Op.Failed("شناسه UPS بدرستی ارسال نشده است");
            }
            if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 2000)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {1.ToPersianNumber()} تا {2000.ToPersianNumber()} کاراکتر است");
            }
            if (model.AttachmentUrl.IsNotNull())
            {
                if (model.AttachmentUrl.Contains(' '))
                {
                    return Op.Failed("آدرس فایل سروریس UPS بدرستی ارسال نشده است");
                }
                if (model.AttachmentUrl.Length > 180)
                {
                    return Op.Failed($"تعداد کاراکتر مجاز آدرس فایل سروریس UPS از {1.ToPersianNumber()} تا {180.ToPersianNumber()} کاراکتر است");
                }
            }
            try
            {
                var ups = await _context.SystemUPSes.Include(x => x.UpsServices).FirstOrDefaultAsync(x => x.Id == model.UpsId, cancellationToken: cancellationToken);
                if (ups == null)
                {
                    return Op.Failed("شناسه UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                List<DateTime> serviceDates = new() { model.Date == null ? Op.OperationDate : Convert.ToDateTime(model.Date) };
                if (ups.UpsServices != null && ups.UpsServices.Any())
                {
                    serviceDates.AddRange(ups.UpsServices.Select(x => x.Date).ToList());
                }
                await _context.SystemUpsServices.AddAsync(new Models.SystemUpsService
                {
                    AttachmentUrl = model.AttachmentUrl.IsNotNull() ? model.AttachmentUrl : null,
                    Date = model.Date == null ? Op.OperationDate : Convert.ToDateTime(model.Date),
                    Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                    UpsId = ups.Id,
                }, cancellationToken);
                ups.LatestServiceDate = serviceDates.FirstOrDefault(x => x.Ticks == serviceDates.Select(x => x.Ticks).Max());
                _context.Entry<Models.SystemUPS>(ups).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت سرویس برای UPS با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت سرویس UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> UpdateUpsService(UpdateUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateUpsService");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.UpsServiceId <= 0)
            {
                return Op.Failed("شناسه سرویس UPS بدرستی ارسال نشده است");
            }
            if (!model.Description.IsNotNull() && !model.AttachmentUrl.IsNotNull())
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 2000)
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {1.ToPersianNumber()} تا {2000.ToPersianNumber()} کاراکتر است");
            }
            if (model.AttachmentUrl.IsNotNull())
            {
                if (model.AttachmentUrl.Contains(' '))
                {
                    return Op.Failed("آدرس فایل سروریس UPS بدرستی ارسال نشده است");
                }
                if (model.AttachmentUrl.Length > 180)
                {
                    return Op.Failed($"تعداد کاراکتر مجاز آدرس فایل سروریس UPS از {1.ToPersianNumber()} تا {180.ToPersianNumber()} کاراکتر است");
                }
            }
            try
            {
                var service = await _context.SystemUpsServices.FirstOrDefaultAsync(x => x.Id == model.UpsServiceId, cancellationToken: cancellationToken);
                if (service == null)
                {
                    return Op.Failed("شناسه سرویس UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.Description.IsNotNull())
                {
                    service.Description = model.Description.MultipleSpaceRemoverTrim();
                }
                if (model.AttachmentUrl.IsNotNull())
                {
                    service.AttachmentUrl = model.AttachmentUrl;
                }
                _context.Entry<Models.SystemUpsService>(service).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی سرویس UPS با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی سرویس UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<GetUpsDTO>> GetUPSes(CancellationToken cancellationToken = default)
        {
            OperationResult<GetUpsDTO> Op = new("GetUPSes");
            try
            {
                var data = (from ups in await _context.SystemUPSes.Include(x => x.Project).Include(x => x.UpsServices).ToListAsync(cancellationToken: cancellationToken)
                            select new GetUpsDTO
                            {
                                BatteryCapacityInAH = ups.BatteryCapacityInAH,
                                BatteryCount = ups.BatteryCount,
                                BatteryPurchaseDate = ups.BatteryPurchaseDate,
                                BatteryUsageDate = ups.BatteryUsageDate,
                                Description = ups.Description,
                                Devices = ups.Devices.IsNotNull() ? ups.Devices.Split(',').ToList() : null,
                                Id = ups.Id,
                                LatestServiceDate = ups.LatestServiceDate,
                                Location = ups.Location,
                                MaintenanceEnvironment = ups.MaintenanceEnvironment,
                                Model = ups.Model,
                                PowerInKVA = ups.PowerInKVA,
                                ProjectId = ups.ProjectId,
                                ProjectName = ups.Project.Name,
                                PurchaseDate = ups.PurchaseDate,
                                Services = ups.UpsServices != null && ups.UpsServices.Any() ? ups.UpsServices.Select(x => new GetUps_ServicesDTO
                                {
                                    AttachmentUrl = x.AttachmentUrl,
                                    ServiceDate = x.Date,
                                    ServiceDescription = x.Description,
                                    ServiceId = x.Id,
                                }).ToList() : null
                            }).ToList();
                if (data == null || !data.Any())
                {
                    return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", data);
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست UPS ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<GetUpsDTO>> GetUPS(int Id, CancellationToken cancellationToken = default)
        {
            OperationResult<GetUpsDTO> Op = new("GetUPS");
            if (Id <= 0)
            {
                return Op.Failed("شناسه UPS بدرستی ارسال نشده است");
            }
            try
            {
                var ups = await _context.SystemUPSes.Include(x => x.Project).Include(x => x.UpsServices).FirstOrDefaultAsync(x => x.Id == Id, cancellationToken: cancellationToken);
                if (ups == null)
                {
                    return Op.Failed("شناسه UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                return Op.Succeed("دریافت اطلاعات UPS با موفقیت انجام شد", new GetUpsDTO
                {
                    BatteryCapacityInAH = ups.BatteryCapacityInAH,
                    BatteryCount = ups.BatteryCount,
                    BatteryPurchaseDate = ups.BatteryPurchaseDate,
                    BatteryUsageDate = ups.BatteryUsageDate,
                    Description = ups.Description,
                    Devices = ups.Devices.IsNotNull() ? ups.Devices.Split(',').ToList() : null,
                    Id = ups.Id,
                    LatestServiceDate = ups.LatestServiceDate,
                    Location = ups.Location,
                    MaintenanceEnvironment = ups.MaintenanceEnvironment,
                    Model = ups.Model,
                    PowerInKVA = ups.PowerInKVA,
                    ProjectId = ups.ProjectId,
                    ProjectName = ups.Project.Name,
                    PurchaseDate = ups.PurchaseDate,
                    Services = ups.UpsServices != null && ups.UpsServices.Any() ? ups.UpsServices.Select(x => new GetUps_ServicesDTO
                    {
                        AttachmentUrl = x.AttachmentUrl,
                        ServiceDate = x.Date,
                        ServiceDescription = x.Description,
                        ServiceId = x.Id,
                    }).ToList() : null
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<GetSystemProjectDTO>> GetUpsProjects(CancellationToken cancellationToken = default)
        {
            OperationResult<GetSystemProjectDTO> Op = new("GetUpsProjects");
            try
            {
                var data = (from x in await _context.SystemProjects.ToListAsync(cancellationToken: cancellationToken)
                            select new GetSystemProjectDTO
                            {
                                Id = x.Id,
                                Name = x.Name,
                            }).ToList();
                if (data == null || !data.Any())
                {
                    return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", data);
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات پروژه ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteUpsService(DeleteUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteUpsService");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت حذف سرویس UPS ارسال نشده است");
            }
            if (model.UpsServiceId <= 0)
            {
                return Op.Failed("شناسه سرویس UPS بدرستی ارسال نشده است");
            }
            try
            {
                var service = await _context.SystemUpsServices.FirstOrDefaultAsync(x => x.Id == model.UpsServiceId, cancellationToken: cancellationToken);
                if (service == null)
                {
                    return Op.Failed("شناسه سرویس UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var ups = await _context.SystemUPSes.FirstOrDefaultAsync(x => x.Id == service.UpsId, cancellationToken: cancellationToken);
                if (ups == null)
                {
                    return Op.Failed("شناسه سرویس UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                List<DateTime> serviceDates = (from ser in await _context.SystemUpsServices.ToListAsync(cancellationToken: cancellationToken)
                                               where ser.UpsId == service.UpsId && ser.Id != service.Id
                                               select ser.Date).ToList();
                ups.LatestServiceDate = serviceDates != null && serviceDates.Any() ? serviceDates.FirstOrDefault(x => x.Ticks == serviceDates.Select(x => x.Ticks).Max()) : null;
                _context.Entry<Models.SystemUPS>(ups).State = EntityState.Modified;
                _context.SystemUpsServices.Remove(service);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف سرویس UPS با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("حذف سرویس UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteUps(DeleteUpsDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteUps");
            if (model == null)
            {
                return Op.Failed("ارسال شناسه UPS اجباری است");
            }
            if (model.UpsId <= 0)
            {
                return Op.Failed("شناسه UPS بدرستی ارسال نشده است");
            }
            try
            {
                var ups = await _context.SystemUPSes.FirstOrDefaultAsync(x => x.Id == model.UpsId, cancellationToken: cancellationToken);
                if (ups == null)
                {
                    return Op.Failed("شناسه UPS بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                _context.SystemUpsServices.RemoveRange(await _context.SystemUpsServices.Where(x => x.UpsId == model.UpsId).ToListAsync(cancellationToken: cancellationToken));
                _context.SystemUPSes.Remove(ups);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف UPS با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف UPS با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }


    }
}
