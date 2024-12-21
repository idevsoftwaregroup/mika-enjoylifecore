using core.application.Contract.API.DTO.Reservation.Joint;
using core.application.Contract.API.DTO.Reservation.Joint.Filters;
using core.application.Contract.API.DTO.Reservation.Joint.Middles;
using core.application.Contract.API.DTO.Reservation.Reservation;
using core.application.Contract.API.DTO.Reservation.Reservation.Filter;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.ReservationDTOs;
using core.domain.entity;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.enums;
using core.domain.entity.ReservationModels.JointModels;
using core.domain.entity.ReservationModels.ReservationModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Net;
using System.Numerics;

namespace core.infrastructure.Data.repository;

public class ReservationRepository : IReservationRepository
{
    private readonly EnjoyLifeContext _context;
    public ReservationRepository(EnjoyLifeContext context)
    {
        this._context = context;
    }

    public async Task<OperationResult<object>> UpdateJoint(Request_UpdateJointDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdateJoint");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (
            model.ComplexId == null &&
            model.JointStatusId == null &&
            !model.Title.IsNotNull() &&
            !model.Location.IsNotNull() &&
            (model.PhoneNumbers == null || !model.PhoneNumbers.Any()) &&
            !model.Description.IsNotNull() &&
            !model.TermsText.IsNotNull() &&
            !model.TermsFileUrl.IsNotNull() &&
            (model.MultiMedias == null || !model.MultiMedias.Any()) &&
            (model.DailyActivityHours == null || !model.DailyActivityHours.Any()) &&
            (model.DailyUnitReservationCount == null || model.DailyUnitReservationCount <= 0) &&
            (model.WeeklyUnitReservationCount == null || model.WeeklyUnitReservationCount <= 0) &&
            (model.MonthlyUnitReservationCount == null || model.MonthlyUnitReservationCount <= 0) &&
            (model.YearlyUnitReservationCount == null || model.YearlyUnitReservationCount <= 0)
          )
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
        }
        if (model.ComplexId != null && model.ComplexId <= 0)
        {
            return Op.Failed("شناسه ساختمان بدرستی ارسال نشدخ است");
        }
        if (model.JointStatusId != null && model.JointStatusId <= 0)
        {
            return Op.Failed("شناسه وضعیت مشاع بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 100))
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای عنوان مشاع از {2.ToPersianNumber()} تا {100.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.Location.IsNotNull() && model.Location.MultipleSpaceRemoverTrim().Length > 150)
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای موقعیت جغرافیایی مشاع از {1.ToPersianNumber()} تا {150.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.PhoneNumbers != null && model.PhoneNumbers.Any() && model.PhoneNumbers.Any(x => !x.IsNotNull() || !x.IsNumeric(false) || x.Length < 3 || x.Length > 15))
        {
            return Op.Failed(model.PhoneNumbers.Count == 1 ? "شماره تماس شاع بدرستی ارسال نشده است" : $"شماره تماس شاع بدرستی ارسال نشده است، خطا در ردیف: {model.PhoneNumbers.FindIndex(x => !x.IsNotNull() || !x.IsNumeric(false) || x.Length < 3 || x.Length > 15) + 1}");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 3 || model.Description.MultipleSpaceRemoverTrim().Length > 2300))
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای توضیحات مشاع از {3.ToPersianNumber()} تا {2300.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.TermsText.IsNotNull() && model.TermsText.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای مقررات مشاع از {1.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.TermsFileUrl.IsNotNull() && (model.TermsFileUrl.Contains(' ') || model.TermsFileUrl.Length > 190))
        {
            return Op.Failed("آدرس فایل مقررات مشاع بدرستی ارسال نشده است");
        }
        if (model.MultiMedias != null && model.MultiMedias.Any())
        {
            if (model.MultiMedias.Any(x => !x.Url.IsNotNull() || x.Url.Contains(' ') || x.Url.Length > 190))
            {
                return Op.Failed(model.MultiMedias.Count == 1 ? "آدرس فایل مشاع بدرستی ارسال نشده است" : $"آدرس فایل مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.MultiMedias.FindIndex(x => !x.Url.IsNotNull() || x.Url.Contains(' ') || x.Url.Length > 190) + 1}");
            }
            if (model.MultiMedias.Any(x => x.Alt.IsNotNull() && x.Alt.Length > 230))
            {
                return Op.Failed(model.MultiMedias.Count == 1 ? "توضیح فایل مشاع بدرستی ارسال نشده است" : $"توضیح فایل مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.MultiMedias.FindIndex(x => x.Alt.IsNotNull() && x.Alt.Length > 230) + 1}");
            }
        }
        if (model.DailyActivityHours != null && model.DailyActivityHours.Any())
        {
            if (model.DailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() > x.PartialEndTime.ResetTimeSpan()))
            {
                return Op.Failed(model.DailyActivityHours.Count == 1 ? "بازه زمانی فعالیت مشاع بدرستی ارسال نشده است" : $"بازه زمانی فعالیت مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.DailyActivityHours.FindIndex(x => x.PartialStartTime.TimeOfDay > x.PartialEndTime.TimeOfDay) + 1}");
            }
            if (model.DailyActivityHours.Count > 1)
            {
                var checkStart = model.DailyActivityHours.OrderBy(x => x.PartialStartTime).FirstOrDefault().PartialStartTime.ResetTimeSpan();
                var checkEnd = model.DailyActivityHours.OrderBy(x => x.PartialStartTime).FirstOrDefault().PartialEndTime.ResetTimeSpan();
                for (var i = 1; i < model.DailyActivityHours.Count; i++)
                {
                    if (
                        (model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan() < checkStart && model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan() <= checkEnd) ||
                        (model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan() >= checkEnd && model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan() > checkEnd)
                       )
                    {
                        checkStart = model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan();
                        checkEnd = model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan();
                    }
                    else
                    {
                        return Op.Failed($"بازه زمانی فعالیت مشاع بدرستی ارسال نشده است، خطا در ردیف: {i + 1}");
                    }
                }
            }
        }
        if (model.DailyUnitReservationCount != null && model.DailyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو روزانه هر واحد بدرستی ارسال نشده است");
        }
        if (model.WeeklyUnitReservationCount != null && model.WeeklyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو هفتگی هر واحد بدرستی ارسال نشده است");
        }
        if (model.MonthlyUnitReservationCount != null && model.MonthlyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو ماهانه هر واحد بدرستی ارسال نشده است");
        }
        if (model.YearlyUnitReservationCount != null && model.YearlyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو سالیانه هر واحد بدرستی ارسال نشده است");
        }
        try
        {
            var joint = await _context.Joints.FirstOrDefaultAsync(x => x.JointId == model.JointId, cancellationToken: cancellationToken);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (model.ComplexId != null && !await _context.Complexes.AnyAsync(x => x.Id == model.ComplexId && x.IsDelete != true, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه ساختمان بدرستی ارسال نشدخ است", HttpStatusCode.NotFound);
            }
            if (model.JointStatusId != null && !await _context.JointStatuses.AnyAsync(x => x.JointStatusId == model.JointStatusId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه وضعیت مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (model.Title.IsNotNull() && await _context.Joints.AnyAsync(x => x.Title.ToLower() == model.Title.MultipleSpaceRemoverTrim().ToLower() && x.ComplexId == model.ComplexId && x.JointId != joint.JointId, cancellationToken: cancellationToken))
            {
                return Op.Failed("ثبت مشاع با عنوان تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
            }

            if (model.ComplexId != null)
            {
                joint.ComplexId = Convert.ToInt32(model.ComplexId);
            }
            if (model.JointStatusId != null)
            {
                joint.JointStatusId = Convert.ToInt32(model.JointStatusId);
            }
            if (model.Title.IsNotNull())
            {
                joint.Title = model.Title.MultipleSpaceRemoverTrim();
            }
            if (model.Location.IsNotNull())
            {
                joint.Location = model.Location.MultipleSpaceRemoverTrim();
            }
            if (model.PhoneNumbers != null && model.PhoneNumbers.Any())
            {
                joint.PhoneNumbers = string.Join(",", model.PhoneNumbers);
            }
            if (model.Description.IsNotNull())
            {
                joint.Description = model.Description.MultipleSpaceRemoverTrim();
            }
            if (model.TermsText.IsNotNull())
            {
                joint.TermsText = model.TermsText.MultipleSpaceRemoverTrim();
            }
            if (model.TermsFileUrl.IsNotNull())
            {
                joint.TermsFileUrl = model.TermsFileUrl;
            }
            joint.DailyUnitReservationCount = model.DailyUnitReservationCount;
            joint.WeeklyUnitReservationCount = model.WeeklyUnitReservationCount;
            joint.MonthlyUnitReservationCount = model.MonthlyUnitReservationCount;
            joint.YearlyUnitReservationCount = model.YearlyUnitReservationCount;
            if (model.MultiMedias != null && model.MultiMedias.Any())
            {
                joint.ThumbnailUrl = model.MultiMedias.Any(x => x.IsThumbnail == true) ? model.MultiMedias.FirstOrDefault(x => x.IsThumbnail == true).Url : model.MultiMedias.FirstOrDefault().Url;
                _context.JointMultiMedias.RemoveRange(await _context.JointMultiMedias.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken));
                await _context.JointMultiMedias.AddRangeAsync(model.MultiMedias.Select(x => new JointMultiMedia
                {
                    Alt = x.Alt.IsNotNull() ? x.Alt.MultipleSpaceRemoverTrim() : joint.Title,
                    MediaType = x.MediaType == null ? MediaType.IMAGE : (MediaType)x.MediaType,
                    Url = x.Url,
                    JointId = joint.JointId

                }), cancellationToken);
            }
            _context.Entry<Joint>(joint).State = EntityState.Modified;
            if (model.DailyActivityHours != null && model.DailyActivityHours.Any())
            {
                _context.JointDailyActivityHours.RemoveRange(await _context.JointDailyActivityHours.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken));
                await _context.JointDailyActivityHours.AddRangeAsync(model.DailyActivityHours.Select(x => new domain.entity.ReservationModels.JointModels.JointDailyActivityHour
                {
                    PartialStartTime = x.PartialStartTime.ResetTimeSpan(),
                    PartialEndTime = x.PartialEndTime.ResetTimeSpan(),
                    JointId = joint.JointId
                }).ToList(), cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("بروزرسانی اطلاعات مشاع با موفقیت انجام شد");

        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی مشاع با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }


    }
    public async Task<OperationResult<object>> CreateJoint(Request_CreateJointDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateJoint");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (model.ComplexId <= 0)
        {
            return Op.Failed("شناسه ساختمان بدرستی ارسال نشدخ است");
        }
        if (model.JointStatusId <= 0)
        {
            return Op.Failed("شناسه وضعیت مشاع بدرستی ارسال نشده است");
        }
        if (!model.Title.IsNotNull())
        {
            return Op.Failed("ارسال عنوان مشاع اجباری است");
        }
        if (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 100)
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای عنوان مشاع از {2.ToPersianNumber()} تا {100.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.Location.IsNotNull() && model.Location.MultipleSpaceRemoverTrim().Length > 150)
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای موقعیت جغرافیایی مشاع از {1.ToPersianNumber()} تا {150.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.PhoneNumbers != null && model.PhoneNumbers.Any() && model.PhoneNumbers.Any(x => !x.IsNotNull() || !x.IsNumeric(false) || x.Length < 3 || x.Length > 15))
        {
            return Op.Failed(model.PhoneNumbers.Count == 1 ? "شماره تماس شاع بدرستی ارسال نشده است" : $"شماره تماس شاع بدرستی ارسال نشده است، خطا در ردیف: {model.PhoneNumbers.FindIndex(x => !x.IsNotNull() || !x.IsNumeric(false) || x.Length < 3 || x.Length > 15) + 1}");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 3 || model.Description.MultipleSpaceRemoverTrim().Length > 2300))
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای توضیحات مشاع از {3.ToPersianNumber()} تا {2300.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.TermsText.IsNotNull() && model.TermsText.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed($"تعداد کاراکتر مجاز برای مقررات مشاع از {1.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر میباشد");
        }
        if (model.TermsFileUrl.IsNotNull() && (model.TermsFileUrl.Contains(' ') || model.TermsFileUrl.Length > 190))
        {
            return Op.Failed("آدرس فایل مقررات مشاع بدرستی ارسال نشده است");
        }
        if (model.MultiMedias != null && model.MultiMedias.Any())
        {
            if (model.MultiMedias.Any(x => !x.Url.IsNotNull() || x.Url.Contains(' ') || x.Url.Length > 190))
            {
                return Op.Failed(model.MultiMedias.Count == 1 ? "آدرس فایل مشاع بدرستی ارسال نشده است" : $"آدرس فایل مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.MultiMedias.FindIndex(x => !x.Url.IsNotNull() || x.Url.Contains(' ') || x.Url.Length > 190) + 1}");
            }
            if (model.MultiMedias.Any(x => x.Alt.IsNotNull() && x.Alt.Length > 230))
            {
                return Op.Failed(model.MultiMedias.Count == 1 ? "توضیح فایل مشاع بدرستی ارسال نشده است" : $"توضیح فایل مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.MultiMedias.FindIndex(x => x.Alt.IsNotNull() && x.Alt.Length > 230) + 1}");
            }
        }
        if (model.DailyActivityHours == null || !model.DailyActivityHours.Any())
        {
            return Op.Failed("حداقل یک بازه زمانی برای ثبت زمان فعالیت روزانه مشاع اجباری است");
        }
        if (model.DailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() > x.PartialEndTime.ResetTimeSpan()))
        {
            return Op.Failed(model.DailyActivityHours.Count == 1 ? "بازه زمانی فعالیت مشاع بدرستی ارسال نشده است" : $"بازه زمانی فعالیت مشاع بدرستی ارسال نشده است، خطا در ردیف: {model.DailyActivityHours.FindIndex(x => x.PartialStartTime.TimeOfDay > x.PartialEndTime.TimeOfDay) + 1}");
        }
        if (model.DailyActivityHours.Count > 1)
        {
            var checkStart = model.DailyActivityHours.OrderBy(x => x.PartialStartTime).FirstOrDefault().PartialStartTime.ResetTimeSpan();
            var checkEnd = model.DailyActivityHours.OrderBy(x => x.PartialStartTime).FirstOrDefault().PartialEndTime.ResetTimeSpan();
            for (var i = 1; i < model.DailyActivityHours.Count; i++)
            {
                if (
                    (model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan() < checkStart && model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan() <= checkEnd) ||
                    (model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan() >= checkEnd && model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan() > checkEnd)
                   )
                {
                    checkStart = model.DailyActivityHours[i].PartialStartTime.ResetTimeSpan();
                    checkEnd = model.DailyActivityHours[i].PartialEndTime.ResetTimeSpan();
                }
                else
                {
                    return Op.Failed($"بازه زمانی فعالیت مشاع بدرستی ارسال نشده است، خطا در ردیف: {i + 1}");
                }
            }
        }
        if (model.DailyUnitReservationCount != null && model.DailyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو روزانه هر واحد بدرستی ارسال نشده است");
        }
        if (model.WeeklyUnitReservationCount != null && model.WeeklyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو هفتگی هر واحد بدرستی ارسال نشده است");
        }
        if (model.MonthlyUnitReservationCount != null && model.MonthlyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو ماهانه هر واحد بدرستی ارسال نشده است");
        }
        if (model.YearlyUnitReservationCount != null && model.YearlyUnitReservationCount <= 0)
        {
            return Op.Failed("محدودیت رزرو سالیانه هر واحد بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Complexes.AnyAsync(x => x.Id == model.ComplexId && x.IsDelete != true, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه ساختمان بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.JointStatuses.AnyAsync(x => x.JointStatusId == model.JointStatusId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه وضعیت مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (await _context.Joints.AnyAsync(x => x.Title.ToLower() == model.Title.MultipleSpaceRemoverTrim().ToLower() && x.ComplexId == model.ComplexId, cancellationToken: cancellationToken))
            {
                return Op.Failed("ثبت مشاع با عنوان تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
            }
            Joint data = new()
            {
                ComplexId = model.ComplexId,
                Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                JointStatusId = model.JointStatusId,
                Location = model.Location.IsNotNull() ? model.Location.MultipleSpaceRemoverTrim() : null,
                PhoneNumbers = model.PhoneNumbers != null && model.PhoneNumbers.Any() ? string.Join(",", model.PhoneNumbers) : null,
                TermsFileUrl = model.TermsFileUrl.IsNotNull() ? model.TermsFileUrl : null,
                TermsText = model.TermsText.IsNotNull() ? model.TermsText.MultipleSpaceRemoverTrim() : null,
                ThumbnailUrl = model.MultiMedias == null || !model.MultiMedias.Any() ? null : model.MultiMedias.Any(x => x.IsThumbnail == true) ? model.MultiMedias.FirstOrDefault(x => x.IsThumbnail == true).Url : model.MultiMedias.FirstOrDefault().Url,
                Title = model.Title.MultipleSpaceRemoverTrim(),
                DailyUnitReservationCount = model.DailyUnitReservationCount,
                WeeklyUnitReservationCount = model.WeeklyUnitReservationCount,
                MonthlyUnitReservationCount = model.MonthlyUnitReservationCount,
                YearlyUnitReservationCount = model.YearlyUnitReservationCount,
            };
            await _context.Joints.AddAsync(data, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            if (model.MultiMedias != null && model.MultiMedias.Any())
            {
                await _context.JointMultiMedias.AddRangeAsync(model.MultiMedias.Select(x => new domain.entity.ReservationModels.JointModels.JointMultiMedia
                {
                    Alt = x.Alt.IsNotNull() ? x.Alt.MultipleSpaceRemoverTrim() : data.Title,
                    MediaType = x.MediaType == null ? MediaType.IMAGE : (MediaType)x.MediaType,
                    Url = x.Url,
                    JointId = data.JointId
                }).ToList(), cancellationToken);
            }
            await _context.JointDailyActivityHours.AddRangeAsync(model.DailyActivityHours.Select(x => new domain.entity.ReservationModels.JointModels.JointDailyActivityHour
            {
                PartialStartTime = x.PartialStartTime.ResetTimeSpan(),
                PartialEndTime = x.PartialEndTime.ResetTimeSpan(),
                JointId = data.JointId
            }).ToList(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت مشاع با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت مشاع با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<Response_GetComplexDTO>> GetComplexes(CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetComplexDTO> Op = new("GetComplexes");
        try
        {
            var data = (from complex in await _context.Complexes.ToListAsync(cancellationToken: cancellationToken)
                        where complex.IsDelete != true
                        select new Response_GetComplexDTO
                        {
                            Address = complex.Address,
                            Description = complex.Description,
                            Directions = complex.Directions,
                            Id = complex.Id,
                            Positions = complex.Positions,
                            Title = complex.Title,
                            Usages = complex.Usages,
                        }).ToList();
            if (data == null || !data.Any())
            {
                return Op.Succeed("دریافت لیست ساختمان ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            return Op.Succeed("دریافت لیست ساختمان ها با موفقیت انجام شد", data);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست ساختمان ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetJointDTO>> GetJoints(Filter_GetJointDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetJointDTO> Op = new("GetJoints");
        try
        {
            var data = (from joint in await _context.Joints.Include(x => x.JointDailyActivityHours).Include(x => x.JointMultiMedias).ToListAsync(cancellationToken: cancellationToken)
                        join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                        on joint.JointStatusId equals status.JointStatusId
                        where filter == null || !filter.Search.IsNotNull() || joint.Title.ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || (joint.Location.IsNotNull() ? joint.Location : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || (joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower())
                        select new Response_GetJointDTO
                        {
                            ComplexId = joint.ComplexId,
                            DailyActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_GetJointDailyActivityHourDTO
                            {
                                JointDailyActivityHourId = x.JointDailyActivityHourId,
                                PartialEndTime = x.PartialEndTime,
                                PartialStartTime = x.PartialStartTime
                            }).OrderBy(x => x.PartialStartTime).ToList(),
                            Description = joint.Description,
                            JointId = joint.JointId,
                            JointStatusDisplayTitle = status.DisplayTitle,
                            JointStatusId = joint.JointStatusId,
                            JointStatusTitle = status.Title,
                            Location = joint.Location,
                            MultiMedias = joint.JointMultiMedias != null && joint.JointMultiMedias.Any() ? joint.JointMultiMedias.Select(x => new Middle_GetJointMultiMediaDTO
                            {
                                Alt = x.Alt,
                                JointMultiMediaId = x.JointMultiMediaId,
                                MediaType = x.MediaType,
                                Url = x.Url,
                            }).ToList() : null,
                            PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                            TermsFileUrl = joint.TermsFileUrl,
                            TermsText = joint.TermsText,
                            ThumbnailUrl = joint.ThumbnailUrl,
                            Title = joint.Title,
                            IsActive = status.Title.ToLower() == "active" ? true : false,
                            DailyUnitReservationCount = joint.DailyUnitReservationCount,
                            MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                            WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                            YearlyUnitReservationCount = joint.YearlyUnitReservationCount
                        }).ToList();
            if (data == null || !data.Any())
            {
                return Op.Succeed("دریافت لیست مشاعات با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            return Op.Succeed("دریافت لیست مشاعات با موفقیت انجام شد", data);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست مشاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetJointStatusDTO>> GetJointStatuses(CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetJointStatusDTO> Op = new("GetJointStatuses");
        try
        {
            var data = (from status in await _context.JointStatuses.ToListAsync(cancellationToken)
                        select new Response_GetJointStatusDTO
                        {
                            DisplayTitle = status.DisplayTitle,
                            JointStatusId = status.JointStatusId,
                            Title = status.Title,
                        }).ToList();

            if (data == null || !data.Any())
            {
                return Op.Succeed("دریافت لیست وضعیت مشاعات با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            return Op.Succeed("دریافت لیست وضعیت مشاعات با موفقیت انجام شد", data);

        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست وضعیت مشاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteJoint(Request_DeleteJointDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteJoint");
        if (model == null || model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        try
        {
            var joint = await _context.Joints.Include(x => x.JointSessions).FirstOrDefaultAsync(x => x.JointId == model.JointId, cancellationToken: cancellationToken);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (joint.JointSessions != null && joint.JointSessions.Any())
            {
                return Op.Failed("امکان حذف مشاع دارای سانس تعریف شده امکانپذیر نمیباشد");
            }
            _context.JointMultiMedias.RemoveRange(await _context.JointMultiMedias.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken));
            _context.JointDailyActivityHours.RemoveRange(await _context.JointDailyActivityHours.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken));
            _context.Joints.Remove(joint);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("حذف مشاع با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("حذف مشاع با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateClosureJointSession(int AdminId, Request_CreateClosureJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateClosureJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت سانس تعطیل وجود ندارد");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس تعطیل از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس تعطیل از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
        {
            return Op.Failed("امکان ثبت سانس تعطیل برای روزهای قبل از امروز امکانپذیر نمیباشد");
        }
        if ((model.StartTime != null && model.EndTime == null) || (model.StartTime == null && model.EndTime != null))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartTime != null && model.EndTime != null && (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan() || (Convert.ToDateTime(model.EndTime) - Convert.ToDateTime(model.StartTime)).TotalMinutes < 15))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }


        try
        {
            var joint = (from j in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                         join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                         on j.JointStatusId equals status.JointStatusId
                         where status.Title.ToUpper() == "ACTIVE"
                         select j).FirstOrDefault(x => x.JointId == model.JointId);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }

            var todaySessions = await _context.JointSessions.Where(x => x.JointId == joint.JointId && x.SessionDate == model.SessionDate.ResetTime()).ToListAsync(cancellationToken: cancellationToken);
            if (todaySessions.Any())
            {
                if (todaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان ثبت سانس تعطیل در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                if (model.StartTime == null || model.EndTime == null)
                {
                    return Op.Failed("تعریف زمان شروع و پایان سانس تعطیل در صورت وجود سانس در روز اجباری است");
                }
                for (int i = 0; i < todaySessions.Count; i++)
                {
                    if (todaySessions[i].StartTime != null && todaySessions[i].EndTime != null)
                    {
                        if (
                            !(
                                (model.StartTime.ResetTimeSpan() >= todaySessions[i].EndTime && model.EndTime.ResetTimeSpan() > todaySessions[i].EndTime) ||
                                (model.StartTime.ResetTimeSpan() < todaySessions[i].StartTime && model.EndTime.ResetTimeSpan() <= todaySessions[i].StartTime)
                            )
                          )
                        {
                            return Op.Failed("تعریف سانس تعطیل با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("ثبت سانس تعطیل با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (model.StartTime != null && model.EndTime != null && !joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("ثبت سانس تعطیل با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }

            await _context.JointSessions.AddAsync(new domain.entity.ReservationModels.ReservationModels.JointSession
            {
                CreationDate = Op.OperationDate,
                Creator = AdminId,
                JointId = joint.JointId,
                Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                EndTime = model.EndTime != null ? model.EndTime.ResetTimeSpan() : null,
                IsClosure = true,
                SessionDate = model.SessionDate.ResetTime(),
                StartTime = model.StartTime != null ? model.StartTime.ResetTimeSpan() : null,
                Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,

            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت سانس تعطیل با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سانس تعطیل با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> UpdateClosureJointSession(int AdminId, Request_UpdateClosureJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdateClosureJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی سانس تعطیل ارسال نشده است");
        }
        if (model.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس تعطیل از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس تعطیل از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan())
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        try
        {
            var jointSession = await _context.JointSessions.Include(x => x.Joint).ThenInclude(x => x.JointDailyActivityHours).FirstOrDefaultAsync(x => x.IsClosure && x.JointSessionId == model.JointSessionId, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == jointSession.JointId && x.SessionDate == jointSession.SessionDate.ResetTime() && x.JointSessionId != model.JointSessionId).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions.Any())
            {
                if (onDaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان بروزرسانی سانس تعطیل در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (onDaySessions[i].StartTime != null && onDaySessions[i].EndTime != null)
                    {
                        if (
                            (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime.ResetTimeSpan()) ||
                            (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime.ResetTimeSpan())
                          )
                        {
                            continue;
                        }
                        else
                        {
                            return Op.Failed("بروزرسانی سانس تعطیل با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("بروزرسانی سانس تعطیل خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("بروزرسانی سانس تعطیل خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            jointSession.Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null;
            jointSession.Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null;
            jointSession.StartTime = model.StartTime.ResetTimeSpan();
            jointSession.EndTime = model.EndTime.ResetTimeSpan();
            jointSession.LastModifier = AdminId;
            jointSession.LastModificationDate = Op.OperationDate;

            _context.Entry<JointSession>(jointSession).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("بروزرسانی سانس تعطیل با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی سانس تعطیل با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> CreatePrivateJointSession(int AdminId, Request_CreatePrivateJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreatePrivateJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت سانس خصوصی وجود ندارد");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
        {
            return Op.Failed("امکان ثبت سانس برای روزهای قبل از امروز امکانپذیر نمیباشد");
        }
        if ((model.StartTime != null && model.EndTime == null) || (model.StartTime == null && model.EndTime != null))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartTime != null && model.EndTime != null && (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan() || (Convert.ToDateTime(model.EndTime) - Convert.ToDateTime(model.StartTime)).TotalMinutes < 15))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartReservationDate != null && model.EndReservationDate != null && model.StartReservationDate > model.EndReservationDate)
        {
            return Op.Failed("تاریخ و زمان مجاز برای ثبت رزرو بدرستی وارد نشده است");
        }
        if (model.StartReservationDate != null)
        {
            if (model.StartReservationDate.ResetTime() > model.SessionDate.ResetTime())
            {
                return Op.Failed("تاریخ شروع مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
            }
            if (model.StartTime != null && model.StartReservationDate.ResetTime() == model.SessionDate.ResetTime() && model.StartReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
            {
                return Op.Failed("ساعت شروع مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
            }
        }
        if (model.EndReservationDate != null)
        {
            if (model.EndReservationDate.ResetTime() > model.SessionDate.ResetTime())
            {
                return Op.Failed("تاریخ پایان مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
            }
            if (model.StartTime != null && model.EndReservationDate.ResetTime() == model.SessionDate.ResetTime() && model.EndReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
            {
                return Op.Failed("ساعت پایان مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
            }
        }
        if (model.MinimumReservationMinutes != null)
        {
            if (model.MinimumReservationMinutes <= 0)
            {
                return Op.Failed("حداقل زمان مجاز برای رزرو بدرستی ارسال نشده است");
            }
            if (model.MinimumReservationMinutes < 5)
            {
                return Op.Failed("حداقل زمان مجاز برای رزرو میتواند پنج دقیقه باشد");
            }
        }
        if (model.MaximumReservationMinutes != null)
        {
            if (model.MaximumReservationMinutes <= 0)
            {
                return Op.Failed("حداکثر زمان مجاز برای رزرو بدرستی ارسال نشده است");
            }
            if (model.StartTime != null && model.EndTime != null && model.MaximumReservationMinutes > (Convert.ToDateTime(model.EndTime) - Convert.ToDateTime(model.StartTime)).TotalMinutes)
            {
                return Op.Failed("حداکثر زمان مجاز برای رزرو نمیتواند از زمان شروع تا پایان رزرو بیشتر باشد");
            }
        }
        if (model.SessionCost != null && model.SessionCost < 0)
        {
            return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
        }
        if (!model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد در صورت ثبت امکان رزرو اضافه امکانپذیر است");
        }
        if (model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null && model.UnitExtraReservationCost < 0)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد بدرستی ارسال نشده است");
        }
        if (model.Capacity != null && model.Capacity <= 0)
        {
            return Op.Failed("ظرفیت سانس بدرستی ارسال نشده است");
        }
        if (model.GuestCapacity != null && model.GuestCapacity <= 0)
        {
            return Op.Failed("ظرفیت مهمان برای سانس بدرستی ارسال نشده است");
        }
        if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any(x => x <= 0))
        {
            return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
        }
        try
        {
            var joint = (from j in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                         join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                         on j.JointStatusId equals status.JointStatusId
                         where status.Title.ToUpper() == "ACTIVE"
                         select j).FirstOrDefault(x => x.JointId == model.JointId);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == joint.JointId && x.SessionDate == model.SessionDate.ResetTime()).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions.Any())
            {
                if (onDaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان ثبت سانس خصوصی در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                if (model.StartTime == null || model.EndTime == null)
                {
                    return Op.Failed("تعریف زمان شروع و پایان سانس خصوصی در صورت وجود سانس در روز اجباری است");
                }
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (onDaySessions[i].StartTime != null && onDaySessions[i].EndTime != null)
                    {
                        if (
                            !(
                                (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime) ||
                                (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime)
                            )
                          )
                        {
                            return Op.Failed("تعریف سانس خصوصی با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("ثبت سانس خصوصی با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (model.StartTime != null && model.EndTime != null && !joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("ثبت سانس خصوصی با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var dbUnitIDs = await _context.Units.Select(x => x.Id).ToListAsync(cancellationToken: cancellationToken);
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!dbUnitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {i + 1}");
                    }
                }
            }
            await _context.JointSessions.AddAsync(new domain.entity.ReservationModels.ReservationModels.JointSession
            {
                Capacity = model.Capacity,
                CreationDate = Op.OperationDate,
                Creator = AdminId,
                Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                EndReservationDate = model.EndReservationDate,
                EndTime = model.EndTime != null ? model.EndTime.ResetTimeSpan() : null,
                GuestCapacity = model.GuestCapacity,
                IsClosure = false,
                IsPrivate = true,
                JointId = joint.JointId,
                MaximumReservationMinutes = model.MaximumReservationMinutes,
                MinimumReservationMinutes = model.MinimumReservationMinutes,
                SessionCost = model.SessionCost,
                SessionDate = model.SessionDate.ResetTime(),
                StartReservationDate = model.StartReservationDate,
                StartTime = model.StartTime != null ? model.StartTime.ResetTimeSpan() : null,
                Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation,
                UnitExtraReservationCost = model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null ? model.UnitExtraReservationCost : null,
            }, cancellationToken);
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
                var addedJointSession = await _context.JointSessions.FirstOrDefaultAsync(x => x.CreationDate == Op.OperationDate, cancellationToken: cancellationToken);
                if (addedJointSession == null)
                {
                    return Op.Failed("دریافت اطلاعات سانس ثبت شده با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new domain.entity.ReservationModels.ReservationModels.JointSessionAcceptableUnit
                {
                    JointSessionId = addedJointSession.JointSessionId,
                    UnitId = x
                }), cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت سانس خصوصی با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سانس خصوصی با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreatePublicJointSession(int AdminId, Request_CreatePublicJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreatePublicJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت سانس عمومی وجود ندارد");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
        {
            return Op.Failed("امکان ثبت سانس برای روزهای قبل از امروز امکانپذیر نمیباشد");
        }
        if ((model.StartTime != null && model.EndTime == null) || (model.StartTime == null && model.EndTime != null))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartTime != null && model.EndTime != null && (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan() || (Convert.ToDateTime(model.EndTime) - Convert.ToDateTime(model.StartTime)).TotalMinutes < 15))
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartReservationDate != null && model.EndReservationDate != null && model.StartReservationDate > model.EndReservationDate)
        {
            return Op.Failed("تاریخ و زمان مجاز برای ثبت رزرو بدرستی وارد نشده است");
        }
        if (model.StartReservationDate != null)
        {
            if (model.StartReservationDate.ResetTime() > model.SessionDate.ResetTime())
            {
                return Op.Failed("تاریخ شروع مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
            }
            if (model.StartTime != null && model.StartReservationDate.ResetTime() == model.SessionDate.ResetTime() && model.StartReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
            {
                return Op.Failed("ساعت شروع مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
            }
        }
        if (model.EndReservationDate != null)
        {
            if (model.EndReservationDate.ResetTime() > model.SessionDate.ResetTime())
            {
                return Op.Failed("تاریخ پایان مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
            }
            if (model.StartTime != null && model.EndReservationDate.ResetTime() == model.SessionDate.ResetTime() && model.EndReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
            {
                return Op.Failed("ساعت پایان مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
            }
        }
        if (model.MinimumReservationMinutes != null)
        {
            if (model.MinimumReservationMinutes <= 0)
            {
                return Op.Failed("حداقل زمان مجاز برای رزرو بدرستی ارسال نشده است");
            }
            if (model.MinimumReservationMinutes < 5)
            {
                return Op.Failed("حداقل زمان مجاز برای رزرو میتواند پنج دقیقه باشد");
            }
        }
        if (model.MaximumReservationMinutes != null)
        {
            if (model.MaximumReservationMinutes <= 0)
            {
                return Op.Failed("حداکثر زمان مجاز برای رزرو بدرستی ارسال نشده است");
            }
            if (model.StartTime != null && model.EndTime != null && model.MaximumReservationMinutes > (Convert.ToDateTime(model.EndTime) - Convert.ToDateTime(model.StartTime)).TotalMinutes)
            {
                return Op.Failed("حداکثر زمان مجاز برای رزرو نمیتواند از زمان شروع تا پایان رزرو بیشتر باشد");
            }
        }
        if (model.SessionCost != null && model.SessionCost < 0)
        {
            return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
        }
        if (!model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد در صورت ثبت امکان رزرو اضافه امکانپذیر است");
        }
        if (model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null && model.UnitExtraReservationCost < 0)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد بدرستی ارسال نشده است");
        }
        if (model.Capacity != null && model.Capacity <= 0)
        {
            return Op.Failed("ظرفیت سانس بدرستی ارسال نشده است");
        }
        if (model.GuestCapacity != null && model.GuestCapacity <= 0)
        {
            return Op.Failed("ظرفیت مهمان برای سانس بدرستی ارسال نشده است");
        }
        if (model.SessionGender != null && !new List<int>() { (int)GenderType.MALE, (int)GenderType.FAMALE, (int)GenderType.ALL }.Any(x => x == model.SessionGender))
        {
            return Op.Failed("جنسیت افراد سانس عمومی بدرستی ارسال نشده است");
        }
        if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any(x => x <= 0))
        {
            return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
        }
        try
        {
            var joint = (from j in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                         join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                         on j.JointStatusId equals status.JointStatusId
                         where status.Title.ToUpper() == "ACTIVE"
                         select j).FirstOrDefault(x => x.JointId == model.JointId);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == joint.JointId && x.SessionDate == model.SessionDate.ResetTime()).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions.Any())
            {
                if (onDaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان ثبت سانس در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                if (model.StartTime == null || model.EndTime == null)
                {
                    return Op.Failed("تعریف زمان شروع و پایان سانس در صورت وجود سانس در روز اجباری است");
                }
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (onDaySessions[i].StartTime != null && onDaySessions[i].EndTime != null)
                    {
                        if (
                            !(
                                (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime) ||
                                (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime)
                            )
                          )
                        {
                            return Op.Failed("تعریف سانس با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("ثبت سانس با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (model.StartTime != null && model.EndTime != null && !joint.JointDailyActivityHours.Any(x => x.PartialStartTime <= model.StartTime.ResetTimeSpan() && x.PartialEndTime >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("ثبت سانس با بازه زمانی مشخص خارج از بازه زمانی مجاز فعالیت های مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var dbUnitIDs = await _context.Units.Select(x => x.Id).ToListAsync(cancellationToken: cancellationToken);
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!dbUnitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {i + 1}");
                    }
                }
            }
            await _context.JointSessions.AddAsync(new domain.entity.ReservationModels.ReservationModels.JointSession
            {
                Capacity = model.Capacity,
                CreationDate = Op.OperationDate,
                Creator = AdminId,
                Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                EndReservationDate = model.EndReservationDate,
                EndTime = model.EndTime != null ? model.EndTime.ResetTimeSpan() : null,
                GuestCapacity = model.GuestCapacity,
                IsClosure = false,
                IsPrivate = false,
                PublicSessionGender = model.SessionGender == null ? GenderType.ALL : (GenderType)model.SessionGender,
                JointId = joint.JointId,
                MaximumReservationMinutes = model.MaximumReservationMinutes,
                MinimumReservationMinutes = model.MinimumReservationMinutes,
                SessionCost = model.SessionCost,
                SessionDate = model.SessionDate.ResetTime(),
                StartReservationDate = model.StartReservationDate,
                StartTime = model.StartTime != null ? model.StartTime.ResetTimeSpan() : null,
                Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation,
                UnitExtraReservationCost = model.UnitHasAccessForExtraReservation && model.UnitExtraReservationCost != null ? model.UnitExtraReservationCost : null,
            }, cancellationToken);
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
                var addedJointSession = await _context.JointSessions.FirstOrDefaultAsync(x => x.CreationDate == Op.OperationDate, cancellationToken: cancellationToken);
                if (addedJointSession == null)
                {
                    return Op.Failed("دریافت اطلاعات سانس ثبت شده با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new domain.entity.ReservationModels.ReservationModels.JointSessionAcceptableUnit
                {
                    JointSessionId = addedJointSession.JointSessionId,
                    UnitId = x
                }), cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت سانس عمومی با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سانس عمومی با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteJointSession(Request_DeleteJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteJointSession");
        if (model == null || model.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        try
        {
            var jointSession = await _context.JointSessions.Include(x => x.Reservations).FirstOrDefaultAsync(x => x.JointSessionId == model.JointSessionId, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (jointSession.Reservations != null && jointSession.Reservations.Any(x => !x.CancelledByAdmin && !x.CancelledByByUser))
            {
                return Op.Failed("حذف سانس رزرو شده مجاز نمیباشد");
            }
            _context.JointSessionAcceptableUnits.RemoveRange(await _context.JointSessionAcceptableUnits.Where(x => x.JointSessionId == model.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            _context.JointSessionCancellationHours.RemoveRange(await _context.JointSessionCancellationHours.Where(x => x.JointSessionId == model.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            _context.Reservations.RemoveRange(await _context.Reservations.Where(x => x.JointSessionId == model.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            _context.JointSessions.Remove(jointSession);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("حذف سانس با موفقیت انجام شد");

        }
        catch (Exception ex)
        {
            return Op.Failed("حذف سانس با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> UpdatePrivateJointSession(int AdminId, Request_UpdatePrivateJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdatePrivateJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی سانس خصوصی ارسال نشده است");
        }
        if (model.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan())
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartReservationDate != null && model.EndReservationDate != null && model.StartReservationDate > model.EndReservationDate)
        {
            return Op.Failed("تاریخ و زمان مجاز برای ثبت رزرو بدرستی وارد نشده است");
        }
        if (model.SessionCost != null && model.SessionCost < 0)
        {
            return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
        }
        if ((model.UnitHasAccessForExtraReservation == null || model.UnitHasAccessForExtraReservation == false) && model.UnitExtraReservationCost != null)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد در صورت ثبت امکان رزرو اضافه امکانپذیر است");
        }
        if (model.UnitHasAccessForExtraReservation == true && model.UnitExtraReservationCost != null && model.UnitExtraReservationCost < 0)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد بدرستی ارسال نشده است");
        }
        if (model.Capacity != null && model.Capacity < 0)
        {
            return Op.Failed("ظرفیت سانس بدرستی ارسال نشده است");
        }
        if (model.GuestCapacity != null && model.GuestCapacity < 0)
        {
            return Op.Failed("ظرفیت مهمان برای سانس بدرستی ارسال نشده است");
        }
        if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any(x => x <= 0))
        {
            return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
        }
        if (model.CancellationTo != null)
        {
            if (!model.CancellationTo.Type.IsNotNull())
            {
                return Op.Failed("ارسال نوع زمان مجاز برای لغو رزرو اجباری است");
            }
            if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.CancellationTo.Type.MultipleSpaceRemoverTrim().ToLower()))
            {
                return Op.Failed("نوع زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
            }
            if (model.CancellationTo.Value < 0)
            {
                return Op.Failed("زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
            }
        }
        try
        {
            var jointSession = await _context.JointSessions.Include(x => x.Reservations).Include(x => x.Joint).ThenInclude(x => x.JointDailyActivityHours).FirstOrDefaultAsync(x => x.JointSessionId == model.JointSessionId && x.IsPrivate, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (jointSession.SessionDate.ResetTime() <= Op.OperationDate.ResetTime())
            {
                return Op.Failed("بروزرسانی سانس خصوصی تنها تا روز قبل از سانس امکان پذیر است", HttpStatusCode.Forbidden);
            }
            if (jointSession.Reservations != null && jointSession.Reservations.Any())
            {
                return Op.Failed("بروزرسانی سانس رزرو شده امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
            }
            if (model.StartReservationDate != null)
            {
                if (model.StartReservationDate.ResetTime() > jointSession.SessionDate.ResetTime())
                {
                    return Op.Failed("تاریخ شروع مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
                }

                if (model.StartReservationDate.ResetTime() == jointSession.SessionDate.ResetTime() && model.StartReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
                {
                    return Op.Failed("ساعت شروع مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
                }
            }
            if (model.EndReservationDate != null)
            {
                if (model.EndReservationDate.ResetTime() > jointSession.SessionDate.ResetTime())
                {
                    return Op.Failed("تاریخ پایان مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
                }
                if (model.EndReservationDate.ResetTime() == jointSession.SessionDate.ResetTime() && model.EndReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
                {
                    return Op.Failed("ساعت پایان مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
                }
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == jointSession.JointId && x.SessionDate == jointSession.SessionDate.ResetTime() && x.JointSessionId != model.JointSessionId).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions.Any())
            {
                if (onDaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان بروزرسانی سانس خصوصی در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (onDaySessions[i].StartTime != null && onDaySessions[i].EndTime != null)
                    {
                        if (
                            (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime.ResetTimeSpan()) ||
                            (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime.ResetTimeSpan())
                          )
                        {
                            continue;
                        }
                        else
                        {
                            return Op.Failed("بروزرسانی سانس خصوصی با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("بروزرسانی سانس خصوصی خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("بروزرسانی سانس خصوصی خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var dbUnitIDs = await _context.Units.Select(x => x.Id).ToListAsync(cancellationToken: cancellationToken);
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!dbUnitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {i + 1}");
                    }
                }
            }

            jointSession.Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null;
            jointSession.Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null;
            jointSession.StartTime = model.StartTime.ResetTimeSpan();
            jointSession.EndTime = model.EndTime.ResetTimeSpan();
            jointSession.StartReservationDate = model.StartReservationDate;
            jointSession.EndReservationDate = model.EndReservationDate;
            jointSession.SessionCost = model.SessionCost;
            jointSession.UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation != null && model.UnitHasAccessForExtraReservation != false;
            jointSession.UnitExtraReservationCost = model.UnitHasAccessForExtraReservation == true && model.UnitExtraReservationCost != null ? model.UnitExtraReservationCost : null;
            jointSession.Capacity = model.Capacity != null && model.Capacity > 0 ? model.Capacity : 0;
            jointSession.GuestCapacity = model.GuestCapacity != null && model.GuestCapacity > 0 ? model.GuestCapacity : 0;

            if (model.CancellationTo != null)
            {
                var cancellationData = await _context.JointSessionCancellationHours.FirstOrDefaultAsync(x => x.JointSessionId == jointSession.JointSessionId, cancellationToken: cancellationToken);
                if (cancellationData == null)
                {
                    await _context.JointSessionCancellationHours.AddAsync(new JointSessionCancellationHour
                    {
                        Hour = model.CancellationTo.Type switch
                        {
                            "monthly" => model.CancellationTo.Value * 30 * 24,
                            "weekly" => model.CancellationTo.Value * 7 * 24,
                            "daily" => model.CancellationTo.Value * 24,
                            "hourly" => model.CancellationTo.Value,
                            _ => model.CancellationTo.Value
                        },
                        JointSessionId = jointSession.JointSessionId
                    }, cancellationToken);
                }
                else
                {
                    cancellationData.Hour = model.CancellationTo.Type switch
                    {
                        "monthly" => model.CancellationTo.Value * 30 * 24,
                        "weekly" => model.CancellationTo.Value * 7 * 24,
                        "daily" => model.CancellationTo.Value * 24,
                        "hourly" => model.CancellationTo.Value,
                        _ => model.CancellationTo.Value
                    };
                    _context.Entry<JointSessionCancellationHour>(cancellationData).State = EntityState.Modified;
                }
            }
            else
            {
                _context.JointSessionCancellationHours.RemoveRange(await _context.JointSessionCancellationHours.Where(x => x.JointSessionId == jointSession.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            }

            _context.JointSessionAcceptableUnits.RemoveRange(await _context.JointSessionAcceptableUnits.Where(x => x.JointSessionId == jointSession.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new JointSessionAcceptableUnit
                {
                    JointSessionId = jointSession.JointSessionId,
                    UnitId = x
                }).ToList(), cancellationToken);
            }
            jointSession.LastModifier = AdminId;
            jointSession.LastModificationDate = Op.OperationDate;
            _context.Entry<JointSession>(jointSession).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("بروزرسانی سانس خصوصی با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی سانس خصوصی با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> UpdatePublicJointSession(int AdminId, Request_UpdatePublicJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdatePublicJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای بروزرسانی سانس ارسال نشده است");
        }
        if (model.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        if (model.Title.IsNotNull() && (model.Title.MultipleSpaceRemoverTrim().Length < 2 || model.Title.MultipleSpaceRemoverTrim().Length > 90))
        {
            return Op.Failed($"تعداد کاراکتر مجاز عنوان سانس از {2.ToPersianNumber()} تا {90.ToPersianNumber()} کاراکتر است");
        }
        if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 5 || model.Description.MultipleSpaceRemoverTrim().Length > 1800))
        {
            return Op.Failed($"تعداد کاراکتر مجاز توضیحات سانس از {5.ToPersianNumber()} تا {1800.ToPersianNumber()} کاراکتر است");
        }
        if (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan())
        {
            return Op.Failed("زمان شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.StartReservationDate != null && model.EndReservationDate != null && model.StartReservationDate > model.EndReservationDate)
        {
            return Op.Failed("تاریخ و زمان مجاز برای ثبت رزرو بدرستی وارد نشده است");
        }
        if (model.SessionGender != null && !new List<int>() { (int)GenderType.MALE, (int)GenderType.FAMALE, (int)GenderType.ALL }.Any(x => x == model.SessionGender))
        {
            return Op.Failed("جنسیت مجاز برای سانس بدرستی ارسال نشده است");
        }
        if (model.SessionCost != null && model.SessionCost < 0)
        {
            return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
        }
        if ((model.UnitHasAccessForExtraReservation == null || model.UnitHasAccessForExtraReservation == false) && model.UnitExtraReservationCost != null)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد در صورت ثبت امکان رزرو اضافه امکانپذیر است");
        }
        if (model.UnitHasAccessForExtraReservation == true && model.UnitExtraReservationCost != null && model.UnitExtraReservationCost < 0)
        {
            return Op.Failed("هزینه رزرو اضافه برای هر واحد بدرستی ارسال نشده است");
        }
        if (model.Capacity != null && model.Capacity < 0)
        {
            return Op.Failed("ظرفیت سانس بدرستی ارسال نشده است");
        }
        if (model.GuestCapacity != null && model.GuestCapacity < 0)
        {
            return Op.Failed("ظرفیت مهمان برای سانس بدرستی ارسال نشده است");
        }
        if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any(x => x <= 0))
        {
            return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
        }
        if (model.CancellationTo != null)
        {
            if (!model.CancellationTo.Type.IsNotNull())
            {
                return Op.Failed("ارسال نوع زمان مجاز برای لغو رزرو اجباری است");
            }
            if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.CancellationTo.Type.MultipleSpaceRemoverTrim().ToLower()))
            {
                return Op.Failed("نوع زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
            }
            if (model.CancellationTo.Value < 0)
            {
                return Op.Failed("زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
            }
        }
        try
        {
            var jointSession = await _context.JointSessions.Include(x => x.Reservations).Include(x => x.Joint).ThenInclude(x => x.JointDailyActivityHours).FirstOrDefaultAsync(x => x.JointSessionId == model.JointSessionId && !x.IsPrivate, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (jointSession.SessionDate.ResetTime() <= Op.OperationDate.ResetTime())
            {
                return Op.Failed("بروزرسانی سانس تنها تا روز قبل از سانس امکان پذیر است", HttpStatusCode.Forbidden);
            }
            if (jointSession.Reservations != null && jointSession.Reservations.Any())
            {
                return Op.Failed("بروزرسانی سانس رزرو شده امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
            }
            if (model.StartReservationDate != null)
            {
                if (model.StartReservationDate.ResetTime() > jointSession.SessionDate.ResetTime())
                {
                    return Op.Failed("تاریخ شروع مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
                }

                if (model.StartReservationDate.ResetTime() == jointSession.SessionDate.ResetTime() && model.StartReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
                {
                    return Op.Failed("ساعت شروع مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
                }
            }
            if (model.EndReservationDate != null)
            {
                if (model.EndReservationDate.ResetTime() > jointSession.SessionDate.ResetTime())
                {
                    return Op.Failed("تاریخ پایان مجاز برای ثبت رزرو نباید از تاریخ سانس بزگتر باشد");
                }
                if (model.EndReservationDate.ResetTime() == jointSession.SessionDate.ResetTime() && model.EndReservationDate.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
                {
                    return Op.Failed("ساعت پایان مجاز برای ثبت رزرو نباید از شروع سانس بزگتر باشد");
                }
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == jointSession.JointId && x.SessionDate == jointSession.SessionDate.ResetTime() && x.JointSessionId != model.JointSessionId).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions.Any())
            {
                if (onDaySessions.Any(x => x.StartTime == null && x.EndTime == null))
                {
                    return Op.Failed("امکان بروزرسانی سانس عمومی در صورت وجود سانس کامل تعریف شده وجود ندارد", HttpStatusCode.Conflict);
                }
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (onDaySessions[i].StartTime != null && onDaySessions[i].EndTime != null)
                    {
                        if (
                            (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime.ResetTimeSpan()) ||
                            (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime.ResetTimeSpan())
                          )
                        {
                            continue;
                        }
                        else
                        {
                            return Op.Failed("بروزرسانی سانس عمومی با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                        }
                    }
                }
                if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
                {
                    return Op.Failed("بروزرسانی سانس عمومی خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
            }
            else if (!jointSession.Joint.JointDailyActivityHours.Any(x => x.PartialStartTime.ResetTimeSpan() <= model.StartTime.ResetTimeSpan() && x.PartialEndTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan()))
            {
                return Op.Failed("بروزرسانی سانس عمومی خارج از بازه های زمانی فعالیت مشاع امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var dbUnitIDs = await _context.Units.Select(x => x.Id).ToListAsync(cancellationToken: cancellationToken);
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!dbUnitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد مجاز برای ثبت رزرو بدرستی ارسال نشده است، خطا در ردیف : {i + 1}");
                    }
                }
            }

            jointSession.Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null;
            jointSession.Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null;
            jointSession.StartTime = model.StartTime.ResetTimeSpan();
            jointSession.EndTime = model.EndTime.ResetTimeSpan();
            jointSession.StartReservationDate = model.StartReservationDate;
            jointSession.EndReservationDate = model.EndReservationDate;
            jointSession.SessionCost = model.SessionCost;
            jointSession.UnitHasAccessForExtraReservation = model.UnitHasAccessForExtraReservation != null && model.UnitHasAccessForExtraReservation != false;
            jointSession.UnitExtraReservationCost = model.UnitHasAccessForExtraReservation == true && model.UnitExtraReservationCost != null ? model.UnitExtraReservationCost : null;
            jointSession.Capacity = model.Capacity != null && model.Capacity > 0 ? model.Capacity : 0;
            jointSession.GuestCapacity = model.GuestCapacity != null && model.GuestCapacity > 0 ? model.GuestCapacity : 0;
            jointSession.PublicSessionGender = model.SessionGender == null ? GenderType.ALL : (GenderType)model.SessionGender;

            if (model.CancellationTo != null)
            {
                var cancellationData = await _context.JointSessionCancellationHours.FirstOrDefaultAsync(x => x.JointSessionId == jointSession.JointSessionId, cancellationToken: cancellationToken);
                if (cancellationData == null)
                {
                    await _context.JointSessionCancellationHours.AddAsync(new JointSessionCancellationHour
                    {
                        Hour = model.CancellationTo.Type switch
                        {
                            "monthly" => model.CancellationTo.Value * 30 * 24,
                            "weekly" => model.CancellationTo.Value * 7 * 24,
                            "daily" => model.CancellationTo.Value * 24,
                            "hourly" => model.CancellationTo.Value,
                            _ => model.CancellationTo.Value
                        },
                        JointSessionId = jointSession.JointSessionId
                    }, cancellationToken);
                }
                else
                {
                    cancellationData.Hour = model.CancellationTo.Type switch
                    {
                        "monthly" => model.CancellationTo.Value * 30 * 24,
                        "weekly" => model.CancellationTo.Value * 7 * 24,
                        "daily" => model.CancellationTo.Value * 24,
                        "hourly" => model.CancellationTo.Value,
                        _ => model.CancellationTo.Value
                    };
                    _context.Entry<JointSessionCancellationHour>(cancellationData).State = EntityState.Modified;
                }
            }
            else
            {
                _context.JointSessionCancellationHours.RemoveRange(await _context.JointSessionCancellationHours.Where(x => x.JointSessionId == jointSession.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            }

            _context.JointSessionAcceptableUnits.RemoveRange(await _context.JointSessionAcceptableUnits.Where(x => x.JointSessionId == jointSession.JointSessionId).ToListAsync(cancellationToken: cancellationToken));
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new JointSessionAcceptableUnit
                {
                    JointSessionId = jointSession.JointSessionId,
                    UnitId = x
                }).ToList(), cancellationToken);
            }
            jointSession.LastModifier = AdminId;
            jointSession.LastModificationDate = Op.OperationDate;
            _context.Entry<JointSession>(jointSession).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("بروزرسانی سانس با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("بروزرسانی سانس با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetJointSessionByAdminDTO>> GetJointSessionsByAdmin(Filter_GetJointSessionByAdminDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetJointSessionByAdminDTO> Op = new("GetJointSessionsByAdmin");
        if (filter != null)
        {
            if (filter.FromSessionDate != null && filter.ToSessionDate != null && filter.FromSessionDate.ResetTime() > filter.ToSessionDate.ResetTime())
            {
                return Op.Failed("فیلتر تاریخ شروع و پایان رزرو بدرستی ارسال نشده است");
            }
            if (filter.FromReservationDate != null && filter.ToReservationDate != null && filter.FromReservationDate.ResetTime() > filter.ToReservationDate.ResetTime())
            {
                return Op.Failed("فیلتر تاریخ شروع و پایان مجاز رزرو بدرستی ارسال نشده است");
            }
            if (filter.JointId != null && filter.JointId <= 0)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
            }
        }
        try
        {
            if (filter != null && filter.JointId != null && !await _context.Joints.AnyAsync(x => x.JointId == filter.JointId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            long totalCount = Convert.ToInt64((from session in await _context.JointSessions.ToListAsync(cancellationToken: cancellationToken)
                                               where
                                                  (filter == null || (!filter.Search.IsNotNull() || ((session.Title.IsNotNull() ? session.Title : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || (session.Description.IsNotNull() ? session.Description : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()))) &&
                                                      (filter.FromSessionDate == null || session.SessionDate.ResetTime() >= filter.FromSessionDate.ResetTime()) &&
                                                      (filter.ToSessionDate == null || session.SessionDate.ResetTime() <= filter.ToSessionDate.ResetTimeEnd()) &&
                                                      (filter.IsClosure == null || session.IsClosure == Convert.ToBoolean(filter.IsClosure)) &&
                                                      (filter.FromReservationDate == null || session.StartReservationDate != null && session.StartReservationDate.ResetTime() >= filter.FromReservationDate.ResetTime()) &&
                                                      (filter.ToReservationDate == null || session.EndReservationDate != null && session.EndReservationDate.ResetTime() <= filter.ToReservationDate.ResetTimeEnd()) &&
                                                      (filter.IsPrivate == null || session.IsPrivate == Convert.ToBoolean(filter.IsPrivate)) &&
                                                      (filter.JointId == null || session.JointId == filter.JointId)
                                                  )
                                               select session.JointSessionId).ToList().Count);
            if (totalCount == 0)
            {
                return Op.Succeed("دریافت لیست سانس ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد");
            }
            List<Middle_GetJointSessionDTO> result = (from session in await _context.JointSessions.Include(x => x.JointSessionCancellationHours).Include(x => x.Joint).ThenInclude(x => x.JointDailyActivityHours).Include(x => x.AcceptableUnits).ThenInclude(x => x.Unit).Include(x => x.Reservations).ToListAsync(cancellationToken: cancellationToken)
                                                      where
                                                          (filter == null || (!filter.Search.IsNotNull() || ((session.Title.IsNotNull() ? session.Title : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || (session.Description.IsNotNull() ? session.Description : "").ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()))) &&
                                                              (filter.FromSessionDate == null || session.SessionDate.ResetTime() >= filter.FromSessionDate.ResetTime()) &&
                                                              (filter.ToSessionDate == null || session.SessionDate.ResetTime() <= filter.ToSessionDate.ResetTimeEnd()) &&
                                                              (filter.IsClosure == null || session.IsClosure == Convert.ToBoolean(filter.IsClosure)) &&
                                                              (filter.FromReservationDate == null || session.StartReservationDate != null && session.StartReservationDate.ResetTime() >= filter.FromReservationDate.ResetTime()) &&
                                                              (filter.ToReservationDate == null || session.EndReservationDate != null && session.EndReservationDate.ResetTime() <= filter.ToReservationDate.ResetTimeEnd()) &&
                                                              (filter.IsPrivate == null || session.IsPrivate == Convert.ToBoolean(filter.IsPrivate)) &&
                                                              (filter.JointId == null || session.JointId == filter.JointId)
                                                          )
                                                      select session).OrderByDescending(x => x.SessionDate.ResetTime()).ThenBy(x => x.StartTime).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).Select(session =>
                                                      {
                                                          var data = new Middle_GetJointSessionDTO
                                                          {
                                                              AcceptableUnits = session.AcceptableUnits != null && session.AcceptableUnits.Any() ? session.AcceptableUnits.Select(x => new Middle_AcceptableUnitDTO
                                                              {
                                                                  UnitId = x.UnitId,
                                                                  UnitName = x.Unit.Name
                                                              }).ToList() : null,
                                                              Capacity = session.Capacity,
                                                              CreationDate = session.CreationDate,
                                                              Creator = session.Creator,
                                                              Description = session.Description,
                                                              EndReservationDate = session.EndReservationDate?.ResetTime(),
                                                              EndTime = session.EndTime.ResetTimeSpan(),
                                                              GuestCapacity = session.GuestCapacity,
                                                              IsClosure = session.IsClosure,
                                                              IsPrivate = session.IsPrivate,
                                                              Joint = new Middle_JointDTO
                                                              {
                                                                  ActivityHours = session.Joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO
                                                                  {
                                                                      PartialEndTime = x.PartialEndTime.ResetTimeSpan(),
                                                                      PartialStartTime = x.PartialStartTime.ResetTimeSpan()
                                                                  }).ToList(),
                                                                  DailyUnitReservationCount = session.Joint.DailyUnitReservationCount,
                                                                  Description = session.Joint.Description,
                                                                  JointId = session.JointId,
                                                                  Location = session.Joint.Location,
                                                                  MonthlyUnitReservationCount = session.Joint.MonthlyUnitReservationCount,
                                                                  PhoneNumbers = session.Joint.PhoneNumbers.IsNotNull() ? session.Joint.PhoneNumbers.Split(",").ToList() : null,
                                                                  TermsFileUrl = session.Joint.TermsFileUrl,
                                                                  TermsText = session.Joint.TermsText,
                                                                  ThumbnailUrl = session.Joint.ThumbnailUrl,
                                                                  Title = session.Joint.Title,
                                                                  WeeklyUnitReservationCount = session.Joint.WeeklyUnitReservationCount,
                                                                  YearlyUnitReservationCount = session.Joint.YearlyUnitReservationCount
                                                              },
                                                              JointSessionId = session.JointSessionId,
                                                              LastModificationDate = session.LastModificationDate,
                                                              LastModifier = session.LastModifier,
                                                              MaximumReservationMinutes = session.MaximumReservationMinutes,
                                                              MinimumReservationMinutes = session.MinimumReservationMinutes,
                                                              PublicSessionGender = session.PublicSessionGender,
                                                              PublicSessionGenderDisplayTitle = session.PublicSessionGender == null ? null : session.PublicSessionGender switch
                                                              {
                                                                  null => "همگانی",
                                                                  GenderType.ALL => "همگانی",
                                                                  GenderType.MALE => "آقایان",
                                                                  GenderType.FAMALE => "بانوان",
                                                                  _ => "همگانی",
                                                              },
                                                              PublicSessionGenderTitle = session.PublicSessionGender == null ? null : session.PublicSessionGender switch
                                                              {
                                                                  null => null,
                                                                  GenderType.ALL => "All",
                                                                  GenderType.MALE => "Male",
                                                                  GenderType.FAMALE => "Female",
                                                                  _ => null,
                                                              },
                                                              SessionCost = session.SessionCost,
                                                              SessionDate = session.SessionDate.ResetTime(),
                                                              StartReservationDate = session.StartReservationDate?.ResetTime(),
                                                              StartTime = session.StartTime.ResetTimeSpan(),
                                                              Title = session.Title,
                                                              UnitExtraReservationCost = session.UnitExtraReservationCost,
                                                              UnitHasAccessForExtraReservation = session.UnitHasAccessForExtraReservation,
                                                              IsEditable = session.SessionDate.ResetTime() > Op.OperationDate.ResetTime() && (session.Reservations == null || !session.Reservations.Any()),
                                                              StartReservationTime = session.StartReservationDate?.ResetTimeSpan(),
                                                              EndReservationTime = session.EndReservationDate?.ResetTimeSpan(),
                                                              SessionDateDay = session.SessionDate.GetDayOfWeekTitle()
                                                          };
                                                          if (session.JointSessionCancellationHours != null && session.JointSessionCancellationHours.Any())
                                                          {
                                                              var (Key, Value) = session.JointSessionCancellationHours[0].Hour.GetTimeConcept();
                                                              data.CancellationTo = new Middle_CreateJointSessions_TimeConcept
                                                              {
                                                                  Type = Key,
                                                                  Value = Value
                                                              };
                                                          }
                                                          return data;
                                                      }).ToList();
            return Op.Succeed("دریافت لیست سانس ها با موفقیت انجام شد", new Response_GetJointSessionByAdminDTO
            {
                JointSessions = result,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست سانس ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<Response_GetJointDTO>> GetJoint(int JointId, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetJointDTO> Op = new("GetJoint");
        if (JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        try
        {
            var data = (from joint in await _context.Joints.Include(x => x.JointDailyActivityHours).Include(x => x.JointMultiMedias).ToListAsync(cancellationToken: cancellationToken)
                        join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                        on joint.JointStatusId equals status.JointStatusId
                        where joint.JointId == JointId
                        select new Response_GetJointDTO
                        {
                            ComplexId = joint.ComplexId,
                            DailyActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_GetJointDailyActivityHourDTO
                            {
                                JointDailyActivityHourId = x.JointDailyActivityHourId,
                                PartialEndTime = x.PartialEndTime,
                                PartialStartTime = x.PartialStartTime
                            }).OrderBy(x => x.PartialStartTime).ToList(),
                            Description = joint.Description,
                            JointId = joint.JointId,
                            JointStatusDisplayTitle = status.DisplayTitle,
                            JointStatusId = joint.JointStatusId,
                            JointStatusTitle = status.Title,
                            Location = joint.Location,
                            MultiMedias = joint.JointMultiMedias != null && joint.JointMultiMedias.Any() ? joint.JointMultiMedias.Select(x => new Middle_GetJointMultiMediaDTO
                            {
                                Alt = x.Alt,
                                JointMultiMediaId = x.JointMultiMediaId,
                                MediaType = x.MediaType,
                                Url = x.Url,
                            }).ToList() : null,
                            PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                            TermsFileUrl = joint.TermsFileUrl,
                            TermsText = joint.TermsText,
                            ThumbnailUrl = joint.ThumbnailUrl,
                            Title = joint.Title,
                            IsActive = status.Title.ToLower() == "active",
                            DailyUnitReservationCount = joint.DailyUnitReservationCount,
                            MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                            WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                            YearlyUnitReservationCount = joint.YearlyUnitReservationCount
                        }).FirstOrDefault();
            if (data == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            return Op.Succeed("دریافت اطلاعات مشاع با موفقیت انجام شد", data);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات مشاع با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<Response_GetUnitDTO>> GetUnits(CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetUnitDTO> Op = new("GetUnits");
        try
        {
            var data = (from unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                        where unit.IsDelete != true
                        orderby unit.Floor
                        select new Response_GetUnitDTO
                        {
                            Id = unit.Id,
                            Name = unit.Name,
                        }).ToList();
            if (data == null || !data.Any())
            {
                return Op.Succeed("دریافت لیست واحد ها با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            return Op.Succeed("دریافت لیست واحد ها با موفقیت انجام شد", data);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست واحد ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetJointSessionByCustomerDTO>> GetJointSessionsByCustomer(int CustomerId, int UnitId, int JointId, DateTime RequestDate, Filter_GetJointSessionByCustomerDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetJointSessionByCustomerDTO> Op = new("GetJointSessionsByCustomer");
        if (CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (UnitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        if (JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (RequestDate.ResetTime() < Op.OperationDate.ResetTime())
        {
            return Op.Failed("امکان رزرو برای سانس های قبل از امروز مجاز نمیباشد");
        }
        if (filter != null && filter.JointSessionId != null && filter.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Units.AnyAsync(x => x.Id == UnitId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Residents.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == UnitId && x.User.Id == CustomerId, cancellationToken: cancellationToken) &&
                !await _context.Owners.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == UnitId && x.User.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("واحد با کاربری ارسال شده مطابقت ندارد", HttpStatusCode.Conflict);
            }
            var joint = await _context.Joints.FirstOrDefaultAsync(x => x.JointId == JointId, cancellationToken: cancellationToken);
            if (joint == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            joint.JointStatus = await _context.JointStatuses.FirstOrDefaultAsync(x => x.JointStatusId == joint.JointStatusId, cancellationToken: cancellationToken);
            joint.JointDailyActivityHours = await _context.JointDailyActivityHours.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken);
            joint.JointMultiMedias = await _context.JointMultiMedias.Where(x => x.JointId == joint.JointId).ToListAsync(cancellationToken: cancellationToken);
            if (joint.JointStatus.Title.ToLower() != "active")
            {
                return Op.Failed("امکان دریافت لیست سانس ها در صورت فعال نبودن مشاع وجود ندارد", HttpStatusCode.Forbidden);
            }
            var previousReservedOperation = await _GetUnitsReservedCounts(JointId, RequestDate, UnitId, cancellationToken);
            if (!previousReservedOperation.Success)
            {
                return Op.Failed(previousReservedOperation.Message, previousReservedOperation.ExMessage, previousReservedOperation.Status);
            }

            var middleJointModel = new Middle_JointDTO
            {
                ActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO { PartialEndTime = x.PartialEndTime, PartialStartTime = x.PartialStartTime }).ToList(),
                DailyUnitReservationCount = joint.DailyUnitReservationCount,
                Description = joint.Description,
                JointId = joint.JointId,
                Location = joint.Location,
                MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                TermsFileUrl = joint.TermsFileUrl,
                TermsText = joint.TermsText,
                ThumbnailUrl = joint.ThumbnailUrl,
                Title = joint.Title,
                WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                YearlyUnitReservationCount = joint.YearlyUnitReservationCount,
            };

            List<Middle_GetJointSessionsByCustomerDTO> todayJointSessions = (from session in await _context.JointSessions.Include(x => x.AcceptableUnits).Include(x => x.Reservations).ToListAsync(cancellationToken: cancellationToken)
                                                                             where (filter == null || filter.JointSessionId == null || session.JointSessionId == filter.JointSessionId) && session.JointId == JointId && session.SessionDate.ResetTime() == RequestDate.ResetTime()
                                                                             select session).ToList().Select(x =>
                                                                             {
                                                                                 Middle_GetJointSessionsByCustomerDTO result = new();
                                                                                 if (x.IsClosure)
                                                                                 {
                                                                                     result = new Middle_GetJointSessionsByCustomerDTO
                                                                                     {
                                                                                         Capacity = null,
                                                                                         Description = x.Description,
                                                                                         EndReservationDate = null,
                                                                                         EndReservationTime = null,
                                                                                         EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                         ForbiddenText = "سانس تعطیل",
                                                                                         GuestCapacity = null,
                                                                                         IsAvailable = false,
                                                                                         IsClosure = true,
                                                                                         IsPrivate = false,
                                                                                         Joint = middleJointModel,
                                                                                         JointSessionId = x.JointSessionId,
                                                                                         MaximumReservationMinutes = null,
                                                                                         MinimumReservationMinutes = null,
                                                                                         PublicSessionGender = null,
                                                                                         SessionCost = null,
                                                                                         SessionDate = x.SessionDate,
                                                                                         StartReservationDate = null,
                                                                                         StartReservationTime = null,
                                                                                         StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                         Title = x.Title,
                                                                                         UnitExtraReservationCost = null,
                                                                                         UnitHasAccessForExtraReservation = false
                                                                                     };
                                                                                 }
                                                                                 else
                                                                                 {
                                                                                     if (x.AcceptableUnits != null && x.AcceptableUnits.Any())
                                                                                     {
                                                                                         if (x.AcceptableUnits.Any(x => x.UnitId == UnitId))
                                                                                         {
                                                                                             if (x.Reservations != null && x.Reservations.Any())
                                                                                             {
                                                                                                 if (x.IsPrivate)
                                                                                                 {
                                                                                                     if (x.Reservations.Any(x => x.ReservedForUnitId == UnitId))
                                                                                                     {
                                                                                                         result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                         {
                                                                                                             Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                             Description = x.Description,
                                                                                                             EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                             EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                             EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                             GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                             IsClosure = false,
                                                                                                             IsPrivate = true,
                                                                                                             Joint = middleJointModel,
                                                                                                             JointSessionId = x.JointSessionId,
                                                                                                             MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                             MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                             PublicSessionGender = null,
                                                                                                             PublicSessionGenderTitle = null,
                                                                                                             PublicSessionGenderDisplay = null,
                                                                                                             SessionCost = x.SessionCost,
                                                                                                             SessionDate = x.SessionDate,
                                                                                                             StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                             StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                             StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                             Title = x.Title,
                                                                                                             UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                             UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                         };
                                                                                                         if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                     ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                      (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                      (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                      (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                            )
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                             var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                             var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                             if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                             {
                                                                                                                 if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                                 {
                                                                                                                     result.IsAvailable = true;
                                                                                                                 }
                                                                                                                 else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                                 {
                                                                                                                     result.IsAvailable = false;
                                                                                                                     result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                                 }
                                                                                                                 else
                                                                                                                 {
                                                                                                                     result.IsAvailable = true;
                                                                                                                 }
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 result.IsAvailable = false;
                                                                                                                 result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                             }

                                                                                                         }
                                                                                                     }
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                     {
                                                                                                         Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                         Description = x.Description,
                                                                                                         EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                         EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                         EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                         GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                         IsClosure = false,
                                                                                                         IsPrivate = false,
                                                                                                         Joint = middleJointModel,
                                                                                                         JointSessionId = x.JointSessionId,
                                                                                                         MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                         MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                         PublicSessionGender = x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                         PublicSessionGenderTitle = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                         {
                                                                                                             null => "All",
                                                                                                             GenderType.ALL => "All",
                                                                                                             GenderType.MALE => "Male",
                                                                                                             GenderType.FAMALE => "Female",
                                                                                                             _ => "All",
                                                                                                         } : "All",
                                                                                                         PublicSessionGenderDisplay = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                         {
                                                                                                             null => "همگانی",
                                                                                                             GenderType.ALL => "همگانی",
                                                                                                             GenderType.MALE => "آقایان",
                                                                                                             GenderType.FAMALE => "بانوان",
                                                                                                             _ => "همگانی",
                                                                                                         } : "همگانی",
                                                                                                         SessionCost = x.SessionCost,
                                                                                                         SessionDate = x.SessionDate,
                                                                                                         StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                         StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                         StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                         Title = x.Title,
                                                                                                         UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                         UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                     };
                                                                                                     if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                 ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                  (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                  (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                  (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                        )
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                         var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                         var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                         if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                         {
                                                                                                             if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                             else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                             {
                                                                                                                 result.IsAvailable = false;
                                                                                                                 result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                         }

                                                                                                     }
                                                                                                 }
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                 {
                                                                                                     Capacity = x.Capacity,
                                                                                                     Description = x.Description,
                                                                                                     EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                     EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                     EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                     GuestCapacity = x.GuestCapacity,
                                                                                                     IsClosure = false,
                                                                                                     IsPrivate = x.IsPrivate,
                                                                                                     Joint = middleJointModel,
                                                                                                     JointSessionId = x.JointSessionId,
                                                                                                     MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                     MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                     PublicSessionGender = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                     PublicSessionGenderTitle = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "All",
                                                                                                         GenderType.ALL => "All",
                                                                                                         GenderType.MALE => "Male",
                                                                                                         GenderType.FAMALE => "Female",
                                                                                                         _ => "All"
                                                                                                     } : "All",
                                                                                                     PublicSessionGenderDisplay = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "همگانی",
                                                                                                         GenderType.ALL => "همگانی",
                                                                                                         GenderType.MALE => "آقایان",
                                                                                                         GenderType.FAMALE => "بانوان",
                                                                                                         _ => "همگانی"
                                                                                                     } : "همگانی",
                                                                                                     SessionCost = x.SessionCost,
                                                                                                     SessionDate = x.SessionDate,
                                                                                                     StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                     StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                     StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                     Title = x.Title,
                                                                                                     UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                     UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                 };
                                                                                                 if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                 (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                 (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                 (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                       )
                                                                                                 {
                                                                                                     result.IsAvailable = false;
                                                                                                     result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                     var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                     var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                     if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                     {
                                                                                                         result.IsAvailable = true;
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                     }

                                                                                                 }
                                                                                             }
                                                                                         }
                                                                                     }
                                                                                     else
                                                                                     {
                                                                                         if (x.Reservations != null && x.Reservations.Any())
                                                                                         {
                                                                                             if (x.IsPrivate)
                                                                                             {
                                                                                                 if (x.Reservations.Any(x => x.ReservedForUnitId == UnitId))
                                                                                                 {
                                                                                                     result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                     {
                                                                                                         Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                         Description = x.Description,
                                                                                                         EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                         EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                         EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                         GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                         IsClosure = false,
                                                                                                         IsPrivate = true,
                                                                                                         Joint = middleJointModel,
                                                                                                         JointSessionId = x.JointSessionId,
                                                                                                         MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                         MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                         PublicSessionGender = null,
                                                                                                         PublicSessionGenderTitle = null,
                                                                                                         PublicSessionGenderDisplay = null,
                                                                                                         SessionCost = x.SessionCost,
                                                                                                         SessionDate = x.SessionDate,
                                                                                                         StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                         StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                         StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                         Title = x.Title,
                                                                                                         UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                         UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                     };
                                                                                                     if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                 ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                  (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                  (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                  (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                        )
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                         var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                         var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                         if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                         {
                                                                                                             if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                             else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                             {
                                                                                                                 result.IsAvailable = false;
                                                                                                                 result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                         }

                                                                                                     }
                                                                                                 }
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                 {
                                                                                                     Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                     Description = x.Description,
                                                                                                     EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                     EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                     EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                     GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                     IsClosure = false,
                                                                                                     IsPrivate = false,
                                                                                                     Joint = middleJointModel,
                                                                                                     JointSessionId = x.JointSessionId,
                                                                                                     MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                     MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                     PublicSessionGender = x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                     PublicSessionGenderTitle = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "All",
                                                                                                         GenderType.ALL => "All",
                                                                                                         GenderType.MALE => "Male",
                                                                                                         GenderType.FAMALE => "Female",
                                                                                                         _ => "All",
                                                                                                     } : "All",
                                                                                                     PublicSessionGenderDisplay = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "همگانی",
                                                                                                         GenderType.ALL => "همگانی",
                                                                                                         GenderType.MALE => "آقایان",
                                                                                                         GenderType.FAMALE => "بانوان",
                                                                                                         _ => "همگانی",
                                                                                                     } : "همگانی",
                                                                                                     SessionCost = x.SessionCost,
                                                                                                     SessionDate = x.SessionDate,
                                                                                                     StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                     StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                     StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                     Title = x.Title,
                                                                                                     UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                     UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                 };
                                                                                                 if (!x.UnitHasAccessForExtraReservation &&
                                                                                                             ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                              (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                              (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                              (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                    )
                                                                                                 {
                                                                                                     result.IsAvailable = false;
                                                                                                     result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                     var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                     var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                     if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                     {
                                                                                                         if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                         else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                     }

                                                                                                 }
                                                                                             }
                                                                                         }
                                                                                         else
                                                                                         {
                                                                                             result = new Middle_GetJointSessionsByCustomerDTO
                                                                                             {
                                                                                                 Capacity = x.Capacity,
                                                                                                 Description = x.Description,
                                                                                                 EndReservationDate = x.EndReservationDate != null ? x.EndReservationDate.ResetTime() : null,
                                                                                                 EndReservationTime = x.EndReservationDate != null ? x.EndReservationDate.ResetTimeSpan() : null,
                                                                                                 EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                 GuestCapacity = x.GuestCapacity,
                                                                                                 IsClosure = false,
                                                                                                 IsPrivate = x.IsPrivate,
                                                                                                 Joint = middleJointModel,
                                                                                                 JointSessionId = x.JointSessionId,
                                                                                                 MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                 MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                 PublicSessionGender = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                 PublicSessionGenderTitle = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                 {
                                                                                                     null => "All",
                                                                                                     GenderType.ALL => "All",
                                                                                                     GenderType.MALE => "Male",
                                                                                                     GenderType.FAMALE => "Female",
                                                                                                     _ => "All"
                                                                                                 } : "All",
                                                                                                 PublicSessionGenderDisplay = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                 {
                                                                                                     null => "همگانی",
                                                                                                     GenderType.ALL => "همگانی",
                                                                                                     GenderType.MALE => "آقایان",
                                                                                                     GenderType.FAMALE => "بانوان",
                                                                                                     _ => "همگانی"
                                                                                                 } : "همگانی",
                                                                                                 SessionCost = x.SessionCost,
                                                                                                 SessionDate = x.SessionDate,
                                                                                                 StartReservationDate = x.StartReservationDate != null ? x.StartReservationDate.ResetTime() : null,
                                                                                                 StartReservationTime = x.StartReservationDate != null ? x.StartReservationDate.ResetTimeSpan() : null,
                                                                                                 StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                 Title = x.Title,
                                                                                                 UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                 UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                             };
                                                                                             if (!x.UnitHasAccessForExtraReservation &&
                                                                                                            ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                             (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                             (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                             (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                   )
                                                                                             {
                                                                                                 result.IsAvailable = false;
                                                                                                 result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                 var startReservationDateTime = x.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(x.StartReservationDate, x.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
                                                                                                 var endReservationDateTime = x.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(x.EndReservationDate, x.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(x.SessionDate, x.StartTime != null ? x.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
                                                                                                 if (requestDateTime >= startReservationDateTime && requestDateTime <= endReservationDateTime)
                                                                                                 {
                                                                                                     result.IsAvailable = true;
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     result.IsAvailable = false;
                                                                                                     result.ForbiddenText = "زمان در بازه مجاز رزرو این سانس قرار ندارد";
                                                                                                 }

                                                                                             }
                                                                                         }
                                                                                     }
                                                                                 }
                                                                                 return result;
                                                                             }).ToList().Where(x => x.JointSessionId > 0).ToList();

            List<Middle_GetJointSessionsByCustomerDTO> availableJointSessions = (from session in await _context.JointSessions.Include(x => x.AcceptableUnits).Include(x => x.Reservations).ToListAsync(cancellationToken: cancellationToken)
                                                                                 where
                                                                                    (filter == null || filter.JointSessionId == null || session.JointSessionId == filter.JointSessionId) &&
                                                                                    session.JointId == JointId &&
                                                                                    session.SessionDate.ResetTime() > RequestDate.ResetTime() &&
                                                                                    !session.IsClosure &&
                                                                                    (DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan()) >= (session.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(session.StartReservationDate.ResetTime(), session.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan()))) &&
                                                                                    (DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan()) <= (session.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(session.EndReservationDate.ResetTime(), session.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(session.SessionDate.ResetTime(), session.StartTime != null ? session.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan())))
                                                                                 select session).ToList().Select(x =>
                                                                                 {
                                                                                     Middle_GetJointSessionsByCustomerDTO result = new();
                                                                                     if (x.AcceptableUnits != null && x.AcceptableUnits.Any())
                                                                                     {
                                                                                         if (x.AcceptableUnits.Any(x => x.UnitId == UnitId))
                                                                                         {
                                                                                             if (x.Reservations != null && x.Reservations.Any())
                                                                                             {
                                                                                                 if (x.IsPrivate)
                                                                                                 {
                                                                                                     if (x.Reservations.Any(x => x.ReservedForUnitId == UnitId))
                                                                                                     {
                                                                                                         result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                         {
                                                                                                             Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                             Description = x.Description,
                                                                                                             EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                             EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                             EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                             GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                             IsClosure = false,
                                                                                                             IsPrivate = true,
                                                                                                             Joint = middleJointModel,
                                                                                                             JointSessionId = x.JointSessionId,
                                                                                                             MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                             MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                             PublicSessionGender = null,
                                                                                                             PublicSessionGenderTitle = null,
                                                                                                             PublicSessionGenderDisplay = null,
                                                                                                             SessionCost = x.SessionCost,
                                                                                                             SessionDate = x.SessionDate,
                                                                                                             StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                             StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                             StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                             Title = x.Title,
                                                                                                             UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                             UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                         };
                                                                                                         if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                     ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                      (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                      (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                      (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                            )
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                             else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                             {
                                                                                                                 result.IsAvailable = false;
                                                                                                                 result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 result.IsAvailable = true;
                                                                                                             }
                                                                                                         }
                                                                                                     }
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                     {
                                                                                                         Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                         Description = x.Description,
                                                                                                         EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                         EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                         EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                         GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                         IsClosure = false,
                                                                                                         IsPrivate = false,
                                                                                                         Joint = middleJointModel,
                                                                                                         JointSessionId = x.JointSessionId,
                                                                                                         MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                         MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                         PublicSessionGender = x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                         PublicSessionGenderTitle = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                         {
                                                                                                             null => "All",
                                                                                                             GenderType.ALL => "All",
                                                                                                             GenderType.MALE => "Male",
                                                                                                             GenderType.FAMALE => "Female",
                                                                                                             _ => "All",
                                                                                                         } : "All",
                                                                                                         PublicSessionGenderDisplay = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                         {
                                                                                                             null => "همگانی",
                                                                                                             GenderType.ALL => "همگانی",
                                                                                                             GenderType.MALE => "آقایان",
                                                                                                             GenderType.FAMALE => "بانوان",
                                                                                                             _ => "همگانی",
                                                                                                         } : "همگانی",
                                                                                                         SessionCost = x.SessionCost,
                                                                                                         SessionDate = x.SessionDate,
                                                                                                         StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                         StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                         StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                         Title = x.Title,
                                                                                                         UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                         UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                     };
                                                                                                     if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                 ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                  (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                  (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                  (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                        )
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                         else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                     }
                                                                                                 }
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                 {
                                                                                                     Capacity = x.Capacity,
                                                                                                     Description = x.Description,
                                                                                                     EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                     EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                     EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                     GuestCapacity = x.GuestCapacity,
                                                                                                     IsClosure = false,
                                                                                                     IsPrivate = x.IsPrivate,
                                                                                                     Joint = middleJointModel,
                                                                                                     JointSessionId = x.JointSessionId,
                                                                                                     MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                     MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                     PublicSessionGender = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                     PublicSessionGenderTitle = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "All",
                                                                                                         GenderType.ALL => "All",
                                                                                                         GenderType.MALE => "Male",
                                                                                                         GenderType.FAMALE => "Female",
                                                                                                         _ => "All"
                                                                                                     } : "All",
                                                                                                     PublicSessionGenderDisplay = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "همگانی",
                                                                                                         GenderType.ALL => "همگانی",
                                                                                                         GenderType.MALE => "آقایان",
                                                                                                         GenderType.FAMALE => "بانوان",
                                                                                                         _ => "همگانی"
                                                                                                     } : "همگانی",
                                                                                                     SessionCost = x.SessionCost,
                                                                                                     SessionDate = x.SessionDate,
                                                                                                     StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                     StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                     StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                     Title = x.Title,
                                                                                                     UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                     UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                 };
                                                                                                 if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                 (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                 (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                 (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                       )
                                                                                                 {
                                                                                                     result.IsAvailable = false;
                                                                                                     result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     result.IsAvailable = true;
                                                                                                 }
                                                                                             }
                                                                                         }
                                                                                     }
                                                                                     else
                                                                                     {
                                                                                         if (x.Reservations != null && x.Reservations.Any())
                                                                                         {
                                                                                             if (x.IsPrivate)
                                                                                             {
                                                                                                 if (x.Reservations.Any(x => x.ReservedForUnitId == UnitId))
                                                                                                 {
                                                                                                     result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                     {
                                                                                                         Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                         Description = x.Description,
                                                                                                         EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                         EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                         EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                         GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                         IsClosure = false,
                                                                                                         IsPrivate = true,
                                                                                                         Joint = middleJointModel,
                                                                                                         JointSessionId = x.JointSessionId,
                                                                                                         MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                         MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                         PublicSessionGender = null,
                                                                                                         PublicSessionGenderTitle = null,
                                                                                                         PublicSessionGenderDisplay = null,
                                                                                                         SessionCost = x.SessionCost,
                                                                                                         SessionDate = x.SessionDate,
                                                                                                         StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                         StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                         StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                         Title = x.Title,
                                                                                                         UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                         UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                     };
                                                                                                     if (!x.UnitHasAccessForExtraReservation &&
                                                                                                                 ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                                  (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                                  (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                                  (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                        )
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                         else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                         {
                                                                                                             result.IsAvailable = false;
                                                                                                             result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                         }
                                                                                                         else
                                                                                                         {
                                                                                                             result.IsAvailable = true;
                                                                                                         }
                                                                                                     }
                                                                                                 }
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 result = new Middle_GetJointSessionsByCustomerDTO
                                                                                                 {
                                                                                                     Capacity = x.Capacity != null ? x.Capacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) ? x.Capacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.Count).Sum()) : 0 : null,
                                                                                                     Description = x.Description,
                                                                                                     EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                     EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                     EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                     GuestCapacity = x.GuestCapacity != null ? x.GuestCapacity > Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) ? x.GuestCapacity - Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(y => y.GuestCount).Sum()) : 0 : null,
                                                                                                     IsClosure = false,
                                                                                                     IsPrivate = false,
                                                                                                     Joint = middleJointModel,
                                                                                                     JointSessionId = x.JointSessionId,
                                                                                                     MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                     MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                     PublicSessionGender = x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                     PublicSessionGenderTitle = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "All",
                                                                                                         GenderType.ALL => "All",
                                                                                                         GenderType.MALE => "Male",
                                                                                                         GenderType.FAMALE => "Female",
                                                                                                         _ => "All",
                                                                                                     } : "All",
                                                                                                     PublicSessionGenderDisplay = x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                     {
                                                                                                         null => "همگانی",
                                                                                                         GenderType.ALL => "همگانی",
                                                                                                         GenderType.MALE => "آقایان",
                                                                                                         GenderType.FAMALE => "بانوان",
                                                                                                         _ => "همگانی",
                                                                                                     } : "همگانی",
                                                                                                     SessionCost = x.SessionCost,
                                                                                                     SessionDate = x.SessionDate,
                                                                                                     StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                     StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                     StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                     Title = x.Title,
                                                                                                     UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                     UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                                 };
                                                                                                 if (!x.UnitHasAccessForExtraReservation &&
                                                                                                             ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                              (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                              (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                              (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                    )
                                                                                                 {
                                                                                                     result.IsAvailable = false;
                                                                                                     result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     if (x.Capacity == null && x.GuestCapacity == null)
                                                                                                     {
                                                                                                         result.IsAvailable = true;
                                                                                                     }
                                                                                                     else if (((x.Capacity != null ? x.Capacity : 0) + (x.GuestCapacity != null ? x.GuestCapacity : 0)) <= ((Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(x.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                                                                                                     {
                                                                                                         result.IsAvailable = false;
                                                                                                         result.ForbiddenText = "ظرفیت مجاز سانس به پایان رسیده است";
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         result.IsAvailable = true;
                                                                                                     }
                                                                                                 }
                                                                                             }
                                                                                         }
                                                                                         else
                                                                                         {
                                                                                             result = new Middle_GetJointSessionsByCustomerDTO
                                                                                             {
                                                                                                 Capacity = x.Capacity,
                                                                                                 Description = x.Description,
                                                                                                 EndReservationDate = x.EndReservationDate?.ResetTime(),
                                                                                                 EndReservationTime = x.EndReservationDate?.ResetTimeSpan(),
                                                                                                 EndTime = x.EndTime?.ResetTimeSpan(),
                                                                                                 GuestCapacity = x.GuestCapacity,
                                                                                                 IsClosure = false,
                                                                                                 IsPrivate = x.IsPrivate,
                                                                                                 Joint = middleJointModel,
                                                                                                 JointSessionId = x.JointSessionId,
                                                                                                 MaximumReservationMinutes = x.MaximumReservationMinutes,
                                                                                                 MinimumReservationMinutes = x.MinimumReservationMinutes,
                                                                                                 PublicSessionGender = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender : GenderType.ALL,
                                                                                                 PublicSessionGenderTitle = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                 {
                                                                                                     null => "All",
                                                                                                     GenderType.ALL => "All",
                                                                                                     GenderType.MALE => "Male",
                                                                                                     GenderType.FAMALE => "Female",
                                                                                                     _ => "All"
                                                                                                 } : "All",
                                                                                                 PublicSessionGenderDisplay = x.IsPrivate ? null : x.PublicSessionGender != null ? x.PublicSessionGender switch
                                                                                                 {
                                                                                                     null => "همگانی",
                                                                                                     GenderType.ALL => "همگانی",
                                                                                                     GenderType.MALE => "آقایان",
                                                                                                     GenderType.FAMALE => "بانوان",
                                                                                                     _ => "همگانی"
                                                                                                 } : "همگانی",
                                                                                                 SessionCost = x.SessionCost,
                                                                                                 SessionDate = x.SessionDate,
                                                                                                 StartReservationDate = x.StartReservationDate?.ResetTime(),
                                                                                                 StartReservationTime = x.StartReservationDate?.ResetTimeSpan(),
                                                                                                 StartTime = x.StartTime?.ResetTimeSpan(),
                                                                                                 Title = x.Title,
                                                                                                 UnitHasAccessForExtraReservation = x.UnitHasAccessForExtraReservation,
                                                                                                 UnitExtraReservationCost = x.UnitExtraReservationCost,
                                                                                             };
                                                                                             if (!x.UnitHasAccessForExtraReservation &&
                                                                                                            ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                                                                                                             (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                                                                                                             (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                                                                                                             (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay))
                                                                                                   )
                                                                                             {
                                                                                                 result.IsAvailable = false;
                                                                                                 result.ForbiddenText = "سقف مجاز رزرو به پایان رسیده است";
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 result.IsAvailable = true;
                                                                                             }
                                                                                         }
                                                                                     }
                                                                                     return result;
                                                                                 }).ToList().Where(x => x.JointSessionId > 0).ToList();
            return Op.Succeed("دریافت لیست سانس ها با موفقیت انجام شد", new Response_GetJointSessionByCustomerDTO
            {
                RequestDate = RequestDate.ResetTime(),
                AvailableJointSessions = availableJointSessions,
                TodayJointSessions = todayJointSessions
            });

        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست سانس ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Middle_GetJointSessionsByCustomerDTO>> GetSingleJointSessionByCustomer(int CustomerId, int UnitId, long JointSessionId, CancellationToken cancellationToken = default)
    {
        OperationResult<Middle_GetJointSessionsByCustomerDTO> Op = new("GetSingleJointSessionByCustomer");
        if (CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (UnitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        if (JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Units.AnyAsync(x => x.Id == UnitId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Residents.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == UnitId && x.User.Id == CustomerId, cancellationToken: cancellationToken) &&
                !await _context.Owners.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == UnitId && x.User.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("واحد با کاربری ارسال شده مطابقت ندارد", HttpStatusCode.Conflict);
            }
            var jointSession = await _context.JointSessions.Include(x => x.AcceptableUnits).Include(x => x.Reservations).FirstOrDefaultAsync(x => x.JointSessionId == JointSessionId, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var joint = await _context.Joints.Include(x => x.JointStatus).Include(x => x.JointDailyActivityHours).Include(x => x.JointMultiMedias).FirstOrDefaultAsync(x => x.JointId == jointSession.JointId, cancellationToken: cancellationToken);
            if (joint == null)
            {
                return Op.Failed("دریافت اطلاعات مشاع با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (joint.JointStatus.Title.ToLower() != "active")
            {
                return Op.Failed("امکان دریافت سانس در صورت فعال نبودن مشاع وجود ندارد", HttpStatusCode.Forbidden);
            }
            var previousReservedOperation = await _GetUnitsReservedCounts(jointSession.JointId, Op.OperationDate, UnitId, cancellationToken);
            if (!previousReservedOperation.Success)
            {
                return Op.Failed(previousReservedOperation.Message, previousReservedOperation.ExMessage, previousReservedOperation.Status);
            }
            if (!jointSession.UnitHasAccessForExtraReservation &&
                    ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                    (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                    (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                    (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay)))
            {
                return Op.Failed("سقف مجاز رزرو برای واحد شما به پایان رسیده است", HttpStatusCode.Forbidden);
            }
            if (jointSession.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
            {
                return Op.Failed("دریافت اطلاعات سانس روزهای قبل از امروز امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
            }
            if (jointSession.IsClosure)
            {
                return Op.Failed("سانس تعطیل قابل رزرو نمیباشد");
            }
            if (jointSession.AcceptableUnits != null && jointSession.AcceptableUnits.Any() && !jointSession.AcceptableUnits.Any(x => x.UnitId == UnitId))
            {
                return Op.Failed("امکان دریافت اطلاعات سانس برای واحد شما مجاز نمیباشد", HttpStatusCode.Forbidden);
            }
            var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
            var startReservationDateTime = jointSession.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(jointSession.StartReservationDate, jointSession.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
            var endReservationDateTime = jointSession.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(jointSession.EndReservationDate, jointSession.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(jointSession.SessionDate, jointSession.StartTime != null ? jointSession.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
            if (requestDateTime < startReservationDateTime || requestDateTime > endReservationDateTime)
            {
                return Op.Failed("تاریخ و زمان برای دریافت اطلاعات سانس به پایان رسیده است", HttpStatusCode.Forbidden);
            }
            if (jointSession.Reservations != null && jointSession.Reservations.Any(x => !x.CancelledByByUser && !x.CancelledByAdmin))
            {
                if (jointSession.IsPrivate && jointSession.Reservations.Any(x => !x.CancelledByByUser && !x.CancelledByAdmin && x.ReservedForUnitId != UnitId))
                {
                    return Op.Failed("امکان دریافت اطلاعات سانس خصوصی برای واحد شما مجاز نمیباشد", HttpStatusCode.Forbidden);
                }
                if ((jointSession.Capacity != null || jointSession.GuestCapacity != null) && ((jointSession.Capacity != null ? jointSession.Capacity : 0) + (jointSession.GuestCapacity != null ? jointSession.GuestCapacity : 0) <= (Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                {
                    return Op.Failed("ظرفیت سانس به پایان رسیده است", HttpStatusCode.Forbidden);
                }
            }
            var result = new Middle_GetJointSessionsByCustomerDTO
            {
                Capacity = jointSession.Capacity != null ? jointSession.Reservations != null && jointSession.Reservations.Any() ? jointSession.Capacity > Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) ? jointSession.Capacity - Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) : 0 : jointSession.Capacity : null,
                Description = jointSession.Description,
                EndReservationDate = jointSession.EndReservationDate?.ResetTime(),
                EndReservationTime = jointSession.EndReservationDate?.ResetTimeSpan(),
                EndTime = jointSession.EndTime?.ResetTimeSpan(),
                ForbiddenText = null,
                GuestCapacity = jointSession.GuestCapacity != null ? jointSession.Reservations != null && jointSession.Reservations.Any() ? jointSession.GuestCapacity > Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) ? jointSession.GuestCapacity - Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) : 0 : jointSession.GuestCapacity : null,
                IsAvailable = true,
                IsClosure = false,
                IsPrivate = jointSession.IsPrivate,
                Joint = new Middle_JointDTO
                {
                    ActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO { PartialEndTime = x.PartialEndTime, PartialStartTime = x.PartialStartTime }).ToList(),
                    DailyUnitReservationCount = joint.DailyUnitReservationCount,
                    Description = joint.Description,
                    JointId = joint.JointId,
                    Location = joint.Location,
                    MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                    PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                    TermsFileUrl = joint.TermsFileUrl,
                    TermsText = joint.TermsText,
                    ThumbnailUrl = joint.ThumbnailUrl,
                    Title = joint.Title,
                    WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                    YearlyUnitReservationCount = joint.YearlyUnitReservationCount,
                },
                JointSessionId = jointSession.JointSessionId,
                MaximumReservationMinutes = jointSession.MaximumReservationMinutes,
                MinimumReservationMinutes = jointSession.MinimumReservationMinutes,
                PublicSessionGender = jointSession.IsPrivate ? null : jointSession.PublicSessionGender != null ? jointSession.PublicSessionGender : GenderType.ALL,
                PublicSessionGenderTitle = jointSession.IsPrivate ? null : jointSession.PublicSessionGender != null ? jointSession.PublicSessionGender switch
                {
                    null => "All",
                    GenderType.ALL => "All",
                    GenderType.MALE => "Male",
                    GenderType.FAMALE => "Female",
                    _ => "All"
                } : "All",
                PublicSessionGenderDisplay = jointSession.IsPrivate ? null : jointSession.PublicSessionGender != null ? jointSession.PublicSessionGender switch
                {
                    null => "همگانی",
                    GenderType.ALL => "همگانی",
                    GenderType.MALE => "آقایان",
                    GenderType.FAMALE => "بانوان",
                    _ => "همگانی"
                } : "همگانی",
                SessionCost = jointSession.SessionCost,
                SessionDate = jointSession.SessionDate.ResetTime(),
                StartReservationDate = jointSession.StartReservationDate?.ResetTime(),
                StartReservationTime = jointSession.StartReservationDate?.ResetTimeSpan(),
                StartTime = jointSession.StartTime?.ResetTimeSpan(),
                Title = jointSession.Title,
                UnitHasAccessForExtraReservation = jointSession.UnitHasAccessForExtraReservation,
                UnitExtraReservationCost = jointSession.UnitExtraReservationCost
            };
            return Op.Succeed("دریافت اطلاعات سانس با موفقیت انجام شد", result);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات سانس با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    private async Task<OperationResult<Response_GetUnitsReservedCountsDTO>> _GetUnitsReservedCounts(int JointId, DateTime RequestDate, int unitId, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetUnitsReservedCountsDTO> Op = new("_GetUnitsReservedCounts");
        if (unitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        try
        {
            var allReserved = (from reserved in await _context.Reservations.Include(x => x.JointSession).ToListAsync(cancellationToken: cancellationToken)
                               where !reserved.CancelledByAdmin && !reserved.CancelledByByUser && reserved.ReservedForUnitId == unitId && reserved.ReservationDate.ResetTime() <= RequestDate.ResetTime() && reserved.JointSession.JointId == JointId
                               select reserved).ToList();
            return Op.Succeed("دریافت اطلاعات رزرو های قبل با موفقیت انجام شد", new Response_GetUnitsReservedCountsDTO
            {
                ReservedInYear = allReserved == null || !allReserved.Any() ? 0 : allReserved.Where(x => x.ReservationDate.ResetTime().Year == RequestDate.ResetTime().Year).ToList().Count,
                ReservedInMonth = allReserved == null || !allReserved.Any() ? 0 : allReserved.Where(x => x.ReservationDate.ResetTime().Year == RequestDate.ResetTime().Year && x.ReservationDate.ResetTime().Month == RequestDate.ResetTime().Month).ToList().Count,
                ReservedInWeek = allReserved == null || !allReserved.Any() ? 0 : allReserved.Where(x => RequestDate.ResetTime() >= x.ReservationDate.GetWeekOfDate().From && RequestDate.ResetTime() <= x.ReservationDate.GetWeekOfDate().To).ToList().Count,
                ReservedInDay = allReserved == null || !allReserved.Any() ? 0 : allReserved.Where(x => x.ReservationDate.ResetTime().Year == RequestDate.ResetTime().Year && x.ReservationDate.ResetTime().Month == RequestDate.ResetTime().Month && x.ReservationDate.ResetTime().Day == RequestDate.ResetTime().Day).ToList().Count,
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات رزرو های قبل با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetUnitDTO>> GetUnitsByUserId(int CustomerId, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetUnitDTO> Op = new("GetUnitIDsByUserId");
        if (CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var unitIDs = new List<int>();
            unitIDs.AddRange((from ow in await _context.Owners.Include(x => x.User).Include(x => x.Unit).ToListAsync(cancellationToken: cancellationToken)
                              where ow.User.Id == CustomerId && ow.Unit.IsDelete != true
                              select ow.Unit.Id).ToList());
            unitIDs.AddRange((from re in await _context.Residents.Include(x => x.User).Include(x => x.Unit).ToListAsync(cancellationToken: cancellationToken)
                              where re.User.Id == CustomerId && re.Unit.IsDelete != true
                              select re.Unit.Id).ToList());
            if (unitIDs.Distinct().ToList() == null || !unitIDs.Distinct().ToList().Any())
            {
                return Op.Failed("واحدی به کاربری شما اختصاص داده نشده است", HttpStatusCode.NotFound);
            }
            return Op.Succeed("دریافت لیست واحد ها با موفقیت انجام شد", (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                                                         join id in unitIDs.Distinct().ToList()
                                                                         on u.Id equals id
                                                                         select new Response_GetUnitDTO
                                                                         {
                                                                             Id = u.Id,
                                                                             Name = u.Name,
                                                                         }).ToList());
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست واحد ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetReservationsByAdminDTO>> GetReservationsByAdmin(int JointId, Filter_GetReservationsByAdminDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetReservationsByAdminDTO> Op = new("GetReservationsByAdmin");
        if (JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (filter != null)
        {
            if (filter.JointSessionId != null && filter.JointSessionId <= 0)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
            }
            if (filter.ReservedForUnitId != null && filter.ReservedForUnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (filter.ReservedById != null && filter.ReservedById <= 0)
            {
                return Op.Failed("شناسه کاربر رزرو کننده بدرستی ارسال نشده است");
            }
        }
        try
        {
            var joint = (from j in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                         join s in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                         on j.JointStatusId equals s.JointStatusId
                         where j.JointId == JointId
                         select new Joint
                         {
                             Complex = j.Complex,
                             ComplexId = j.ComplexId,
                             DailyUnitReservationCount = j.DailyUnitReservationCount,
                             Description = j.Description,
                             JointDailyActivityHours = j.JointDailyActivityHours,
                             JointId = j.JointId,
                             JointMultiMedias = j.JointMultiMedias,
                             JointSessions = j.JointSessions,
                             JointStatus = s,
                             JointStatusId = j.JointStatusId,
                             Location = j.Location,
                             MonthlyUnitReservationCount = j.MonthlyUnitReservationCount,
                             PhoneNumbers = j.PhoneNumbers,
                             TermsFileUrl = j.TermsFileUrl,
                             TermsText = j.TermsText,
                             ThumbnailUrl = j.ThumbnailUrl,
                             Title = j.Title,
                             WeeklyUnitReservationCount = j.WeeklyUnitReservationCount,
                             YearlyUnitReservationCount = j.YearlyUnitReservationCount
                         }).FirstOrDefault();
            if (joint == null)
            {
                return Op.Failed("دریافت اطلاعات مشاع با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (filter != null)
            {
                if (filter.JointSessionId != null && !await _context.JointSessions.AnyAsync(x => x.JointSessionId == filter.JointSessionId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (filter.ReservedForUnitId != null && !await _context.Units.AnyAsync(x => x.Id == filter.ReservedForUnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (filter.ReservedById != null && !await _context.Users.AnyAsync(x => x.Id == filter.ReservedById, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه کاربر رزرو کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (filter.ReservedForUnitId != null && filter.ReservedById != null &&
                   !await _context.Owners.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.User.Id == filter.ReservedById, cancellationToken: cancellationToken) &&
                   !await _context.Residents.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.User.Id == filter.ReservedById, cancellationToken: cancellationToken))
                {
                    return Op.Failed("واحد با کاربری ارسال شده مطابقت ندارد", HttpStatusCode.Conflict);
                }
            }
            long totalCount = Convert.ToInt64((from r in await _context.Reservations.Include(x => x.JointSession).ToListAsync(cancellationToken: cancellationToken)
                                               where r.JointSession.JointId == JointId &&
                                                 (filter == null || (
                                                         (filter.JointSessionId == null || r.JointSessionId == filter.JointSessionId) &&
                                                         (filter.JointSessionDateFrom == null || r.JointSession.SessionDate.ResetTime() >= filter.JointSessionDateFrom.ResetTime()) &&
                                                         (filter.JointSessionDateTo == null || r.JointSession.SessionDate.ResetTime() <= filter.JointSessionDateTo.ResetTime()) &&
                                                         (filter.ReservedForUnitId == null || r.ReservedForUnitId == filter.ReservedForUnitId) &&
                                                         (filter.ReservedById == null || r.ReservedById == filter.ReservedById) &&
                                                         (filter.CancelledByAdmin == null || r.CancelledByAdmin == Convert.ToBoolean(filter.CancelledByAdmin)) &&
                                                         (filter.CancelledByUser == null || r.CancelledByByUser == Convert.ToBoolean(filter.CancelledByUser))
                                                  ))

                                               select r.ReservationId).ToList().Count);
            if (totalCount == 0)
            {
                return Op.Succeed("دریافت لیست رزرو ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            List<Middle_GetReservationsByAdminDTO> result = new();
            if (filter == null)
            {
                result = (from r in await _context.Reservations.Include(x => x.JointSession).ThenInclude(x => x.Reservations).Include(x => x.ReservedBy).Include(x => x.ReservedForUnit).ToListAsync(cancellationToken: cancellationToken)
                          where r.JointSession.JointId == JointId && (
                                  filter == null || (
                                                        (filter.JointSessionId == null || r.JointSessionId == filter.JointSessionId) &&
                                                        (filter.JointSessionDateFrom == null || r.JointSession.SessionDate.ResetTime() >= filter.JointSessionDateFrom.ResetTime()) &&
                                                        (filter.JointSessionDateTo == null || r.JointSession.SessionDate.ResetTime() <= filter.JointSessionDateTo.ResetTime()) &&
                                                        (filter.ReservedForUnitId == null || r.ReservedForUnitId == filter.ReservedForUnitId) &&
                                                        (filter.ReservedById == null || r.ReservedById == filter.ReservedById) &&
                                                        (filter.CancelledByAdmin == null || r.CancelledByAdmin == Convert.ToBoolean(filter.CancelledByAdmin)) &&
                                                        (filter.CancelledByUser == null || r.CancelledByByUser == Convert.ToBoolean(filter.CancelledByUser))
                                                    ))
                          select new Middle_GetReservationsByAdminDTO
                          {
                              CancellationDescription = r.CancellationDescription,
                              CancelledByAdmin = r.CancelledByAdmin,
                              CancelledByByUser = r.CancelledByByUser,
                              Cost = r.Cost,
                              Count = r.Count,
                              EndTime = r.EndDate.ResetTimeSpan(),
                              GuestCount = r.GuestCount,
                              JointSession = new Middle_GetReservationsByAdmin_JointSessionDTO
                              {
                                  Capacity = r.JointSession.Capacity,
                                  RemainCapacity = r.JointSession.Capacity != null ? r.JointSession.Reservations != null && r.JointSession.Reservations.Any() ? r.JointSession.Capacity > Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) ? r.JointSession.Capacity - Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) : 0 : r.JointSession.Capacity : null,
                                  Description = r.JointSession.Description,
                                  EndReservationDate = r.JointSession.EndReservationDate?.ResetTime(),
                                  EndReservationTime = r.JointSession.EndReservationDate?.ResetTimeSpan(),
                                  EndTime = r.JointSession.EndTime?.ResetTimeSpan(),
                                  GuestCapacity = r.JointSession.GuestCapacity,
                                  RemainGuestCapacity = r.JointSession.GuestCapacity != null ? r.JointSession.Reservations != null && r.JointSession.Reservations.Any() ? r.JointSession.GuestCapacity > Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) ? r.JointSession.GuestCapacity - Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) : 0 : r.JointSession.GuestCapacity : null,
                                  IsPrivate = r.JointSession.IsPrivate,
                                  Joint = new Middle_JointDTO
                                  {
                                      ActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO
                                      {
                                          PartialEndTime = x.PartialEndTime.ResetTimeSpan(),
                                          PartialStartTime = x.PartialStartTime.ResetTimeSpan()
                                      }).ToList(),
                                      DailyUnitReservationCount = joint.DailyUnitReservationCount,
                                      Description = joint.Description,
                                      JointId = joint.JointId,
                                      Location = joint.Location,
                                      MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                                      PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                                      TermsFileUrl = joint.TermsFileUrl,
                                      TermsText = joint.TermsText,
                                      ThumbnailUrl = joint.ThumbnailUrl,
                                      Title = joint.Title,
                                      WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                                      YearlyUnitReservationCount = joint.YearlyUnitReservationCount
                                  },
                                  JointSessionId = r.JointSessionId,
                                  PublicSessionGender = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender : GenderType.ALL,
                                  PublicSessionGenderTitle = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender switch
                                  {
                                      null => "All",
                                      GenderType.ALL => "All",
                                      GenderType.MALE => "Male",
                                      GenderType.FAMALE => "Female",
                                      _ => "All"
                                  } : "All",
                                  PublicSessionGenderDisplayTitle = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender switch
                                  {
                                      null => "همگانی",
                                      GenderType.ALL => "همگانی",
                                      GenderType.MALE => "آقایان",
                                      GenderType.FAMALE => "بانوان",
                                      _ => "همگانی"
                                  } : "همگانی",
                                  SessionDate = r.JointSession.SessionDate.ResetTime(),
                                  StartReservationDate = r.JointSession.StartReservationDate?.ResetTime(),
                                  StartReservationTime = r.JointSession.StartReservationDate?.ResetTimeSpan(),
                                  StartTime = r.JointSession.StartTime?.ResetTimeSpan(),
                                  Title = r.JointSession.Title
                              },
                              LastModificationDate = r.LastModificationDate,
                              LastModifier = r.LastModifier,
                              ReservationDate = r.ReservationDate.ResetTime(),
                              ReservationId = r.ReservationId,
                              StartTime = r.StartDate.ResetTimeSpan(),
                              ReservedBy = new Middle_GetReservationsByAdmin_ReservedBy
                              {
                                  Address = r.ReservedBy.Address,
                                  Age = r.ReservedBy.Age,
                                  Email = r.ReservedBy.Email,
                                  FirstName = r.ReservedBy.FirstName,
                                  Gender = r.ReservedBy.Gender != null ? r.ReservedBy.Gender : GenderType.ALL,
                                  GenderTitle = r.ReservedBy.Gender != null ? r.ReservedBy.Gender switch
                                  {
                                      null => "All",
                                      GenderType.ALL => "All",
                                      GenderType.MALE => "Male",
                                      GenderType.FAMALE => "Female",
                                      _ => "All"
                                  } : "All",
                                  GenderDisplayTitle = r.ReservedBy.Gender != null ? r.ReservedBy.Gender switch
                                  {
                                      null => "خانم/آقای",
                                      GenderType.ALL => "خانم/آقای",
                                      GenderType.MALE => "آقای",
                                      GenderType.FAMALE => "خانم",
                                      _ => "خانم/آقای"
                                  } : "خانم/آقای",
                                  Id = r.ReservedById,
                                  LastName = r.ReservedBy.LastName,
                                  NationalID = r.ReservedBy.NationalID,
                                  PhoneNumber = r.ReservedBy.PhoneNumber
                              },
                              ReservedForUnit = new Middle_GetReservationsByAdmin_ReservedForUnit
                              {
                                  Block = r.ReservedForUnit.Block,
                                  ComplexId = r.ReservedForUnit.ComplexId,
                                  Floor = r.ReservedForUnit.Floor,
                                  Id = r.ReservedForUnitId,
                                  Name = r.ReservedForUnit.Name
                              }

                          }).OrderByDescending(x => x.JointSession.SessionDate.ResetTime()).ThenBy(x => x.ReservationDate.ResetTime()).ToList();
            }
            else
            {
                result = (from r in await _context.Reservations.Include(x => x.JointSession).ThenInclude(x => x.Reservations).Include(x => x.ReservedBy).Include(x => x.ReservedForUnit).ToListAsync(cancellationToken: cancellationToken)
                          where r.JointSession.JointId == JointId && (
                                  filter == null || (
                                                        (filter.JointSessionId == null || r.JointSessionId == filter.JointSessionId) &&
                                                        (filter.JointSessionDateFrom == null || r.JointSession.SessionDate.ResetTime() >= filter.JointSessionDateFrom.ResetTime()) &&
                                                        (filter.JointSessionDateTo == null || r.JointSession.SessionDate.ResetTime() <= filter.JointSessionDateTo.ResetTime()) &&
                                                        (filter.ReservedForUnitId == null || r.ReservedForUnitId == filter.ReservedForUnitId) &&
                                                        (filter.ReservedById == null || r.ReservedById == filter.ReservedById) &&
                                                        (filter.CancelledByAdmin == null || r.CancelledByAdmin == Convert.ToBoolean(filter.CancelledByAdmin)) &&
                                                        (filter.CancelledByUser == null || r.CancelledByByUser == Convert.ToBoolean(filter.CancelledByUser))
                                                    )
                          )
                          select new Middle_GetReservationsByAdminDTO
                          {
                              CancellationDescription = r.CancellationDescription,
                              CancelledByAdmin = r.CancelledByAdmin,
                              CancelledByByUser = r.CancelledByByUser,
                              Cost = r.Cost,
                              Count = r.Count,
                              EndTime = r.EndDate.ResetTimeSpan(),
                              GuestCount = r.GuestCount,
                              JointSession = new Middle_GetReservationsByAdmin_JointSessionDTO
                              {
                                  Capacity = r.JointSession.Capacity,
                                  RemainCapacity = r.JointSession.Capacity != null ? r.JointSession.Reservations != null && r.JointSession.Reservations.Any() ? r.JointSession.Capacity > Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) ? r.JointSession.Capacity - Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) : 0 : r.JointSession.Capacity : null,
                                  Description = r.JointSession.Description,
                                  EndReservationDate = r.JointSession.EndReservationDate?.ResetTime(),
                                  EndReservationTime = r.JointSession.EndReservationDate?.ResetTimeSpan(),
                                  EndTime = r.JointSession.EndTime?.ResetTimeSpan(),
                                  GuestCapacity = r.JointSession.GuestCapacity,
                                  RemainGuestCapacity = r.JointSession.GuestCapacity != null ? r.JointSession.Reservations != null && r.JointSession.Reservations.Any() ? r.JointSession.GuestCapacity > Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) ? r.JointSession.GuestCapacity - Convert.ToInt32(r.JointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) : 0 : r.JointSession.GuestCapacity : null,
                                  IsPrivate = r.JointSession.IsPrivate,
                                  Joint = new Middle_JointDTO
                                  {
                                      ActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO
                                      {
                                          PartialEndTime = x.PartialEndTime.ResetTimeSpan(),
                                          PartialStartTime = x.PartialStartTime.ResetTimeSpan()
                                      }).ToList(),
                                      DailyUnitReservationCount = joint.DailyUnitReservationCount,
                                      Description = joint.Description,
                                      JointId = joint.JointId,
                                      Location = joint.Location,
                                      MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                                      PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                                      TermsFileUrl = joint.TermsFileUrl,
                                      TermsText = joint.TermsText,
                                      ThumbnailUrl = joint.ThumbnailUrl,
                                      Title = joint.Title,
                                      WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                                      YearlyUnitReservationCount = joint.YearlyUnitReservationCount
                                  },
                                  JointSessionId = r.JointSessionId,
                                  PublicSessionGender = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender : GenderType.ALL,
                                  PublicSessionGenderTitle = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender switch
                                  {
                                      null => "All",
                                      GenderType.ALL => "All",
                                      GenderType.MALE => "Male",
                                      GenderType.FAMALE => "Female",
                                      _ => "All"
                                  } : "All",
                                  PublicSessionGenderDisplayTitle = r.JointSession.IsPrivate ? null : r.JointSession.PublicSessionGender != null ? r.JointSession.PublicSessionGender switch
                                  {
                                      null => "همگانی",
                                      GenderType.ALL => "همگانی",
                                      GenderType.MALE => "آقایان",
                                      GenderType.FAMALE => "بانوان",
                                      _ => "همگانی"
                                  } : "همگانی",
                                  SessionDate = r.JointSession.SessionDate.ResetTime(),
                                  StartReservationDate = r.JointSession.StartReservationDate?.ResetTime(),
                                  StartReservationTime = r.JointSession.StartReservationDate?.ResetTimeSpan(),
                                  StartTime = r.JointSession.StartTime?.ResetTimeSpan(),
                                  Title = r.JointSession.Title
                              },
                              LastModificationDate = r.LastModificationDate,
                              LastModifier = r.LastModifier,
                              ReservationDate = r.ReservationDate.ResetTime(),
                              ReservationId = r.ReservationId,
                              StartTime = r.StartDate.ResetTimeSpan(),
                              ReservedBy = new Middle_GetReservationsByAdmin_ReservedBy
                              {
                                  Address = r.ReservedBy.Address,
                                  Age = r.ReservedBy.Age,
                                  Email = r.ReservedBy.Email,
                                  FirstName = r.ReservedBy.FirstName,
                                  Gender = r.ReservedBy.Gender != null ? r.ReservedBy.Gender : GenderType.ALL,
                                  GenderTitle = r.ReservedBy.Gender != null ? r.ReservedBy.Gender switch
                                  {
                                      null => "All",
                                      GenderType.ALL => "All",
                                      GenderType.MALE => "Male",
                                      GenderType.FAMALE => "Female",
                                      _ => "All"
                                  } : "All",
                                  GenderDisplayTitle = r.ReservedBy.Gender != null ? r.ReservedBy.Gender switch
                                  {
                                      null => "خانم/آقای",
                                      GenderType.ALL => "خانم/آقای",
                                      GenderType.MALE => "آقای",
                                      GenderType.FAMALE => "خانم",
                                      _ => "خانم/آقای"
                                  } : "خانم/آقای",
                                  Id = r.ReservedById,
                                  LastName = r.ReservedBy.LastName,
                                  NationalID = r.ReservedBy.NationalID,
                                  PhoneNumber = r.ReservedBy.PhoneNumber
                              },
                              ReservedForUnit = new Middle_GetReservationsByAdmin_ReservedForUnit
                              {
                                  Block = r.ReservedForUnit.Block,
                                  ComplexId = r.ReservedForUnit.ComplexId,
                                  Floor = r.ReservedForUnit.Floor,
                                  Id = r.ReservedForUnitId,
                                  Name = r.ReservedForUnit.Name
                              }

                          }).OrderByDescending(x => x.JointSession.SessionDate.ResetTime()).ThenBy(x => x.ReservationDate.ResetTime()).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList();
            }
            return Op.Succeed("دریافت لیست رزرو ها با موفقیت انجام شد", new Response_GetReservationsByAdminDTO
            {
                TotalCount = totalCount,
                Reservations = result
            });

        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست رزرو ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> ReserveJointSessionByCustomer(Request_ReserveJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("ReserveJointSessionByCustomer");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت رزرو وجود ندارد");
        }
        if (model.CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        if (model.UnitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        if (model.JointSessionId <= 0)
        {
            return Op.Failed("شناسه سانس بدرستی ارسال نشده است");
        }
        if (model.StartTime.ResetTimeSpan() > model.EndTime.ResetTimeSpan())
        {
            return Op.Failed("زمان شروع تا پایان رزرو بدرستی ارسال نشده است");
        }
        if (model.Count < 0)
        {
            return Op.Failed("تعداد افراد رزرو بدرستی ارسال نشده است");
        }
        if (model.GuestCount < 0)
        {
            return Op.Failed("تعداد افراد مهمان رزرو بدرستی ارسال نشده است");
        }
        if (model.Count == 0 && model.GuestCount == 0)
        {
            return Op.Failed("تعداد بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == model.CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Units.AnyAsync(x => x.Id == model.UnitId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!await _context.Residents.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == model.UnitId && x.User.Id == model.CustomerId, cancellationToken: cancellationToken) &&
                !await _context.Owners.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.IsDelete != true && x.Unit.Id == model.UnitId && x.User.Id == model.CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("واحد با کاربری ارسال شده مطابقت ندارد", HttpStatusCode.Conflict);
            }
            var jointSession = await _context.JointSessions.Include(x => x.AcceptableUnits).Include(x => x.Reservations).FirstOrDefaultAsync(x => x.JointSessionId == model.JointSessionId, cancellationToken: cancellationToken);
            if (jointSession == null)
            {
                return Op.Failed("شناسه سانس بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var joint = (from j in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                         join s in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                         on j.JointStatusId equals s.JointStatusId
                         where j.JointId == jointSession.JointId
                         select new Joint
                         {
                             Complex = j.Complex,
                             ComplexId = j.ComplexId,
                             DailyUnitReservationCount = j.DailyUnitReservationCount,
                             Description = j.Description,
                             JointDailyActivityHours = j.JointDailyActivityHours,
                             JointId = j.JointId,
                             JointMultiMedias = j.JointMultiMedias,
                             JointSessions = j.JointSessions,
                             JointStatus = s,
                             JointStatusId = j.JointStatusId,
                             Location = j.Location,
                             MonthlyUnitReservationCount = j.MonthlyUnitReservationCount,
                             PhoneNumbers = j.PhoneNumbers,
                             TermsFileUrl = j.TermsFileUrl,
                             TermsText = j.TermsText,
                             ThumbnailUrl = j.ThumbnailUrl,
                             Title = j.Title,
                             WeeklyUnitReservationCount = j.WeeklyUnitReservationCount,
                             YearlyUnitReservationCount = j.YearlyUnitReservationCount
                         }).FirstOrDefault();
            if (joint == null)
            {
                return Op.Failed("دریافت اطلاعات مشاع با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (joint.JointStatus.Title.ToLower() != "active")
            {
                return Op.Failed("امکان دریافت سانس در صورت فعال نبودن مشاع وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (jointSession.IsClosure)
            {
                return Op.Failed("سانس تعطیل قابل رزرو نمیباشد");
            }
            if (jointSession.AcceptableUnits != null && jointSession.AcceptableUnits.Any() && !jointSession.AcceptableUnits.Any(x => x.UnitId == model.UnitId))
            {
                return Op.Failed("امکان دریافت اطلاعات سانس برای واحد شما مجاز نمیباشد", HttpStatusCode.Forbidden);
            }
            var previousReservedOperation = await _GetUnitsReservedCounts(jointSession.JointId, Op.OperationDate, model.UnitId, cancellationToken);
            if (!previousReservedOperation.Success)
            {
                return Op.Failed(previousReservedOperation.Message, previousReservedOperation.ExMessage, previousReservedOperation.Status);
            }
            if (!jointSession.UnitHasAccessForExtraReservation &&
                    ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                    (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                    (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                    (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay)))
            {
                return Op.Failed("سقف مجاز رزرو برای واحد شما به پایان رسیده است", HttpStatusCode.Forbidden);
            }
            if (jointSession.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
            {
                return Op.Failed("دریافت اطلاعات سانس روزهای قبل از امروز امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
            }
            var requestDateTime = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
            var startReservationDateTime = jointSession.StartReservationDate != null ? DatetimeHelper.SetDateAndTime(jointSession.StartReservationDate, jointSession.StartReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan());
            var endReservationDateTime = jointSession.EndReservationDate != null ? DatetimeHelper.SetDateAndTime(jointSession.EndReservationDate, jointSession.EndReservationDate.ResetTimeSpan()) : DatetimeHelper.SetDateAndTime(jointSession.SessionDate, jointSession.StartTime != null ? jointSession.StartTime.ResetTimeSpan() : joint.JointDailyActivityHours.OrderBy(x => x.PartialStartTime).ToList()[0].PartialStartTime.ResetTimeSpan());
            if (requestDateTime < startReservationDateTime || requestDateTime > endReservationDateTime)
            {
                return Op.Failed("تاریخ و زمان برای دریافت اطلاعات سانس به پایان رسیده است", HttpStatusCode.Forbidden);
            }
            if (jointSession.Reservations != null && jointSession.Reservations.Any(x => !x.CancelledByByUser && !x.CancelledByAdmin))
            {
                if (jointSession.IsPrivate && jointSession.Reservations.Any(x => !x.CancelledByByUser && !x.CancelledByAdmin && x.ReservedForUnitId != model.UnitId))
                {
                    return Op.Failed("امکان دریافت اطلاعات سانس خصوصی برای واحد شما مجاز نمیباشد", HttpStatusCode.Forbidden);
                }
                if ((jointSession.Capacity != null || jointSession.GuestCapacity != null) && ((jointSession.Capacity != null ? jointSession.Capacity : 0) + (jointSession.GuestCapacity != null ? jointSession.GuestCapacity : 0) <= (Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum())) + (Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()))))
                {
                    return Op.Failed("ظرفیت سانس به پایان رسیده است", HttpStatusCode.Forbidden);
                }
            }
            if (model.Count > 0 && jointSession.Capacity != null && jointSession.Capacity < (jointSession.Reservations != null && jointSession.Reservations.Any(x => !x.CancelledByAdmin && !x.CancelledByByUser) ? Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.Count).Sum()) : 0) + model.Count)
            {
                return Op.Failed("تعداد ارسال شده از ظرفیت مجاز سانس بیشتر است", HttpStatusCode.Conflict);
            }
            if (model.GuestCount > 0 && jointSession.GuestCapacity != null && jointSession.GuestCapacity < (jointSession.Reservations != null && jointSession.Reservations.Any(x => !x.CancelledByAdmin && !x.CancelledByByUser) ? Convert.ToInt32(jointSession.Reservations.Where(x => !x.CancelledByAdmin && !x.CancelledByByUser).Select(x => x.GuestCount).Sum()) : 0) + model.GuestCount)
            {
                return Op.Failed("تعداد مهمان ارسال شده از ظرفیت مجاز سانس بیشتر است", HttpStatusCode.Conflict);
            }
            if (jointSession.StartTime != null && jointSession.EndTime != null)
            {
                if (model.StartTime.ResetTimeSpan() < jointSession.StartTime.ResetTimeSpan() || model.EndTime.ResetTimeSpan() > jointSession.EndTime.ResetTimeSpan())
                {
                    return Op.Failed("بازه زمانی رزرو ارسال شده در بازه مجاز سانس نمیباشد");
                }
            }
            else
            {
                if (joint.JointDailyActivityHours.Any(x => model.StartTime.ResetTimeSpan() < x.PartialStartTime.ResetTimeSpan() || model.EndTime.ResetTimeSpan() > x.PartialEndTime.ResetTimeSpan()))
                {
                    return Op.Failed("بازه زمانی رزرو ارسال شده در بازه مجاز مشاع نمیباشد");
                }
            }
            if (jointSession.MinimumReservationMinutes != null && model.EndTime.ResetTimeSpan().Subtract(model.StartTime.ResetTimeSpan()).TotalMinutes < jointSession.MinimumReservationMinutes)
            {
                return Op.Failed($"حداقل زمان رزرو {jointSession.MinimumReservationMinutes} دقیقه است");
            }
            if (jointSession.MaximumReservationMinutes != null && model.EndTime.ResetTimeSpan().Subtract(model.StartTime.ResetTimeSpan()).TotalMinutes > jointSession.MaximumReservationMinutes)
            {
                return Op.Failed($"حداکثر زمان رزرو {jointSession.MaximumReservationMinutes} دقیقه است");
            }
            decimal cost = 0;
            if (jointSession.UnitHasAccessForExtraReservation && jointSession.UnitExtraReservationCost != null &&
                    ((joint.YearlyUnitReservationCount != null && joint.YearlyUnitReservationCount == previousReservedOperation.Object.ReservedInYear) ||
                    (joint.MonthlyUnitReservationCount != null && joint.MonthlyUnitReservationCount == previousReservedOperation.Object.ReservedInMonth) ||
                    (joint.WeeklyUnitReservationCount != null && joint.WeeklyUnitReservationCount == previousReservedOperation.Object.ReservedInWeek) ||
                    (joint.DailyUnitReservationCount != null && joint.DailyUnitReservationCount == previousReservedOperation.Object.ReservedInDay)))
            {
                cost += Convert.ToDecimal(jointSession.UnitExtraReservationCost);
            }
            if (jointSession.SessionCost != null)
            {
                //cost += (model.Count + model.GuestCount) * Convert.ToDecimal(jointSession.SessionCost);
                cost += Convert.ToDecimal(jointSession.SessionCost);
            }
            await _context.Reservations.AddAsync(new Reservation
            {
                CancelledByAdmin = false,
                CancelledByByUser = false,
                Cost = cost,
                Count = model.Count,
                EndDate = model.EndTime.ResetTimeSpan(),
                GuestCount = model.GuestCount,
                JointSessionId = jointSession.JointSessionId,
                LastModificationDate = null,
                LastModifier = null,
                ReservationDate = DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan()),
                ReservedById = model.CustomerId,
                ReservedForUnitId = model.UnitId,
                StartDate = model.StartTime.ResetTimeSpan(),
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("رزرو با موفقیت انجام شد");

        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت رزرو با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Middle_GetReservationsByAdmin_ReservedBy>> GetCustomers(int? UnitId, CancellationToken cancellationToken = default)
    {
        OperationResult<Middle_GetReservationsByAdmin_ReservedBy> Op = new("GetCustomers");
        if (UnitId != null && UnitId <= 0)
        {
            return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
        }
        try
        {
            if (UnitId != null && !await _context.Units.AnyAsync(x => x.IsDelete != true && x.Id == UnitId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            List<int> UserIDs = new();
            UserIDs.AddRange((from ow in await _context.Owners.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                              where ow.Unit.IsDelete != true && (UnitId == null || ow.Unit.Id == UnitId)
                              select ow.User.Id).ToList());
            UserIDs.AddRange((from re in await _context.Residents.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                              where re.Unit.IsDelete != true && (UnitId == null || re.Unit.Id == UnitId)
                              select re.User.Id).ToList());
            if (UserIDs == null || !UserIDs.Distinct().Any())
            {
                return Op.Succeed("دریافت لیست کاربران با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد");
            }
            return Op.Succeed("دریافت لیست کاربران با موفقیت انجام شد", (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                                         join id in UserIDs.Distinct().ToList()
                                                                         on u.Id equals id
                                                                         select new Middle_GetReservationsByAdmin_ReservedBy
                                                                         {
                                                                             Address = u.Address,
                                                                             Age = u.Age,
                                                                             Email = u.Email,
                                                                             FirstName = u.FirstName,
                                                                             Gender = u.Gender != null ? u.Gender : GenderType.ALL,
                                                                             GenderDisplayTitle = u.Gender != null ? u.Gender switch
                                                                             {
                                                                                 null => "آقای/خانم",
                                                                                 GenderType.ALL => "آقای/خانم",
                                                                                 GenderType.MALE => "آقای",
                                                                                 GenderType.FAMALE => "خانم",
                                                                                 _ => "آقای/خانم",
                                                                             } : "آقای/خانم",
                                                                             GenderTitle = u.Gender != null ? u.Gender switch
                                                                             {
                                                                                 null => "All",
                                                                                 GenderType.ALL => "All",
                                                                                 GenderType.MALE => "Male",
                                                                                 GenderType.FAMALE => "Female",
                                                                                 _ => "آقای/خانم",
                                                                             } : "All",
                                                                             Id = u.Id,
                                                                             LastName = u.LastName,
                                                                             NationalID = u.NationalID,
                                                                             PhoneNumber = u.PhoneNumber
                                                                         }).ToList());

        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست کاربران با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetReservationsByCustomerDTO>> GetReservationsByCustomer(int CustomerId, Filter_GetReservationsByCustomerDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetReservationsByCustomerDTO> Op = new("GetReservationsByCustomer");
        if (CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
        }
        if (filter != null)
        {
            if (filter.JointId != null && filter.JointId <= 0)
            {
                return Op.Failed("شناسه مشاع فیلتر بدرستی ارسال نشده است");
            }
            if (filter.FromReservationDate != null && filter.ToReservationDate != null && filter.FromReservationDate.ResetTime() > filter.ToReservationDate.ResetTime())
            {
                return Op.Failed("تاریخ شروع و پایان رزرو فیلتر بدرستی ارسال نشده است");
            }
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (filter != null && filter.JointId != null && !await _context.Joints.AnyAsync(x => x.JointId == filter.JointId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه مشاع فیلتر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            List<Response_GetReservationsByCustomerDTO> result = (from reservation in await _context.Reservations.ToListAsync(cancellationToken: cancellationToken)
                                                                  join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                                                  on reservation.ReservedForUnitId equals unit.Id
                                                                  join session in await _context.JointSessions.Include(x => x.Reservations).Include(x => x.JointSessionCancellationHours).ToListAsync(cancellationToken: cancellationToken)
                                                                  on reservation.JointSessionId equals session.JointSessionId
                                                                  join joint in await _context.Joints.Include(x => x.JointDailyActivityHours).ToListAsync(cancellationToken: cancellationToken)
                                                                  on session.JointId equals joint.JointId
                                                                  where reservation.ReservedById == CustomerId &&
                                                                    (filter == null || (
                                                                            (filter.JointId == null || joint.JointId == filter.JointId) &&
                                                                            (filter.JointSessionDate == null || session.SessionDate.ResetTime() == filter.JointSessionDate.ResetTime()) &&
                                                                            (filter.FromReservationDate == null || reservation.ReservationDate.ResetTime() >= filter.FromReservationDate.ResetTime()) &&
                                                                            (filter.ToReservationDate == null || reservation.ReservationDate.ResetTime() <= filter.ToReservationDate.ResetTime()) &&
                                                                            (filter.CancelledByAdmin == null || reservation.CancelledByAdmin == Convert.ToBoolean(filter.CancelledByAdmin)) &&
                                                                            (filter.CancelledByUser == null || reservation.CancelledByByUser == Convert.ToBoolean(filter.CancelledByUser)))
                                                                    )
                                                                  select new Response_GetReservationsByCustomerDTO
                                                                  {
                                                                      CancellationDescription = reservation.CancellationDescription,
                                                                      CancelledByAdmin = reservation.CancelledByAdmin,
                                                                      CancelledByByUser = reservation.CancelledByByUser,
                                                                      Cost = reservation.Cost,
                                                                      Count = reservation.Count,
                                                                      EndTime = reservation.EndDate.ResetTimeSpan(),
                                                                      GuestCount = reservation.GuestCount,
                                                                      ReservationDate = reservation.ReservationDate.ResetTime(),
                                                                      ReservationId = reservation.ReservationId,
                                                                      StartTime = reservation.StartDate.ResetTimeSpan(),
                                                                      ReservedForUnit = new Middle_GetReservationsByAdmin_ReservedForUnit
                                                                      {
                                                                          Block = unit.Block,
                                                                          ComplexId = unit.ComplexId,
                                                                          Floor = unit.Floor,
                                                                          Id = unit.Id,
                                                                          Name = unit.Name
                                                                      },
                                                                      JointSession = new Middle_GetReservationsByAdmin_JointSessionDTO
                                                                      {
                                                                          Capacity = session.Capacity,
                                                                          Description = session.Description,
                                                                          EndReservationDate = session.EndReservationDate?.ResetTime(),
                                                                          EndReservationTime = session.EndReservationDate?.ResetTimeSpan(),
                                                                          EndTime = session.EndTime?.ResetTimeSpan(),
                                                                          GuestCapacity = session.GuestCapacity,
                                                                          IsPrivate = session.IsPrivate,
                                                                          JointSessionId = session.JointSessionId,
                                                                          PublicSessionGender = session.PublicSessionGender,
                                                                          PublicSessionGenderTitle = session.PublicSessionGender == null ? !session.IsPrivate ? "All" : null : session.PublicSessionGender switch
                                                                          {
                                                                              null => "All",
                                                                              GenderType.ALL => "All",
                                                                              GenderType.MALE => "Male",
                                                                              GenderType.FAMALE => "Female",
                                                                              _ => "All"
                                                                          },
                                                                          PublicSessionGenderDisplayTitle = session.PublicSessionGender == null ? !session.IsPrivate ? "همگانی" : null : session.PublicSessionGender switch
                                                                          {
                                                                              null => "همگانی",
                                                                              GenderType.ALL => "همگانی",
                                                                              GenderType.MALE => "آقایان",
                                                                              GenderType.FAMALE => "بانوان",
                                                                              _ => "همگانی"
                                                                          },
                                                                          SessionDate = session.SessionDate.ResetTime(),
                                                                          StartReservationDate = session.StartReservationDate?.ResetTime(),
                                                                          StartReservationTime = session.StartReservationDate?.ResetTimeSpan(),
                                                                          StartTime = session.StartTime?.ResetTimeSpan(),
                                                                          Title = session.Title,
                                                                          RemainCapacity = session.Capacity != null ? session.Reservations != null && session.Reservations.Any() ? session.Capacity > Convert.ToInt32(session.Reservations.Select(x => x.Count).Sum()) ? session.Capacity - Convert.ToInt32(session.Reservations.Select(x => x.Count).Sum()) : 0 : session.Capacity : null,
                                                                          RemainGuestCapacity = session.GuestCapacity != null ? session.Reservations != null && session.Reservations.Any() ? session.GuestCapacity > Convert.ToInt32(session.Reservations.Select(x => x.GuestCount).Sum()) ? session.GuestCapacity - Convert.ToInt32(session.Reservations.Select(x => x.GuestCount).Sum()) : 0 : session.GuestCapacity : null,
                                                                          Joint = new Middle_JointDTO
                                                                          {
                                                                              ActivityHours = joint.JointDailyActivityHours.Select(x => new Middle_JointActivityHoursDTO
                                                                              {
                                                                                  PartialEndTime = x.PartialEndTime,
                                                                                  PartialStartTime = x.PartialStartTime
                                                                              }).ToList(),
                                                                              DailyUnitReservationCount = joint.DailyUnitReservationCount,
                                                                              Description = joint.Description,
                                                                              JointId = joint.JointId,
                                                                              Location = joint.Location,
                                                                              MonthlyUnitReservationCount = joint.MonthlyUnitReservationCount,
                                                                              PhoneNumbers = joint.PhoneNumbers.IsNotNull() ? joint.PhoneNumbers.Split(",").ToList() : null,
                                                                              TermsFileUrl = joint.TermsFileUrl,
                                                                              TermsText = joint.TermsText,
                                                                              ThumbnailUrl = joint.ThumbnailUrl,
                                                                              Title = joint.Title,
                                                                              WeeklyUnitReservationCount = joint.WeeklyUnitReservationCount,
                                                                              YearlyUnitReservationCount = joint.YearlyUnitReservationCount
                                                                          },
                                                                      },
                                                                      Cancellable = !reservation.CancelledByByUser && !reservation.CancelledByAdmin && (Op.OperationDate <= DatetimeHelper.SetDateAndTime(session.SessionDate, session.StartTime.ResetTimeSpan()).AddHours(session.JointSessionCancellationHours != null && session.JointSessionCancellationHours.Any() && session.JointSessionCancellationHours.FirstOrDefault() != null && session.JointSessionCancellationHours.FirstOrDefault().Hour > 0 ? (-1 * session.JointSessionCancellationHours.FirstOrDefault().Hour) : 0))
                                                                  }).OrderByDescending(x => x.ReservationDate).ThenBy(x => x.JointSession.SessionDate).ToList();
            if (result == null || !result.Any())
            {
                return Op.Succeed("دریافت لیست رزرو ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
            }
            return Op.Succeed("دریافت لیست رزرو ها با موفقیت انجام شد", result);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست رزروها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByAdmin(int AdminId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CancelReservationDTO> Op = new("CancelReservationByAdmin");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت لغو وجود ندارد");
        }
        if (model.ReservationId <= 0)
        {
            return Op.Failed("شناسه رزرو بدرستی ارسال نشده است");
        }
        if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed("توضیحات لغو بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.IsDelete != true && x.Id == AdminId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var reservation = await _context.Reservations.Include(x => x.JointSession).ThenInclude(x => x.Joint).FirstOrDefaultAsync(x => x.ReservationId == model.ReservationId, cancellationToken: cancellationToken);
            if (reservation == null)
            {
                return Op.Failed("شناسه رزرو بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (reservation.CancelledByAdmin)
            {
                return Op.Failed("امکان لغو مجدد رزرو لغو شده توسط ادمین وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (reservation.CancelledByByUser)
            {
                return Op.Failed("امکان لغو مجدد رزرو لغو شده توسط کاربر وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (reservation.JointSession.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
            {
                return Op.Failed("امکان لغو رزرو برای سانس روز های قبل از امروز وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (DatetimeHelper.SetDateAndTime(Op.OperationDate, Op.OperationDate.ResetTimeSpan()) >= DatetimeHelper.SetDateAndTime(reservation.JointSession.SessionDate, reservation.StartDate.ResetTimeSpan()))
            {
                return Op.Failed("امکان لغو رزرو در جریان وجود ندارد", HttpStatusCode.Forbidden);
            }
            reservation.CancelledByAdmin = true;
            if (model.Description.IsNotNull())
            {
                reservation.CancellationDescription = model.Description.MultipleSpaceRemoverTrim();
            }
            reservation.LastModifier = AdminId;
            reservation.LastModificationDate = Op.OperationDate;
            _context.Entry<Reservation>(reservation).State = EntityState.Modified;
            var reservedBy = await _context.Users.FirstOrDefaultAsync(x => x.Id == reservation.ReservedById, cancellationToken: cancellationToken);
            if (reservedBy == null || !reservedBy.PhoneNumber.IsNotNull())
            {
                return Op.Failed("دریافت اطلاعات رزرو کننده با مشکل مواجه شده است");
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("لغو رزرو توسط ادمین با موفقیت انجام شد", new Response_CancelReservationDTO
            {
                JonitTitle = reservation.JointSession.Joint.Title,
                CancellationDescription = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : string.Empty,
                EndTime = reservation.EndDate.ResetTimeSpan(),
                SessionDate = reservation.JointSession.SessionDate.ResetTime(),
                StartTime = reservation.StartDate.ResetTimeSpan(),
                TargetPhoneNumber = reservedBy.PhoneNumber,
                ReservedByGenderDisplayTitle = reservedBy.Gender switch
                {
                    null => "خانم/آقای",
                    GenderType.MALE => "آقای",
                    GenderType.FAMALE => "خانم",
                    GenderType.ALL => "خانم/آقای",
                    _ => "خانم/آقای"
                },
                ReservedByFullName = $"{reservedBy.FirstName} {reservedBy.LastName}"
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("لغو رزرو توسط ادمین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<Response_CancelReservationDTO>> CancelReservationByCustomer(int CustomerId, Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_CancelReservationDTO> Op = new("CancelReservationByCustomer");
        if (CustomerId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت لغو وجود ندارد");
        }
        if (model.ReservationId <= 0)
        {
            return Op.Failed("شناسه رزرو بدرستی ارسال نشده است");
        }
        if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed("توضیحات لغو بدرستی ارسال نشده است");
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.IsDelete != true && x.Id == CustomerId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var reservation = await _context.Reservations.Include(x => x.JointSession).ThenInclude(x => x.JointSessionCancellationHours).FirstOrDefaultAsync(x => x.ReservationId == model.ReservationId && x.ReservedById == CustomerId, cancellationToken: cancellationToken);
            if (reservation == null)
            {
                return Op.Failed("شناسه رزرو بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (reservation.CancelledByAdmin)
            {
                return Op.Failed("امکان لغو مجدد رزرو لغو شده توسط ادمین وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (reservation.CancelledByByUser)
            {
                return Op.Failed("امکان لغو مجدد رزرو لغو شده توسط کاربر وجود ندارد", HttpStatusCode.Forbidden);
            }
            if (Op.OperationDate > DatetimeHelper.SetDateAndTime(reservation.JointSession.SessionDate, reservation.JointSession.StartTime.ResetTimeSpan()).AddHours(reservation.JointSession.JointSessionCancellationHours != null && reservation.JointSession.JointSessionCancellationHours.Any() && reservation.JointSession.JointSessionCancellationHours.FirstOrDefault() != null && reservation.JointSession.JointSessionCancellationHours.FirstOrDefault().Hour > 0 ? (-1 * reservation.JointSession.JointSessionCancellationHours.FirstOrDefault().Hour) : 0))
            {
                return Op.Failed("زمان لغو رزرو در بازه مجاز لغو نمیباشد", HttpStatusCode.Forbidden);
            }
            reservation.CancelledByByUser = true;
            if (model.Description.IsNotNull())
            {
                reservation.CancellationDescription = model.Description.MultipleSpaceRemoverTrim();
            }
            reservation.LastModifier = CustomerId;
            reservation.LastModificationDate = Op.OperationDate;
            _context.Entry<Reservation>(reservation).State = EntityState.Modified;
            var reservedBy = await _context.Users.FirstOrDefaultAsync(x => x.Id == reservation.ReservedById, cancellationToken: cancellationToken);
            if (reservedBy == null)
            {
                return Op.Failed("دریافت اطلاعات رزرو کننده با مشکل مواجه شده است");
            }
            var joint = await _context.Joints.FirstOrDefaultAsync(x => x.JointId == reservation.JointSession.JointId, cancellationToken: cancellationToken);
            if (joint == null || !joint.Title.IsNotNull())
            {
                return Op.Failed("دریافت اطلاعات مشاع با مشکل مواجه شده است");
            }
            var admin = await _context.Users.FirstOrDefaultAsync(x => x.Id == reservation.JointSession.Creator && x.IsDelete != true, cancellationToken: cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("لغو رزرو توسط کاربر با موفقیت انجام شد", new Response_CancelReservationDTO
            {
                JonitTitle = joint.Title,
                CancellationDescription = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : string.Empty,
                EndTime = reservation.EndDate.ResetTimeSpan(),
                SessionDate = reservation.JointSession.SessionDate.ResetTime(),
                StartTime = reservation.StartDate.ResetTimeSpan(),
                TargetPhoneNumber = admin != null && admin.IsDelete != true && admin.PhoneNumber.IsNotNull() ? admin.PhoneNumber : string.Empty,
                ReservedByGenderDisplayTitle = reservedBy.Gender switch
                {
                    null => "خانم/آقای",
                    GenderType.MALE => "آقای",
                    GenderType.FAMALE => "خانم",
                    GenderType.ALL => "خانم/آقای",
                    _ => "خانم/آقای"
                },
                ReservedByFullName = $"{reservedBy.FirstName} {reservedBy.LastName}"
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("لغو رزرو با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateJointSessions(int AdminId, Request_CreateJointSessionsDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateJointSessions");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت ثبت سانس ارسال نشده است");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (!model.Type.IsNotNull() || !new List<string> { "closure", "public", "private" }.Any(x => x == model.Type.MultipleSpaceRemoverTrim().ToLower()))
        {
            return Op.Failed("نوع سانس بدرستی ارسال نشده است");
        }
        if (model.Year < DatetimeHelper.GetCurrentPersianYear() || model.Year > DatetimeHelper.GetCurrentPersianYear() + 1)
        {
            return Op.Failed("سال بدرستی ارسال نشده است");
        }
        if (model.Month < 1 || model.Month > 12)
        {
            return Op.Failed("ماه بدرستی ارسال نشده است");
        }
        if (model.Year == DatetimeHelper.GetCurrentPersianYear() && model.Month < DatetimeHelper.GetCurrentPersianMonth())
        {
            return Op.Failed("امکان تعریف سانس برای ماه های قبل از ماه جاری امکان پذیر نمیباشد");
        }
        if (!model.Days.Any())
        {
            return Op.Failed("تعریف حداقل یک روز در هفته اجباری است");
        }
        if (model.Days.Any(x => x < 1 || x > 7))
        {
            return Op.Failed("روزهای هفته بدرستی ارسال نشده است");
        }
        if (!model.Times.Any())
        {
            return Op.Failed("حداقل یک بازه زمانی برای سانس اجباری است");
        }
        if (model.Times.Any(x => x.StartTime.ResetTimeSpan() >= x.EndTime.ResetTimeSpan()))
        {
            return Op.Failed($"بازه زمانی بدرستی ارسال نشده است، خطا در ردیف : {model.Times.FindIndex(x => x.StartTime.ResetTimeSpan() >= x.EndTime.ResetTimeSpan()) + 1}");
        }
        if (model.Times.Count > 1)
        {
            var start = model.Times[0].StartTime.ResetTimeSpan();
            var end = model.Times[0].EndTime.ResetTimeSpan();
            for (int i = 1; i < model.Times.Count; i++)
            {
                if (
                    (model.Times[i].StartTime.ResetTimeSpan() < start && model.Times[i].EndTime.ResetTimeSpan() <= start) ||
                    (model.Times[i].StartTime.ResetTimeSpan() >= end && model.Times[i].EndTime.ResetTimeSpan() > end)
                   )
                {
                    start = model.Times[i].StartTime.ResetTimeSpan();
                    end = model.Times[i].EndTime.ResetTimeSpan();
                }
                else
                {
                    return Op.Failed("امکان تعریف بازه زمانی سانس با تداخل وجود ندارد");
                }
            }
        }
        if (model.Title.IsNotNull() && model.Title.MultipleSpaceRemoverTrim().Length > 80)
        {
            return Op.Failed("عنوان سانس بدرستی ارسال نشده است");
        }
        if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed("توضیحات سانس بدرستی ارسال نشده است");
        }
        if (model.Type.MultipleSpaceRemoverTrim().ToLower() != "closure")
        {
            if (model.Type.MultipleSpaceRemoverTrim().ToLower() == "public")
            {
                if (model.PublicSessionGender == null)
                {
                    return Op.Failed("تعریف جنسیت سانس عمومی اجباری است");
                }
                if (!new List<int> { (int)GenderType.ALL, (int)GenderType.MALE, (int)GenderType.FAMALE }.Any(x => x == model.PublicSessionGender))
                {
                    return Op.Failed("جنسیت سانس عمومی بدرستی ارسال نشده است");
                }
            }
            if (model.StartReservationFrom != null)
            {
                if (!model.StartReservationFrom.Type.IsNotNull())
                {
                    return Op.Failed("ارسال نوع زمان شروع رزرو اجباری است");
                }
                if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.StartReservationFrom.Type.MultipleSpaceRemoverTrim().ToLower()))
                {
                    return Op.Failed("نوع زمان شروع رزرو بدرستی ارسال نشده است");
                }
                if (model.StartReservationFrom.Value < 0)
                {
                    return Op.Failed("زمان شروع رزرو بدرستی ارسال نشده است");
                }
            }
            if (model.EndReservationTo != null)
            {
                if (!model.EndReservationTo.Type.IsNotNull())
                {
                    return Op.Failed("ارسال نوع زمان پایان رزرو اجباری است");
                }
                if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.EndReservationTo.Type.MultipleSpaceRemoverTrim().ToLower()))
                {
                    return Op.Failed("نوع زمان پایان رزرو بدرستی ارسال نشده است");
                }
                if (model.EndReservationTo.Value < 0)
                {
                    return Op.Failed("زمان پایان رزرو بدرستی ارسال نشده است");
                }
            }
            if (model.StartReservationFrom != null && model.EndReservationTo != null)
            {
                var startValueHour = 0;
                var endValueHour = 0;
                switch (model.StartReservationFrom.Type)
                {
                    case "monthly":
                        startValueHour = model.StartReservationFrom.Value * 30 * 24;
                        break;
                    case "weekly":
                        startValueHour = model.StartReservationFrom.Value * 7 * 24;
                        break;
                    case "daily":
                        startValueHour = model.StartReservationFrom.Value * 24;
                        break;
                    case "hourly":
                        startValueHour = model.StartReservationFrom.Value;
                        break;
                }
                switch (model.EndReservationTo.Type)
                {
                    case "monthly":
                        endValueHour = model.EndReservationTo.Value * 30 * 24;
                        break;
                    case "weekly":
                        endValueHour = model.EndReservationTo.Value * 7 * 24;
                        break;
                    case "daily":
                        endValueHour = model.EndReservationTo.Value * 24;
                        break;
                    case "hourly":
                        endValueHour = model.EndReservationTo.Value;
                        break;
                }
                if (endValueHour > startValueHour)
                {
                    return Op.Failed("زمان شروع و پایان رزرو بدرستی ارسال نشده است");
                }
            }
            if (model.SessionCost != null && model.SessionCost < 0)
            {
                return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
            }
            if (model.HasUnitMoreReservationAccess == null)
            {
                return Op.Failed("امکان یا عدم امکان ثبت رزرو بیش از حد مجاز واحد بدرستی ارسال نشده است");
            }
            if (model.HasUnitMoreReservationAccess == true && model.UnitMoreReservationAccessCost != null && model.UnitMoreReservationAccessCost < 0)
            {
                return Op.Failed("هزینه اضافی رزرو واحد بدرستی ارسال نشده است");
            }
            if (model.Capacity != null && model.Capacity < 0)
            {
                return Op.Failed("ظرفیت سانس بدرستی ارسال نشده است");
            }
            if (model.GuestCapacity != null && model.GuestCapacity < 0)
            {
                return Op.Failed("ظرفیت مهمان سانس بدرستی ارسال نشده است");
            }
            if (model.CancellationTo != null)
            {
                if (!model.CancellationTo.Type.IsNotNull())
                {
                    return Op.Failed("ارسال نوع زمان مجاز برای لغو رزرو اجباری است");
                }
                if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.CancellationTo.Type.MultipleSpaceRemoverTrim().ToLower()))
                {
                    return Op.Failed("نوع زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
                }
                if (model.CancellationTo.Value < 0)
                {
                    return Op.Failed("زمان مجاز برای لغو رزرو بدرستی ارسال نشده است");
                }
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any() && model.AcceptableUnitIDs.Distinct().Any(x => x <= 0))
            {
                return Op.Failed($"شناسه واحد مجاز برای رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
            }
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == AdminId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!(from joint in await _context.Joints.ToListAsync(cancellationToken: cancellationToken)
                  join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                  on joint.JointStatusId equals status.JointStatusId
                  where status.Title.ToLower() == "active"
                  select joint.JointId).Any(x => x == model.JointId))
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var unitIDs = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                               where u.IsDelete != true
                               select u.Id).Distinct().ToList();
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!unitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد بدرستی ارسال نشده است، خطا در ردیف : {unitIDs.FindIndex(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]) + 1}");
                    }
                }
            }
            var jointActivityHours = await _context.JointDailyActivityHours.Where(x => x.JointId == model.JointId).ToListAsync(cancellationToken: cancellationToken);
            if (jointActivityHours == null || !jointActivityHours.Any())
            {
                return Op.Failed("دریافت بازه های فعالیت مشاع با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            for (int i = 0; i < model.Times.Count; i++)
            {
                if (!jointActivityHours.Any(x => model.Times[i].StartTime.ResetTimeSpan() >= x.PartialStartTime.ResetTimeSpan() && model.Times[i].EndTime.ResetTimeSpan() <= x.PartialEndTime.ResetTimeSpan()))
                {
                    return Op.Failed($"بازه زمانی ارسال شده در ساعات فعالیت مشاع نمیباشد، خطا در ردیف : {i + 1}");
                }
            }
            var datesOfMonth = model.Year == DatetimeHelper.GetCurrentPersianYear() && model.Month == DatetimeHelper.GetCurrentPersianMonth() ? DatetimeHelper.GetDatesOfMonthWithDayOfWeeks(model.Year, model.Month, model.Days).Where(x => x >= Op.OperationDate.ResetTime()).ToList() : DatetimeHelper.GetDatesOfMonthWithDayOfWeeks(model.Year, model.Month, model.Days);
            if (datesOfMonth == null || !datesOfMonth.Any())
            {
                return Op.Failed("ساخت تاریخ ها با مشکل مواجه شد", HttpStatusCode.NotFound);
            }
            List<JointSession> data = new();
            for (int date_i = 0; date_i < datesOfMonth.Count; date_i++)
            {
                for (int time_i = 0; time_i < model.Times.Count; time_i++)
                {
                    var existJointsSessions = await _context.JointSessions.Where(x => x.JointId == model.JointId && x.SessionDate == datesOfMonth[date_i].ResetTime()).ToListAsync(cancellationToken: cancellationToken);
                    if (existJointsSessions != null && existJointsSessions.Any())
                    {
                        for (int check_i = 0; check_i < existJointsSessions.Count; check_i++)
                        {
                            if ((model.Times[time_i].StartTime.ResetTimeSpan() < existJointsSessions[check_i].StartTime.ResetTimeSpan() && model.Times[time_i].EndTime.ResetTimeSpan() <= existJointsSessions[check_i].StartTime.ResetTimeSpan()) ||
                                (model.Times[time_i].StartTime.ResetTimeSpan() >= existJointsSessions[check_i].EndTime.ResetTimeSpan() && model.Times[time_i].EndTime.ResetTimeSpan() > existJointsSessions[check_i].EndTime.ResetTimeSpan()))
                            {
                                continue;
                            }
                            else
                            {
                                return Op.Failed($"تعریف سانس با سانس تاریخ {datesOfMonth[date_i].ResetTime().GetPersianDateString()} از {existJointsSessions[check_i].StartTime.ResetTimeSpan().GetTimeString()} - تا {existJointsSessions[check_i].EndTime.ResetTimeSpan().GetTimeString()} تداخل دارد", HttpStatusCode.Conflict);
                            }

                        }
                    }
                    if (model.Type.MultipleSpaceRemoverTrim().ToLower() == "closure")
                    {
                        data.Add(new JointSession
                        {
                            CreationDate = Op.OperationDate,
                            Creator = AdminId,
                            Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                            Capacity = null,
                            EndReservationDate = null,
                            EndTime = model.Times[time_i].EndTime.ResetTimeSpan(),
                            GuestCapacity = null,
                            IsClosure = true,
                            IsPrivate = false,
                            JointId = model.JointId,
                            MaximumReservationMinutes = null,
                            MinimumReservationMinutes = null,
                            PublicSessionGender = null,
                            SessionCost = null,
                            SessionDate = datesOfMonth[date_i].ResetTime(),
                            StartReservationDate = null,
                            StartTime = model.Times[time_i].StartTime.ResetTimeSpan(),
                            Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                            UnitExtraReservationCost = null,
                            UnitHasAccessForExtraReservation = false,
                        });
                    }
                    else
                    {
                        var session = new JointSession
                        {
                            CreationDate = Op.OperationDate,
                            Creator = AdminId,
                            Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                            Capacity = model.Capacity != null && model.Capacity > 0 ? model.Capacity : 0,
                            EndTime = model.Times[time_i].EndTime.ResetTimeSpan(),
                            GuestCapacity = model.GuestCapacity != null && model.GuestCapacity > 0 ? model.GuestCapacity : null,
                            IsClosure = false,
                            IsPrivate = model.Type.MultipleSpaceRemoverTrim().ToLower() == "private",
                            JointId = model.JointId,
                            MaximumReservationMinutes = null,
                            MinimumReservationMinutes = null,
                            PublicSessionGender = model.Type.MultipleSpaceRemoverTrim().ToLower() == "private" ? null : model.PublicSessionGender == null ? GenderType.ALL : (GenderType)model.PublicSessionGender,
                            SessionCost = model.SessionCost != null ? model.SessionCost : null,
                            SessionDate = datesOfMonth[date_i].ResetTime(),
                            StartTime = model.Times[time_i].StartTime.ResetTimeSpan(),
                            Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                            UnitHasAccessForExtraReservation = Convert.ToBoolean(model.HasUnitMoreReservationAccess),
                            UnitExtraReservationCost = Convert.ToBoolean(model.HasUnitMoreReservationAccess) ? model.UnitMoreReservationAccessCost != null ? model.UnitMoreReservationAccessCost : null : null,
                        };
                        if (model.StartReservationFrom != null)
                        {
                            switch (model.StartReservationFrom.Type)
                            {
                                case "monthly":
                                    session.StartReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddMonths(model.StartReservationFrom.Value > 0 ? -1 * model.StartReservationFrom.Value : 0);
                                    break;
                                case "weekly":
                                    session.StartReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddDays(model.StartReservationFrom.Value > 0 ? -1 * 7 * model.StartReservationFrom.Value : 0);
                                    break;
                                case "daily":
                                    session.StartReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddDays(model.StartReservationFrom.Value > 0 ? -1 * model.StartReservationFrom.Value : 0);
                                    break;
                                case "hourly":
                                    session.StartReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddHours(model.StartReservationFrom.Value > 0 ? -1 * model.StartReservationFrom.Value : 0);
                                    break;
                            }
                        }
                        if (model.EndReservationTo != null)
                        {
                            switch (model.EndReservationTo.Type)
                            {
                                case "monthly":
                                    session.EndReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddMonths(model.EndReservationTo.Value > 0 ? -1 * model.EndReservationTo.Value : 0);
                                    break;
                                case "weekly":
                                    session.EndReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddDays(model.EndReservationTo.Value > 0 ? -1 * 7 * model.EndReservationTo.Value : 0);
                                    break;
                                case "daily":
                                    session.EndReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddDays(model.EndReservationTo.Value > 0 ? -1 * model.EndReservationTo.Value : 0);
                                    break;
                                case "hourly":
                                    session.EndReservationDate = DatetimeHelper.SetDateAndTime(datesOfMonth[date_i].ResetTime(), model.Times[time_i].StartTime.ResetTimeSpan()).AddHours(model.EndReservationTo.Value > 0 ? -1 * model.EndReservationTo.Value : 0);
                                    break;
                            }
                        }
                        data.Add(session);
                    }
                }
            }
            await _context.JointSessions.AddRangeAsync(data, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            if (model.Type.MultipleSpaceRemoverTrim().ToLower() != "closure" && ((model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any()) || (model.CancellationTo != null)))
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (model.CancellationTo != null)
                    {
                        await _context.JointSessionCancellationHours.AddAsync(new JointSessionCancellationHour
                        {
                            Hour = model.CancellationTo.Type switch
                            {
                                "monthly" => model.CancellationTo.Value * 30 * 24,
                                "weekly" => model.CancellationTo.Value * 7 * 24,
                                "daily" => model.CancellationTo.Value * 24,
                                "hourly" => model.CancellationTo.Value,
                                _ => model.CancellationTo.Value
                            },
                            JointSessionId = data[i].JointSessionId
                        }, cancellationToken);
                    }
                    if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
                    {
                        await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new JointSessionAcceptableUnit
                        {
                            JointSessionId = data[i].JointSessionId,
                            UnitId = x
                        }), cancellationToken);
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
            return Op.Succeed("ثبت سانس ها با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سانس ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public OperationResult<Response_GetYearMonthDTO> GetYearsMonths()
    {
        OperationResult<Response_GetYearMonthDTO> Op = new("GetYearsMonths");
        try
        {
            List<Response_GetYearMonthDTO> result = new()
            {
                new() {Year = DatetimeHelper.GetCurrentPersianYear()},new() {Year = DatetimeHelper.GetCurrentPersianYear() + 1}
            };
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Year == DatetimeHelper.GetCurrentPersianYear())
                {
                    result[i].Months = new List<Middle_GetYearMonth_MonthDTO>();
                    for (int m = DatetimeHelper.GetCurrentPersianMonth(); m <= 12; m++)
                    {
                        result[i].Months.Add(new Middle_GetYearMonth_MonthDTO
                        {
                            Month = m,
                            MonthTitle = m.GetMonthTitle()
                        });
                    }
                }
                else
                {
                    result[i].Months = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }.Select(x => new Middle_GetYearMonth_MonthDTO
                    {
                        Month = x,
                        MonthTitle = x.GetMonthTitle()
                    }).ToList();
                }
            }
            return Op.Succeed("دریافت لیست سال و ماه ها با موفقیت انجام شد", result);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت لیست سال و ماه ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateSingleJointSession(int AdminId, Request_CreateSingleJointSessionDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateSingleJointSession");
        if (AdminId <= 0)
        {
            return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
        }
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت ثبت سانس ارسال نشده است");
        }
        if (model.JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (!model.Type.IsNotNull() || !new List<string> { "closure", "public", "private" }.Any(x => x == model.Type.MultipleSpaceRemoverTrim().ToLower()))
        {
            return Op.Failed("نوع سانس بدرستی ارسال نشده است");
        }
        if (model.SessionDate.ResetTime() < Op.OperationDate.ResetTime())
        {
            return Op.Failed("ثبت سانس برای روز های قبل از امروز امکانپذیر نمیباشد");
        }
        if (model.StartTime.ResetTimeSpan() >= model.EndTime.ResetTimeSpan())
        {
            return Op.Failed("ساعت شروع و پایان سانس بدرستی ارسال نشده است");
        }
        if (model.SessionDate.ResetTime() == Op.OperationDate.ResetTime() && model.StartTime < Op.OperationDate.ResetTimeSpan())
        {
            return Op.Failed($"امکان ثبت سانس برای ساعت قبل از {$"{Op.OperationDate.ResetTimeSpan().Hours}:{Op.OperationDate.ResetTimeSpan().Minutes:00}"} امکانپذیر نمیباشد");
        }
        if (model.Title.IsNotNull() && model.Title.MultipleSpaceRemoverTrim().Length > 80)
        {
            return Op.Failed("عنوان سانس بدرستی ارسال نشده است");
        }
        if (model.Description.IsNotNull() && model.Description.MultipleSpaceRemoverTrim().Length > 1800)
        {
            return Op.Failed("توضیحات سانس بدرستی ارسال نشده است");
        }
        if (model.Type.MultipleSpaceRemoverTrim().ToLower() != "closure")
        {
            if (model.Type.MultipleSpaceRemoverTrim().ToLower() == "public")
            {
                if (model.PublicSessionGender == null)
                {
                    return Op.Failed("تعریف جنسیت سانس عمومی اجباری است");
                }
                if (!new List<int> { (int)GenderType.ALL, (int)GenderType.MALE, (int)GenderType.FAMALE }.Any(x => x == model.PublicSessionGender))
                {
                    return Op.Failed("جنسیت سانس عمومی بدرستی ارسال نشده است");
                }
            }
            if ((model.StartReservationFrom == null && model.EndReservationTo != null) || (model.StartReservationFrom != null && model.EndReservationTo == null))
            {
                return Op.Failed("زمان مجاز شروع و پایان ثبت رزرو بدرستی ارسال نشده است");
            }
            if (model.StartReservationFrom != null && model.EndReservationTo != null && model.StartReservationFrom >= model.EndReservationTo)
            {
                return Op.Failed("زمان مجاز شروع و پایان ثبت رزرو بدرستی ارسال نشده است");
            }
            if (model.EndReservationTo != null && model.EndReservationTo.ResetTime() == model.SessionDate.ResetTime() && model.EndReservationTo.ResetTimeSpan() > model.StartTime.ResetTimeSpan())
            {
                return Op.Failed("ساعت پایان ثبت مجاز رزرو نمیتواند از ساعت شروع سانس بزرگتر باشد");
            }
            if (model.SessionCost != null && model.SessionCost < 0)
            {
                return Op.Failed("هزینه سانس بدرستی ارسال نشده است");
            }
            if (model.HasUnitMoreReservationAccess == null)
            {
                return Op.Failed("ارسال امکان ثبت رزرو اضافه برای واحد اجباری است");
            }
            if (model.HasUnitMoreReservationAccess == false && model.UnitMoreReservationAccessCost != null)
            {
                return Op.Failed("ارسال هزینه اضافه رزرو هر واحد در صورت امکان ثبت رزرو اضافه امکانپذیر است");
            }
            if (model.HasUnitMoreReservationAccess == true && model.UnitMoreReservationAccessCost != null && model.UnitMoreReservationAccessCost < 0)
            {
                return Op.Failed("هزینه اضافی رزرو هر واحد بدرستی ارسال نشده است");
            }
            if (model.Capacity != null && model.Capacity < 0)
            {
                return Op.Failed("ظرفیت رزرو بدرستی ارسال نشده است");
            }
            if (model.GuestCapacity != null && model.GuestCapacity < 0)
            {
                return Op.Failed("ظرفیت مهمان رزرو بدرستی ارسال نشده است");
            }
            if (model.CancellationTo != null)
            {
                if (!model.CancellationTo.Type.IsNotNull())
                {
                    return Op.Failed("ارسال نوع زمان محدودیت لغو رزرو اجباری است");
                }
                if (!new List<string> { "monthly", "weekly", "daily", "hourly" }.Any(x => x == model.CancellationTo.Type.MultipleSpaceRemoverTrim().ToLower()))
                {
                    return Op.Failed("نوع زمان محدودیت لغو رزرو بدرستی ارسال نشده است");
                }
                if (model.CancellationTo.Value < 0)
                {
                    return Op.Failed("زمان محدودیت لغو رزرو بدرستی ارسال نشده است");
                }
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any() && model.AcceptableUnitIDs.Distinct().Any(x => x <= 0))
            {
                return Op.Failed($"شناسه واحد مجاز برای رزرو بدرستی ارسال نشده است، خطا در ردیف : {model.AcceptableUnitIDs.FindIndex(x => x <= 0) + 1}");
            }
        }
        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == AdminId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!(from joint in await _context.Joints.ToListAsync(cancellationToken: cancellationToken)
                  join status in await _context.JointStatuses.ToListAsync(cancellationToken: cancellationToken)
                  on joint.JointStatusId equals status.JointStatusId
                  where status.Title.ToLower() == "active"
                  select joint.JointId).Any(x => x == model.JointId))
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
            {
                var unitIDs = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                               where u.IsDelete != true
                               select u.Id).Distinct().ToList();
                for (int i = 0; i < model.AcceptableUnitIDs.Distinct().ToList().Count; i++)
                {
                    if (!unitIDs.Any(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]))
                    {
                        return Op.Failed($"شناسه واحد بدرستی ارسال نشده است، خطا در ردیف : {unitIDs.FindIndex(x => x == model.AcceptableUnitIDs.Distinct().ToList()[i]) + 1}");
                    }
                }
            }
            var jointActivityHours = await _context.JointDailyActivityHours.Where(x => x.JointId == model.JointId).ToListAsync(cancellationToken: cancellationToken);
            if (jointActivityHours == null || !jointActivityHours.Any())
            {
                return Op.Failed("دریافت بازه های فعالیت مشاع با مشکل مواجه شده است", HttpStatusCode.NotFound);
            }
            if (!jointActivityHours.Any(x => model.StartTime.ResetTimeSpan() >= x.PartialStartTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() <= x.PartialEndTime.ResetTimeSpan()))
            {
                return Op.Failed("بازه زمانی سانس ارسال شده در ساعات فعالیت مشاع نمیباشد", HttpStatusCode.Conflict);
            }
            var onDaySessions = await _context.JointSessions.Where(x => x.JointId == model.JointId && x.SessionDate == model.SessionDate.ResetTime()).ToListAsync(cancellationToken: cancellationToken);
            if (onDaySessions != null && onDaySessions.Any())
            {
                for (int i = 0; i < onDaySessions.Count; i++)
                {
                    if (
                         (model.StartTime.ResetTimeSpan() >= onDaySessions[i].EndTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() > onDaySessions[i].EndTime.ResetTimeSpan()) ||
                         (model.StartTime.ResetTimeSpan() < onDaySessions[i].StartTime.ResetTimeSpan() && model.EndTime.ResetTimeSpan() <= onDaySessions[i].StartTime.ResetTimeSpan())
                      )
                    {
                        continue;
                    }
                    else
                    {
                        return Op.Failed("ثبت سانس با زمان متداخل با سانس های دیگر امکانپذیر نمیباشد");
                    }
                }
            }

            if (model.Type.MultipleSpaceRemoverTrim().ToLower() == "closure")
            {
                await _context.JointSessions.AddAsync(new JointSession
                {
                    CreationDate = Op.OperationDate,
                    Creator = AdminId,
                    Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                    Capacity = null,
                    EndReservationDate = null,
                    EndTime = model.EndTime.ResetTimeSpan(),
                    GuestCapacity = null,
                    IsClosure = true,
                    IsPrivate = false,
                    JointId = model.JointId,
                    MaximumReservationMinutes = null,
                    MinimumReservationMinutes = null,
                    PublicSessionGender = null,
                    SessionCost = null,
                    SessionDate = model.SessionDate.ResetTime(),
                    StartReservationDate = null,
                    StartTime = model.StartTime.ResetTimeSpan(),
                    Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                    UnitExtraReservationCost = null,
                    UnitHasAccessForExtraReservation = false,
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت سانس تعطیل با موفقیت انجام شد");
            }
            else
            {
                var data = new JointSession
                {
                    CreationDate = Op.OperationDate,
                    Creator = AdminId,
                    Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                    Capacity = model.Capacity != null && model.Capacity > 0 ? model.Capacity : 0,
                    EndReservationDate = model.StartReservationFrom != null && model.EndReservationTo != null ? DatetimeHelper.SetDateAndTime(model.EndReservationTo.ResetTime(), model.EndReservationTo.ResetTimeSpan()) : null,
                    EndTime = model.EndTime.ResetTimeSpan(),
                    GuestCapacity = model.GuestCapacity != null && model.GuestCapacity > 0 ? model.GuestCapacity : 0,
                    IsClosure = false,
                    IsPrivate = model.Type.MultipleSpaceRemoverTrim().ToLower() == "private",
                    JointId = model.JointId,
                    MaximumReservationMinutes = null,
                    MinimumReservationMinutes = null,
                    PublicSessionGender = model.Type.MultipleSpaceRemoverTrim().ToLower() == "private" ? null : model.PublicSessionGender == null ? GenderType.ALL : (GenderType)model.PublicSessionGender,
                    SessionCost = model.SessionCost != null ? model.SessionCost : null,
                    SessionDate = model.SessionDate.ResetTime(),
                    StartTime = model.StartTime.ResetTimeSpan(),
                    Title = model.Title.IsNotNull() ? model.Title.MultipleSpaceRemoverTrim() : null,
                    UnitHasAccessForExtraReservation = Convert.ToBoolean(model.HasUnitMoreReservationAccess),
                    UnitExtraReservationCost = Convert.ToBoolean(model.HasUnitMoreReservationAccess) ? model.UnitMoreReservationAccessCost != null ? model.UnitMoreReservationAccessCost : null : null,
                    StartReservationDate = model.StartReservationFrom != null && model.EndReservationTo != null ? DatetimeHelper.SetDateAndTime(model.StartReservationFrom.ResetTime(), model.StartReservationFrom.ResetTimeSpan()) : null,
                };
                await _context.JointSessions.AddAsync(data, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                if ((model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any()) || (model.CancellationTo != null))
                {
                    if (model.AcceptableUnitIDs != null && model.AcceptableUnitIDs.Any())
                    {
                        await _context.JointSessionAcceptableUnits.AddRangeAsync(model.AcceptableUnitIDs.Distinct().Select(x => new JointSessionAcceptableUnit
                        {
                            JointSessionId = data.JointSessionId,
                            UnitId = x,
                        }).ToList(), cancellationToken);
                    }
                    if (model.CancellationTo != null)
                    {
                        await _context.JointSessionCancellationHours.AddAsync(new JointSessionCancellationHour
                        {
                            Hour = model.CancellationTo.Type switch
                            {
                                "monthly" => model.CancellationTo.Value * 30 * 24,
                                "weekly" => model.CancellationTo.Value * 7 * 24,
                                "daily" => model.CancellationTo.Value * 24,
                                "hourly" => model.CancellationTo.Value,
                                _ => model.CancellationTo.Value
                            },
                            JointSessionId = data.JointSessionId
                        }, cancellationToken);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return Op.Succeed($"ثبت سانس {(model.Type.MultipleSpaceRemoverTrim().ToLower() == "private" ? "خصوصی" : "عمومی")} با موفقیت انجام شد");
            }

        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت سانس با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<Response_GetReservationReportDTO>> GetReservationReportByAdmin(int JointId, Filter_GetReservationReportDTO? filter, CancellationToken cancellationToken = default)
    {
        OperationResult<Response_GetReservationReportDTO> Op = new("GetReservationReportByAdmin");
        if (JointId <= 0)
        {
            return Op.Failed("شناسه مشاع بدرستی ارسال نشده است");
        }
        if (filter != null)
        {
            if (filter.FromDate != null && filter.ToDate != null && filter.FromDate.ResetTime() > filter.ToDate.ResetTime())
            {
                return Op.Failed("فیلتر شروع و پایان تاریخ سانس ها بدرستی ارسال نشده است");
            }
            if (filter.ReservedById != null && filter.ReservedById <= 0)
            {
                return Op.Failed("شناسه کاربر رزرو کننده بدرستی ارسال نشده است");
            }
            if (filter.ReservedForUnitId != null && filter.ReservedForUnitId <= 0)
            {
                return Op.Failed("شناسه واحد رزرو کننده بدرستی ارسال نشده است");
            }
        }
        try
        {
            var jointData = await _context.Joints.FirstOrDefaultAsync(x => x.JointId == JointId, cancellationToken: cancellationToken);
            if (jointData == null)
            {
                return Op.Failed("شناسه مشاع بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            var result = new Response_GetReservationReportDTO
            {
                Data = new List<Middle_GetReservationReport_DataDTO>(),
                FromDate = filter != null && filter.FromDate != null ? filter.FromDate.ResetTime() : null,
                ToDate = filter != null && filter.ToDate != null ? filter.ToDate.ResetTime() : null,
                JointTitle = jointData.Title
            };
            if (filter != null)
            {
                if (filter.ReservedById != null)
                {
                    var reservedBy = await _context.Users.FirstOrDefaultAsync(x => x.Id == filter.ReservedById, cancellationToken: cancellationToken);
                    if (reservedBy == null)
                    {
                        return Op.Failed("شناسه کاربر رزرو کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    else { result.UserFullName = $"{reservedBy.FirstName} {reservedBy.LastName}"; }
                }
                if (filter.ReservedForUnitId != null)
                {
                    var reservedForUnit = await _context.Units.FirstOrDefaultAsync(x => x.Id == filter.ReservedForUnitId, cancellationToken: cancellationToken);
                    if (reservedForUnit == null)
                    {
                        return Op.Failed("شناسه واحد رزرو کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    else { result.UnitName = reservedForUnit.Name; }

                }
            }
            result.Data.AddRange((from reservation in await _context.Reservations.ToListAsync(cancellationToken: cancellationToken)
                                  join unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                  on reservation.ReservedForUnitId equals unit.Id
                                  join user in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                  on reservation.ReservedById equals user.Id
                                  join session in await _context.JointSessions.ToListAsync(cancellationToken: cancellationToken)
                                  on reservation.JointSessionId equals session.JointSessionId
                                  join joint in await _context.Joints.ToListAsync(cancellationToken: cancellationToken)
                                  on session.JointId equals joint.JointId
                                  where joint.JointId == JointId && (filter == null || (
                                     (filter.FromDate == null || session.SessionDate.ResetTime() >= filter.FromDate.ResetTime()) &&
                                     (filter.ToDate == null || session.SessionDate.ResetTime() <= filter.ToDate.ResetTime()) &&
                                     (filter.ReservedForUnitId == null || unit.Id == filter.ReservedForUnitId) &&
                                     (filter.ReservedById == null || user.Id == filter.ReservedById))
                                  )
                                  select new Middle_GetReservationReport_DataDTO
                                  {
                                      Count = reservation.Count,
                                      EndTime = reservation.EndDate.ResetTimeSpan(),
                                      GuestCount = reservation.GuestCount,
                                      IsPrivate = session.IsPrivate,
                                      JointTitle = joint.Title,
                                      PublicGenderType = session.IsPrivate ? null : (int?)session.PublicSessionGender,
                                      PublicGenderTypeTitle = session.IsPrivate ? null : session.PublicSessionGender switch
                                      {
                                          null => "All",
                                          GenderType.ALL => "All",
                                          GenderType.MALE => "Male",
                                          GenderType.FAMALE => "Female",
                                          _ => "All"
                                      },
                                      PublicGenderTypeDisplayTitle = session.IsPrivate ? null : session.PublicSessionGender switch
                                      {
                                          null => "همگانی",
                                          GenderType.ALL => "همگانی",
                                          GenderType.MALE => "آقایان",
                                          GenderType.FAMALE => "بانوان",
                                          _ => "همگانی"
                                      },
                                      ReservationDate = reservation.ReservationDate.ResetTime(),
                                      ReservationId = reservation.ReservationId,
                                      ReservedByFullName = $"{user.FirstName} {user.LastName}",
                                      ReservedById = reservation.ReservedById,
                                      ReservedForUnitId = reservation.ReservedForUnitId,
                                      ReservedForUnitName = unit.Name,
                                      SessionDate = session.SessionDate.ResetTime(),
                                      SessionDateDay = session.SessionDate.GetDayOfWeekTitle(),
                                      StartTime = reservation.StartDate.ResetTimeSpan(),
                                      TotalCost = reservation.Cost,
                                      IsActive = !reservation.CancelledByAdmin && !reservation.CancelledByByUser,
                                      StateTitle = (reservation.CancelledByAdmin, reservation.CancelledByByUser) switch
                                      {
                                          (false, false) => "Active",
                                          (true, false) => "CancelledByAdmin",
                                          (false, true) => "CancelledByUser",
                                          _ => "Cancelled",
                                      },
                                      StateDisplayTitle = (reservation.CancelledByAdmin, reservation.CancelledByByUser) switch
                                      {
                                          (false, false) => "فعال",
                                          (true, false) => "لغو شده توسط ادمین",
                                          (false, true) => "لغو شده توسط کاربر",
                                          _ => "لغو شده",
                                      },
                                      CancellationDescription = !reservation.CancelledByAdmin && !reservation.CancelledByByUser ? null : reservation.CancellationDescription

                                  }).OrderByDescending(x => x.SessionDate).ThenByDescending(x => x.StartTime).ToList());
            return Op.Succeed("دریافت اطلاعات گزارش رزرواسیونبا موفقیت انجام شد", result);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات گزارش رزرواسیون با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }
}
