using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.BillDTOs;
using core.domain.DomainModelDTOs.BillDTOs.Filter;
using core.domain.entity.financialModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace core.infrastructure.Data.repository
{
    public class BillRepository : IBillRepository
    {
        private readonly EnjoyLifeContext _context;

        public BillRepository(EnjoyLifeContext context)
        {
            this._context = context;
        }

        public async Task<OperationResult<Response_InitialFilterDataDTO>> GetBillInitialFilterData(CancellationToken cancellationToken = default)
        {
            OperationResult<Response_InitialFilterDataDTO> Op = new("GetBillInitialFilterData");
            try
            {
                var units = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                             select new Middle_BillUnitDTO
                             {
                                 Id = u.Id,
                                 UnitName = u.Name.MultipleSpaceRemoverTrim(),
                             }).ToList();
                if (units == null || !units.Any())
                {
                    return Op.Failed("دریافت اطلاعات واحد ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                var minYear = core.application.Framework.DatetimeHelper.GetCurrentPersianYear() - 10;

                if (await _context.Bills.AnyAsync(cancellationToken: cancellationToken))
                {
                    minYear = (from b in await _context.Bills.ToListAsync(cancellationToken: cancellationToken) select b.Year).Distinct().ToList().Min();
                }
                var years = Enumerable.Range(minYear, (core.application.Framework.DatetimeHelper.GetCurrentPersianYear() + 11) - minYear).Select(x => new Middle_BillYearDTO
                {
                    Year = x,
                    YearTitle = x.ToString(),
                }).ToList();
                if (years == null || !years.Any())
                {
                    return Op.Failed("دریافت اطلاعات سال با مشکل مواجه شده است");
                }
                var months = Enumerable.Range(1, 12).Select(x => new Middle_BillMonthDTO
                {
                    Month = x,
                    MonthTitle = core.application.Framework.DatetimeHelper.GetMonthTitle(x)
                }).ToList();
                if (months == null || !months.Any())
                {
                    return Op.Failed("دریافت اطلاعات ماه با مشکل مواجه شده است");
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", new Response_InitialFilterDataDTO
                {
                    Months = months,
                    Units = units,
                    Years = years,
                });


            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_GetBillTotalReportDTO>> GetBillTotalReport(Filter_GetBillTotalReportDTO? filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetBillTotalReportDTO> Op = new("GetBillTotalReport");
            if (filter != null)
            {
                if (filter.UnitId != null && filter.UnitId <= 0)
                {
                    return Op.Failed("فیلتر شناسه واحد بدرستی ارسال نشده است");
                }
                if (filter.Year != null && (filter.Year < core.application.Framework.DatetimeHelper.GetCurrentPersianYear() - 10 || filter.Year > core.application.Framework.DatetimeHelper.GetCurrentPersianYear() + 10))
                {
                    return Op.Failed("فیلتر سال بدرستی ارسال نشده است");
                }
                if (filter.Months != null && filter.Months.Any(x => x < 1 || x > 12))
                {
                    return Op.Failed("فیلتر ماه بدرستی ارسال نشده است");
                }
            }
            try
            {
                var flatBills = (from b in await _context.Bills.ToListAsync(cancellationToken: cancellationToken)
                                 join mo in (filter != null && filter.Months != null && filter.Months.Any() ? filter.Months.Distinct().ToList() : new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })
                                 on b.Month equals mo
                                 join u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                 on b.UnitId equals u.Id
                                 where filter == null || ((filter.UnitId == null || b.UnitId == filter.UnitId) && (filter.Year == null || b.Year == filter.Year))
                                 select new Middle_BillFlatDTO
                                 {
                                     BillId = b.BillId,
                                     Credit = b.Credit,
                                     Debit = b.Debit,
                                     Description = b.Description,
                                     Month = b.Month,
                                     UnitId = b.UnitId,
                                     UnitName = u.Name.MultipleSpaceRemoverTrim(),
                                     Year = b.Year,
                                 }).OrderBy(x => x.UnitName).ThenByDescending(x => x.Year).ThenBy(x => x.Month).ToList();
                if (flatBills == null || !flatBills.Any())
                {
                    return Op.Succeed("دریافت گزارش کلی صورتحساب ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                List<Middle_GetBillTotalDTO> totalBills = flatBills.GroupBy(x => x.UnitId).Select(x => x.First()).Select(x => new Middle_GetBillTotalDTO
                {
                    UnitId = x.UnitId,
                    UnitName = x.UnitName,
                    Debit = flatBills.Where(y => y.UnitId == x.UnitId).Sum(y => y.Debit),
                    Credit = flatBills.Where(y => y.UnitId == x.UnitId).Sum(y => y.Credit),
                    OutstandingDebt = (flatBills.Where(y => y.UnitId == x.UnitId).Sum(y => y.Debit)) - (flatBills.Where(y => y.UnitId == x.UnitId).Sum(y => y.Credit))
                }).ToList();
                return Op.Succeed("دریافت گزارش کلی صورتحساب ها با موفقیت انجام شد", new Response_GetBillTotalReportDTO
                {
                    TotalBills = totalBills,
                    TotalCredit = totalBills.Sum(x => x.Credit),
                    TotalDebit = totalBills.Sum(x => x.Debit)
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت گزارش کلی صورتحساب ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_GetBillDetailReportDTO>> GetBillDetailReport(int UnitId, Filter_GetBillDetailReportDTO? filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetBillDetailReportDTO> Op = new("GetBillDetailReport");
            if (UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (filter != null)
            {
                if (filter.Year != null && (filter.Year < core.application.Framework.DatetimeHelper.GetCurrentPersianYear() - 10 || filter.Year > core.application.Framework.DatetimeHelper.GetCurrentPersianYear() + 10))
                {
                    return Op.Failed("فیلتر سال بدرستی ارسال نشده است");
                }
                if (filter.Months != null && filter.Months.Any(x => x < 1 || x > 12))
                {
                    return Op.Failed("فیلتر ماه بدرستی ارسال نشده است");
                }
            }
            try
            {
                if (!await _context.Units.AnyAsync(x => x.Id == UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                var bills = (from b in await _context.Bills.ToListAsync(cancellationToken: cancellationToken)
                             join mo in (filter != null && filter.Months != null && filter.Months.Any() ? filter.Months.Distinct().ToList() : new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })
                             on b.Month equals mo
                             join u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                             on b.UnitId equals u.Id
                             where b.UnitId == UnitId && (filter == null || filter.Year == null || b.Year == filter.Year)
                             select new Middle_BillFlatDTO
                             {
                                 BillId = b.BillId,
                                 Credit = b.Credit,
                                 Debit = b.Debit,
                                 Description = b.Description,
                                 Month = b.Month,
                                 UnitId = b.UnitId,
                                 UnitName = u.Name.MultipleSpaceRemoverTrim(),
                                 Year = b.Year,
                             }).OrderBy(x => x.UnitName).ThenByDescending(x => x.Year).ThenBy(x => x.Month).ToList();
                if (bills == null || !bills.Any())
                {
                    return Op.Succeed("دریافت گزارش جزییات صورتحساب ها به تفکیک واحد با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت گزارش جزییات صورتحساب ها به تفکیک واحد با موفقیت انجام شد", new Response_GetBillDetailReportDTO
                {
                    Bills = bills.Select(x => new Middle_GetBillDetailDTO
                    {
                        Credit = x.Credit,
                        Debit = x.Debit,
                        Description = x.Description,
                        Month = x.Month,
                        MonthTitle = core.application.Framework.DatetimeHelper.GetMonthTitle(x.Month),
                        Year = x.Year,
                    }).ToList(),
                    SumCredit = bills.Sum(x => x.Credit),
                    SumDebit = bills.Sum(x => x.Debit),
                    UnitId = bills.First().UnitId,
                    UnitName = bills.First().UnitName,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت گزارش جزییات صورتحساب ها به تفکیک واحد با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_ModifyListBillDTO>> ModifyBillsByExcelFile(int ModifierId, List<Request_ModifyListBillDTO> model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_ModifyListBillDTO> Op = new("ModifyBillsByExcelFile");
            if (ModifierId <= 0)
            {
                return Op.Failed("شناسه ثبت کننده اطلاعات بدرستی ارسال نشده است");
            }
            if (model == null || !model.Any())
            {
                return Op.Failed("اطلاعاتی جهت بروزرسانی صورت حساب ها ارسال نشده است");
            }
            if (model.Any(x => x.RowNumber <= 0))
            {
                return Op.Failed($"شماره ردیف بدرستی ارسال نشده است، خطا در ردیف : {(model.FindIndex(x => x.RowNumber <= 0) + 1)}");
            }

            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == ModifierId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ثبت کننده اطلاعات بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var units = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                             select new Middle_BillUnitDTO
                             {
                                 Id = u.Id,
                                 UnitName = u.Name.MultipleSpaceRemoverTrim()
                             }).ToList();
                if (units == null || !units.Any())
                {
                    return Op.Failed("دریافت اطلاعات واحد ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }

                List<Response_ModifyListBillDTO> responseList = new();
                for (int i = 0; i < model.Count; i++)
                {
                    var request = model[i];
                    var response = new Response_ModifyListBillDTO
                    {
                        RowNumber = request.RowNumber,
                        Success = false,
                        Messages = new List<string>()
                    };

                    if (request.Year < 1370 || request.Year > (core.application.Framework.DatetimeHelper.GetCurrentPersianYear() + 10))
                    {
                        response.Messages.Add("سال بدرستی ارسال نشده است");
                    }
                    if (request.Month < 1 || request.Month > 12)
                    {
                        response.Messages.Add("ماه بدرستی ارسال نشده است");
                    }
                    if (request.Debit < 0)
                    {
                        response.Messages.Add("مبلغ بدهکاری بدرستی ارسال نشده است");
                    }
                    if (request.Credit < 0)
                    {
                        response.Messages.Add("مبلغ بستانکاری بدرستی ارسال نشده است");
                    }
                    if (request.Debit > 0 && request.Credit > 0)
                    {
                        response.Messages.Add("مبلغ بدهکاری یا بستانکاری بدرستی ارسال نشده است");
                    }
                    if (!request.Description.IsNotNull())
                    {
                        response.Messages.Add("ارسال شرح اجباری است");
                    }
                    if (!request.UnitName.IsNotNull())
                    {
                        response.Messages.Add("ارسال نام واحد اجباری است");
                    }
                    var fromDbUnit = units.FirstOrDefault(x => x.UnitName.ToLower() == request.UnitName.ToLower().MultipleSpaceRemoverTrim());
                    if (fromDbUnit == null)
                    {
                        response.Messages.Add("نام واحد بدرستی ارسال نشده است");
                    }

                    if (!response.Messages.Any())
                    {
                        if (await _context.Bills.AnyAsync(x => x.Year == request.Year && x.Month == request.Month && x.UnitId == fromDbUnit.Id && x.ModificationDate != Op.OperationDate, cancellationToken: cancellationToken))
                        {
                            _context.Bills.RemoveRange(await _context.Bills.Where(x => x.Year == request.Year && x.Month == request.Month && x.UnitId == fromDbUnit.Id && x.ModificationDate != Op.OperationDate).ToListAsync(cancellationToken: cancellationToken));
                        }
                        await _context.Bills.AddAsync(new BillModel
                        {
                            Credit = request.Credit,
                            Debit = request.Debit,
                            Description = request.Description.MultipleSpaceRemoverTrim(),
                            ModificationDate = Op.OperationDate,
                            Modifier = ModifierId,
                            Month = request.Month,
                            UnitId = units.First(x => x.UnitName.ToLower() == request.UnitName.ToLower().MultipleSpaceRemoverTrim()).Id,
                            Year = request.Year,
                        }, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        response.Success = true;
                        response.Messages = null;
                    }

                    responseList.Add(response);
                }

                return Op.Succeed("عملیات با موفقیت انجام شد", responseList);
            }
            catch (Exception ex)
            {
                return Op.Failed("عملیات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
