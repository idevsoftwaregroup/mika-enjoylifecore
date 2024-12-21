using Microsoft.EntityFrameworkCore;
using Mika.Domain.Contracts.DTOs.Account;
using Mika.Domain.Contracts.DTOs.Account.Filter;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Contracts.DTOs.Company.Filter;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Domain.Contracts.DTOs.Modules.Middle;
using Mika.Domain.Contracts.DTOs.Role;
using Mika.Domain.Entities;
using Mika.Domain.Entities.RelationalEntities;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Framework.Utilities;
using Mika.Infastructure.Data;
using Mika.Infastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Infastructure.Services.Implementations
{
    public class SettingRepository : ISettingRepository
    {
        private readonly Context _context;
        public SettingRepository(Context context)
        {
            this._context = context;
        }


        public async Task<OperationResult<object>> CreateCompany(long UserId, Request_CreateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateCompany");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت شرکت ارسال نشده است");
            }
            if (!model.CompanyNumber.IsNotNull())
            {
                return Op.Failed("ارسال کد شرکت اجباری است");
            }
            if (model.CompanyNumber.Contains(' '))
            {
                return Op.Failed("ارسال کد شرکت با 'فاصله' مجاز نمیباشد است");
            }
            if (model.CompanyNumber.Length > 28)
            {
                return Op.Failed($"حداکثر کاراکتر مجاز کد شرکت {28.ToPersianNumber()} میباشد");
            }
            if (!model.CompanyCode.IsNotNull())
            {
                return Op.Failed("ارسال کد اختصاری شرکت اجباری است");
            }
            if (model.CompanyCode.MultipleSpaceRemoverTrim().Length > 18)
            {
                return Op.Failed($"تعداد کاراکتر مجاز کد اختصاری شرکت از {1.ToPersianNumber()} تا {18.ToPersianNumber()} کاراکتر میباشد");
            }
            if (!model.Name.IsNotNull())
            {
                return Op.Failed("ارسال نام شرکت اجباری است");
            }
            if (model.Name.MultipleSpaceRemoverTrim().Length < 2 || model.Name.MultipleSpaceRemoverTrim().Length > 145)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام شرکت از {2.ToPersianNumber()} تا {145.ToPersianNumber()} کاراکتر میباشد");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (await _context.Companies.AnyAsync(x => x.CompanyCode.ToLower() == model.CompanyCode.MultipleSpaceRemoverTrim().ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت شرکت با کد تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (await _context.Companies.AnyAsync(x => x.Name.ToLower() == model.Name.MultipleSpaceRemoverTrim().ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت شرکت با نام تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                await _context.Companies.AddAsync(new Domain.Entities.Company
                {
                    CompanyCode = model.CompanyCode.MultipleSpaceRemoverTrim(),
                    CompanyNumber = model.CompanyNumber.ToEnglishNumber(),
                    CreationDate = Op.OperationDate,
                    Creator = UserId,
                    Name = model.Name.MultipleSpaceRemoverTrim()
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت شرکت با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت شرکت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> UpdateCompany(long UserId, Request_UpdateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateCompany");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null || (!model.CompanyCode.IsNotNull() && !model.Name.IsNotNull() && !model.CompanyNumber.IsNotNull()))
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی شرکت ارسال نشده است");
            }
            if (model.CompanyId <= 0)
            {
                return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
            }
            if (model.CompanyNumber.IsNotNull())
            {
                if (model.CompanyNumber.Contains(' '))
                {
                    return Op.Failed("ارسال کد شرکت با 'فاصله' مجاز نمیباشد است");
                }
                if (model.CompanyNumber.Length > 28)
                {
                    return Op.Failed($"حداکثر کاراکتر مجاز کد شرکت {28.ToPersianNumber()} میباشد");
                }
            }
            if (model.CompanyCode.IsNotNull() && model.CompanyCode.MultipleSpaceRemoverTrim().Length > 18)
            {
                return Op.Failed($"تعداد کاراکتر مجاز کد اختصاری شرکت از {1.ToPersianNumber()} تا {18.ToPersianNumber()} کاراکتر میباشد");
            }
            if (model.Name.IsNotNull() && (model.Name.MultipleSpaceRemoverTrim().Length < 2 || model.Name.MultipleSpaceRemoverTrim().Length > 145))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام شرکت از {2.ToPersianNumber()} تا {145.ToPersianNumber()} کاراکتر میباشد");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var company = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyId == model.CompanyId, cancellationToken: cancellationToken);
                if (company == null)
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.CompanyNumber.IsNotNull() && await _context.Companies.AnyAsync(x => x.CompanyNumber.ToLower() == model.CompanyNumber.ToEnglishNumber().ToLower() && x.CompanyId != company.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("بروزرسانی شرکت با کد تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.CompanyCode.IsNotNull() && await _context.Companies.AnyAsync(x => x.CompanyCode.ToLower() == model.CompanyCode.MultipleSpaceRemoverTrim().ToLower() && x.CompanyId != company.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("بروزرسانی شرکت با کد اختصاری تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.Name.IsNotNull() && await _context.Companies.AnyAsync(x => x.Name.ToLower() == model.Name.MultipleSpaceRemoverTrim().ToLower() && x.CompanyId != company.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("بروزرسانی شرکت با نام تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.CompanyNumber.IsNotNull())
                {
                    company.CompanyNumber = model.CompanyNumber.ToEnglishNumber();
                }
                if (model.CompanyCode.IsNotNull())
                {
                    company.CompanyCode = model.CompanyCode.MultipleSpaceRemoverTrim();
                }
                if (model.Name.IsNotNull())
                {
                    company.Name = model.Name.MultipleSpaceRemoverTrim();
                }
                company.LastModificationDate = Op.OperationDate;
                company.LastModifier = UserId;
                _context.Entry<Company>(company).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی اطلاعات شرکت با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات شرکت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> DeleteCompany(long UserId, Request_DeleteCompanyDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteCompany");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model.CompanyId <= 0)
            {
                return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var company = await _context.Companies.Include(x => x.Accounts).ThenInclude(x => x.BiDataEntries).FirstOrDefaultAsync(x => x.CompanyId == model.CompanyId, cancellationToken: cancellationToken);
                if (company == null)
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (company.Accounts != null && company.Accounts.Any(x => x.BiDataEntries != null && x.BiDataEntries.Any()))
                {
                    return Op.Failed("بعلت وجود اطلاعات مالی (BI) مربوط به حساب های شرکت ، امکان حذف وجود ندارد", HttpStatusCode.Forbidden);
                }
                _context.F_UserAccounts.RemoveRange((from f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                                     join account in await _context.Accounts.ToListAsync(cancellationToken: cancellationToken)
                                                     on f_userAccount.AccountId equals account.AccountId
                                                     where account.CompanyId == company.CompanyId
                                                     select f_userAccount).ToList());
                _context.Accounts.RemoveRange(await _context.Accounts.Where(x => x.CompanyId == company.CompanyId).ToListAsync(cancellationToken: cancellationToken));
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف شرکت با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف شرکت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> UpdateModule(long UserId, Request_UpdateModuleDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateModule");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ماژول ارسال نشده است");
            }
            if (model.ModuleId <= 0)
            {
                return Op.Failed("شناسه ماژول بدرستی ارسال نشده است");
            }
            if (!model.DisplayName.IsNotNull() && (model.SubModules == null || !model.SubModules.Any()))
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ماژول ارسال نشده است");
            }
            if (model.DisplayName.IsNotNull() && (model.DisplayName.MultipleSpaceRemoverTrim().Length < 2 || model.DisplayName.MultipleSpaceRemoverTrim().Length > 47))
            {
                return Op.Failed($"تعداد کاراکتر مجاز برای عنوان نمایش ماژول از {2.ToPersianNumber()} تا {47.ToPersianNumber()} کاراکتر است");
            }
            if (model.SubModules != null && model.SubModules.Any())
            {
                if (model.SubModules.Any(x => x.SubModuleId <= 0))
                {
                    return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است، خطا در ردیف : {model.SubModules.FindIndex(x => x.SubModuleId <= 0) + 1}");
                }
                if (model.SubModules.Any(x => !x.SubModuleDisplayName.IsNotNull()))
                {
                    return Op.Failed($"عنوان نمایش دسترسی ماژول بدرستی ارسال نشده است، خطا در ردیف : {model.SubModules.FindIndex(x => !x.SubModuleDisplayName.IsNotNull()) + 1}");
                }
                if (model.SubModules.Any(x => x.SubModuleDisplayName.MultipleSpaceRemoverTrim().Length < 2 || x.SubModuleDisplayName.MultipleSpaceRemoverTrim().Length > 47))
                {
                    return Op.Failed($"تعداد کاراکتر مجاز برای عنوان نمایش دسترسی ماژول از {2.ToPersianNumber()} تا {47.ToPersianNumber()} کاراکتر است، خطا در ردیف : {model.SubModules.FindIndex(x => x.SubModuleDisplayName.MultipleSpaceRemoverTrim().Length < 2 || x.SubModuleDisplayName.MultipleSpaceRemoverTrim().Length > 47) + 1}");
                }
                if (model.SubModules.Select(x => x.SubModuleDisplayName.MultipleSpaceRemoverTrim().ToLower()).ToList().Count != model.SubModules.Select(x => x.SubModuleDisplayName.MultipleSpaceRemoverTrim().ToLower()).Distinct().ToList().Count)
                {
                    return Op.Failed("امکان بروزرسانی عنوان نمایش دسترسی ماژول تکراری وجود ندارد", HttpStatusCode.Conflict);
                }
            }
            try
            {
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN")
                {
                    return Op.Failed("دسترسی بروزرسانی ماژول برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var module = await _context.Modules.FirstOrDefaultAsync(x => x.ModuleId == model.ModuleId, cancellationToken: cancellationToken);
                if (module == null)
                {
                    return Op.Failed("شناسه ماژول بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.DisplayName.IsNotNull() && await _context.Modules.AnyAsync(x => x.DisplayName.ToLower() == model.DisplayName.MultipleSpaceRemoverTrim().ToLower() && x.ModuleId != model.ModuleId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("بروزرسانی عنوان نمایش ماژول تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.DisplayName.IsNotNull())
                {
                    module.DisplayName = model.DisplayName.MultipleSpaceRemoverTrim();
                    _context.Entry<Module>(module).State = EntityState.Modified;
                }
                if (model.SubModules != null && model.SubModules.Any())
                {
                    for (int i = 0; i < model.SubModules.Count; i++)
                    {
                        if (!await _context.SubModules.Where(x => x.ModuleId == model.ModuleId).AnyAsync(x => x.SubModuleId == model.SubModules[i].SubModuleId, cancellationToken: cancellationToken))
                        {
                            return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است، خطا در ردیف : {i + 1}", HttpStatusCode.NotFound);
                        }
                        if (await _context.SubModules.Where(x => x.ModuleId == model.ModuleId).AnyAsync(x => x.SubDisplayName.ToLower() == model.SubModules[i].SubModuleDisplayName.MultipleSpaceRemoverTrim().ToLower() && x.SubModuleId != model.SubModules[i].SubModuleId, cancellationToken: cancellationToken))
                        {
                            return Op.Failed($"بروزرسانی عنوان نمایش دسترسی ماژول تکراری امکانپذیر نمیباشد، خطا در ردیف : {i + 1}", HttpStatusCode.NotFound);
                        }
                        var subModule = await _context.SubModules.Where(x => x.ModuleId == model.ModuleId).FirstOrDefaultAsync(x => x.SubModuleId == model.SubModules[i].SubModuleId, cancellationToken: cancellationToken);
                        if (subModule == null)
                        {
                            return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.NotFound);
                        }
                        subModule.SubDisplayName = model.SubModules[i].SubModuleDisplayName.MultipleSpaceRemoverTrim();
                        _context.Entry<SubModule>(subModule).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی ماژول با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی مازول با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }
        public async Task<OperationResult<Response_GetModuleDTO>> GetModules(long UserId, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetModuleDTO> Op = new("GetModules");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            try
            {
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "SUPERADMIN")
                {
                    return Op.Succeed("دریافت لیست ماژول ها با موفقیت انجام شد", (from module in await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken)
                                                                                  select new Response_GetModuleDTO
                                                                                  {
                                                                                      DisplayName = module.DisplayName,
                                                                                      ModuleId = module.ModuleId,
                                                                                      ModuleName = module.ModuleName,
                                                                                      SubModules = module.SubModules.Select(x => new Middle_SubModuleDTO
                                                                                      {
                                                                                          SubModuleDisplayName = x.SubDisplayName,
                                                                                          SubModuleId = x.SubModuleId,
                                                                                          SubModuleName = x.SubModuleName,
                                                                                      }).ToList()
                                                                                  }).ToList());
                }
                var result = new List<Response_GetModuleDTO>();
                var subModules = (from f_subModule in await _context.F_UserSubModules.ToListAsync(cancellationToken: cancellationToken)
                                  join subModule in await _context.SubModules.Include(x => x.Module).ToListAsync(cancellationToken: cancellationToken)
                                  on f_subModule.SubModuleId equals subModule.SubModuleId
                                  where f_subModule.UserId == UserId
                                  select subModule).ToList();
                if (subModules == null || !subModules.Any())
                {
                    return Op.Succeed("دریافت لیست ماژول ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                for (int i = 0; i < subModules.Count; i++)
                {
                    var moduleInList = result.FirstOrDefault(x => x.ModuleId == subModules[i].ModuleId);
                    if (moduleInList == null)
                    {
                        result.Add(new Response_GetModuleDTO
                        {
                            DisplayName = subModules[i].Module.DisplayName,
                            ModuleId = subModules[i].ModuleId,
                            ModuleName = subModules[i].Module.ModuleName,
                            SubModules = new List<Middle_SubModuleDTO>() { new() {
                                SubModuleDisplayName = subModules[i].SubDisplayName,
                                SubModuleId = subModules[i].SubModuleId,
                                SubModuleName= subModules[i].SubModuleName,
                            }}
                        });
                    }
                    else
                    {
                        moduleInList.SubModules.Add(new Middle_SubModuleDTO
                        {
                            SubModuleDisplayName = subModules[i].SubDisplayName,
                            SubModuleId = subModules[i].SubModuleId,
                            SubModuleName = subModules[i].SubModuleName,
                        });
                    }
                }
                return Op.Succeed("دریافت لیست ماژول ها با موفقیت انجام شد", result);
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست ماژول ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<PagedList<Respone_GetCompanyDTO>>> GetCompanies(long UserId, Filter_GetCompanyDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            OperationResult<PagedList<Respone_GetCompanyDTO>> Op = new("GetCompanies");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (filter != null)
            {
                if (filter.CompanyId != null && filter.CompanyId <= 0)
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
                }
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }

                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                long totalCount = 0;
                var result = new List<Respone_GetCompanyDTO>();
                totalCount = Convert.ToInt64((from company in await _context.Companies.ToListAsync(cancellationToken: cancellationToken)
                                              where filter == null || (
                                                   (filter.CompanyId == null || company.CompanyId == filter.CompanyId) &&
                                                   (!filter.Search.IsNotNull() || company.CompanyNumber.Contains(filter.Search.Trim().ToEnglishNumber()) || company.CompanyCode.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower()) || company.Name.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower())) &&
                                                   (filter.CreationDate == null || company.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                                )
                                              select company.CompanyId
                                      ).ToList().Count);
                if (totalCount == 0)
                {
                    return Op.Succeed("دریافت لیست شرکت ها با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                if (page == null || page.Contain == null || page.Contain <= 0)
                {
                    result = (from company in await _context.Companies.ToListAsync(cancellationToken: cancellationToken)
                              where filter == null || (
                                   (filter.CompanyId == null || company.CompanyId == filter.CompanyId) &&
                                   (!filter.Search.IsNotNull() || company.CompanyNumber.Contains(filter.Search.Trim().ToEnglishNumber()) || company.CompanyCode.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower()) || company.Name.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower())) &&
                                   (filter.CreationDate == null || company.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                )
                              select new Respone_GetCompanyDTO
                              {
                                  CompanyCode = company.CompanyCode,
                                  CompanyId = company.CompanyId,
                                  CompanyNumber = company.CompanyNumber,
                                  CreationDate = company.CreationDate,
                                  LastModificationDate = company.LastModificationDate,
                                  Name = company.Name
                              }).OrderByDescending(x => x.CreationDate).ToList();
                }
                else
                {
                    if (page.Number == null || page.Number <= 0)
                    {
                        page.Number = 1;
                    }
                    result = (from company in await _context.Companies.ToListAsync(cancellationToken: cancellationToken)
                              where filter == null || (
                                   (filter.CompanyId == null || company.CompanyId == filter.CompanyId) &&
                                   (!filter.Search.IsNotNull() || company.CompanyNumber.Contains(filter.Search.Trim().ToEnglishNumber()) || company.CompanyCode.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower()) || company.Name.ToLower().Contains(filter.Search.Trim().ToEnglishNumber().ToLower())) &&
                                   (filter.CreationDate == null || company.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                )
                              select new Respone_GetCompanyDTO
                              {
                                  CompanyCode = company.CompanyCode,
                                  CompanyId = company.CompanyId,
                                  CompanyNumber = company.CompanyNumber,
                                  CreationDate = company.CreationDate,
                                  LastModificationDate = company.LastModificationDate,
                                  Name = company.Name
                              }).OrderByDescending(x => x.CreationDate).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                }
                return Op.Succeed("دریافت لیست شرکت ها با موفقیت انجام شد", new PagedList<Respone_GetCompanyDTO>(result, totalCount, page));
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست شرکت ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<Respone_GetCompanyDTO>> GetCompany(long UserId, long CompanyId, CancellationToken cancellationToken = default)
        {
            OperationResult<Respone_GetCompanyDTO> Op = new("GetCompany");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (CompanyId <= 0)
            {
                return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "SUPERADMIN" || user.Role.RoleName.ToUpper() == "ADMIN")
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyId == CompanyId, cancellationToken: cancellationToken);
                    if (company == null)
                    {
                        return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    return Op.Succeed("دریافت اطلاعات شرکت با موفقیت انجام شد", new Respone_GetCompanyDTO
                    {
                        CompanyCode = company.CompanyCode,
                        CompanyId = company.CompanyId,
                        CompanyNumber = company.CompanyNumber,
                        CreationDate = company.CreationDate,
                        LastModificationDate = company.LastModificationDate,
                        Name = company.Name,
                    });
                }
                else
                {
                    var company = (from c in await _context.Companies.ToListAsync(cancellationToken: cancellationToken)
                                   join account in await _context.Accounts.ToListAsync(cancellationToken: cancellationToken)
                                   on c.CompanyId equals account.CompanyId
                                   join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                   on account.AccountId equals f_userAccount.AccountId
                                   where f_userAccount.UserId == UserId && c.CompanyId == CompanyId
                                   select c).FirstOrDefault();
                    if (company == null)
                    {
                        return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    return Op.Succeed("دریافت اطلاعات شرکت با موفقیت انجام شد", new Respone_GetCompanyDTO
                    {
                        CompanyCode = company.CompanyCode,
                        CompanyId = company.CompanyId,
                        CompanyNumber = company.CompanyNumber,
                        CreationDate = company.CreationDate,
                        LastModificationDate = company.LastModificationDate,
                        Name = company.Name,
                    });
                }
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات شرکت با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<PagedList<Response_GetAccountDTO>>> GetAccounts(long UserId, Filter_GetAccountDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            OperationResult<PagedList<Response_GetAccountDTO>> Op = new("GetAccounts");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (filter != null)
            {
                if (filter.AccountId <= 0)
                {
                    return Op.Failed("شناسه حساب بدرستی ارسال نشده است");
                }
                if (filter.CompanyId <= 0)
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
                }
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if ((user.Role.RoleName.ToUpper() == "ADMIN" || user.Role.RoleName.ToUpper() == "USER") && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                long totalCount = 0;
                var result = new List<Response_GetAccountDTO>();
                var safeSearchString = filter.Search.IsNotNull() ? filter.Search.MultipleSpaceRemoverTrim().ToEnglishNumber().ToLower() : null;
                if (user.Role.RoleName.ToUpper() == "SUPERADMIN" || user.Role.RoleName.ToUpper() == "ADMIN")
                {
                    totalCount = Convert.ToInt64((from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                                  where (filter == null ||
                                                             (
                                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                                             )
                                                        )
                                                  select account.AccountId
                                             ).ToList().Count);
                    if (totalCount == 0)
                    {
                        return Op.Succeed("دریافت لیست حساب ها با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                    }
                    if (page == null || page.Contain == null || page.Contain <= 0)
                    {
                        result = (from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  where (filter == null ||
                                             (
                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                             )
                                        )
                                  select new Response_GetAccountDTO
                                  {
                                      AccountId = account.AccountId,
                                      AccountNumber = account.AccountNumber,
                                      AccountType = account.AccountType,
                                      BankName = account.BankName,
                                      BelongsToCentralOffice = account.BelongsToCentralOffice,
                                      CompanyCode = account.Company.CompanyCode,
                                      CompanyId = account.CompanyId,
                                      CompanyName = account.Company.Name,
                                      CompanyNumber = account.Company.CompanyNumber,
                                      CreationDate = account.CreationDate,
                                      Description = account.Description,
                                      InterestRatePercent = account.InterestRatePercent,
                                      LastModificationDate = account.LastModificationDate,
                                      ProjectName = account.ProjectName,
                                  }).OrderBy(x => x.CompanyId).ThenByDescending(x => x.CreationDate).ToList();
                    }
                    else
                    {
                        if (page.Number == null || page.Number <= 0)
                        {
                            page.Number = 1;
                        }
                        result = (from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  where (filter == null ||
                                             (
                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                             )
                                        )
                                  select new Response_GetAccountDTO
                                  {
                                      AccountId = account.AccountId,
                                      AccountNumber = account.AccountNumber,
                                      AccountType = account.AccountType,
                                      BankName = account.BankName,
                                      BelongsToCentralOffice = account.BelongsToCentralOffice,
                                      CompanyCode = account.Company.CompanyCode,
                                      CompanyId = account.CompanyId,
                                      CompanyName = account.Company.Name,
                                      CompanyNumber = account.Company.CompanyNumber,
                                      CreationDate = account.CreationDate,
                                      Description = account.Description,
                                      InterestRatePercent = account.InterestRatePercent,
                                      LastModificationDate = account.LastModificationDate,
                                      ProjectName = account.ProjectName,
                                  }).OrderBy(x => x.CompanyId).ThenByDescending(x => x.CreationDate).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                    }
                }
                else
                {
                    totalCount = Convert.ToInt64((from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                                  on account.AccountId equals f_userAccount.AccountId
                                                  where (f_userAccount.UserId == user.UserId) && (filter == null ||
                                                             (
                                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                                             )
                                                        )
                                                  select account.AccountId
                                             ).ToList().Count);
                    if (totalCount == 0)
                    {
                        return Op.Succeed("دریافت لیست حساب ها با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                    }
                    if (page == null || page.Contain == null || page.Contain <= 0)
                    {
                        result = (from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                  on account.AccountId equals f_userAccount.AccountId
                                  where (f_userAccount.UserId == user.UserId) && (filter == null ||
                                             (
                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                             )
                                        )
                                  select new Response_GetAccountDTO
                                  {
                                      AccountId = account.AccountId,
                                      AccountNumber = account.AccountNumber,
                                      AccountType = account.AccountType,
                                      BankName = account.BankName,
                                      BelongsToCentralOffice = account.BelongsToCentralOffice,
                                      CompanyCode = account.Company.CompanyCode,
                                      CompanyId = account.CompanyId,
                                      CompanyName = account.Company.Name,
                                      CompanyNumber = account.Company.CompanyNumber,
                                      CreationDate = account.CreationDate,
                                      Description = account.Description,
                                      InterestRatePercent = account.InterestRatePercent,
                                      LastModificationDate = account.LastModificationDate,
                                      ProjectName = account.ProjectName,
                                  }).OrderBy(x => x.CompanyId).ThenByDescending(x => x.CreationDate).ToList();
                    }
                    else
                    {
                        if (page.Number == null || page.Number <= 0)
                        {
                            page.Number = 1;
                        }
                        result = (from account in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                  on account.AccountId equals f_userAccount.AccountId
                                  where (f_userAccount.UserId == user.UserId) && (filter == null ||
                                             (
                                             (filter.AccountId == null || account.AccountId == filter.AccountId) &&
                                             (!safeSearchString.IsNotNull() || account.AccountNumber.ToLower().Contains(safeSearchString) || account.BankName.ToLower().Contains(safeSearchString) || (account.AccountType.IsNotNull() ? account.AccountType.ToLower() : "").Contains(safeSearchString) || account.ProjectName.ToLower().Contains(safeSearchString) || (account.Description.IsNotNull() ? account.Description.ToLower() : "").Contains(safeSearchString) || account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || account.Company.CompanyCode.ToLower().Contains(safeSearchString) || account.Company.Name.ToLower().Contains(safeSearchString)) &&
                                             (filter.BelongsToCentralOffice == null || account.BelongsToCentralOffice != null && account.BelongsToCentralOffice == filter.BelongsToCentralOffice) &&
                                             (filter.CompanyId == null || account.CompanyId == filter.CompanyId) &&
                                             (filter.CreationDate == null || account.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                             )
                                        )
                                  select new Response_GetAccountDTO
                                  {
                                      AccountId = account.AccountId,
                                      AccountNumber = account.AccountNumber,
                                      AccountType = account.AccountType,
                                      BankName = account.BankName,
                                      BelongsToCentralOffice = account.BelongsToCentralOffice,
                                      CompanyCode = account.Company.CompanyCode,
                                      CompanyId = account.CompanyId,
                                      CompanyName = account.Company.Name,
                                      CompanyNumber = account.Company.CompanyNumber,
                                      CreationDate = account.CreationDate,
                                      Description = account.Description,
                                      InterestRatePercent = account.InterestRatePercent,
                                      LastModificationDate = account.LastModificationDate,
                                      ProjectName = account.ProjectName,
                                  }).OrderBy(x => x.CompanyId).ThenByDescending(x => x.CreationDate).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                    }
                }
                return Op.Succeed("دریافت لیست حساب ها با موفقیت انجام شد", new PagedList<Response_GetAccountDTO>(result, totalCount, page));
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست حساب ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> CreateAccount(long UserId, Request_CreateAccountDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateAccount");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت حساب جدید ارسال نشده است");
            }
            if (!model.AccountNumber.IsNotNull())
            {
                return Op.Failed("ارسال شماره حساب اجباری است");
            }
            if (model.AccountNumber.MultipleSpaceRemoverTrim().Length < 2 || model.AccountNumber.MultipleSpaceRemoverTrim().Length > 65)
            {
                return Op.Failed($"تعداد کاراکتر مجاز شماره حساب از {2.ToPersianNumber()} تا {65.ToPersianNumber()} کاراکتر است");
            }
            if (!model.BankName.IsNotNull())
            {
                return Op.Failed("ارسال نام بانک اجباری است");
            }
            if (model.BankName.MultipleSpaceRemoverTrim().Length < 2 || model.BankName.MultipleSpaceRemoverTrim().Length > 95)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام بانک از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.AccountType.IsNotNull() && (model.AccountType.MultipleSpaceRemoverTrim().Length < 2 || model.AccountType.MultipleSpaceRemoverTrim().Length > 95))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نوع حساب از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.InterestRatePercent < 0 || model.InterestRatePercent > 100)
            {
                return Op.Failed("درصد نرخ سود بدرستی ارسال نشده است");
            }
            if (model.ProjectName.IsNotNull() && (model.ProjectName.MultipleSpaceRemoverTrim().Length < 2 || model.ProjectName.MultipleSpaceRemoverTrim().Length > 95))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام پروژه از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 3 || model.Description.MultipleSpaceRemoverTrim().Length > 1995))
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {3.ToPersianNumber()} تا {1995.ToPersianNumber()} کاراکتر است");
            }
            if (model.CompanyId <= 0)
            {
                return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if ((user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN") || (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId)))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (!await _context.Companies.AnyAsync(x => x.CompanyId == model.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (await _context.Accounts.AnyAsync(x => x.AccountNumber.ToLower() == model.AccountNumber.MultipleSpaceRemoverTrim().ToEnglishNumber().ToLower() && x.CompanyId == model.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت حساب با شماره حساب تکراری وجود ندارد", HttpStatusCode.Conflict);
                }
                await _context.Accounts.AddAsync(new Account
                {
                    AccountNumber = model.AccountNumber.MultipleSpaceRemoverTrim().ToEnglishNumber(),
                    AccountType = model.AccountType.IsNotNull() ? model.AccountType.MultipleSpaceRemoverTrim() : null,
                    BankName = model.BankName.MultipleSpaceRemoverTrim(),
                    BelongsToCentralOffice = model.BelongsToCentralOffice == true,
                    CompanyId = model.CompanyId,
                    CreationDate = Op.OperationDate,
                    Creator = UserId,
                    Description = model.Description.IsNotNull() ? model.Description.MultipleSpaceRemoverTrim() : null,
                    InterestRatePercent = model.InterestRatePercent,
                    ProjectName = model.ProjectName.IsNotNull() ? model.ProjectName.MultipleSpaceRemoverTrim() : string.Empty,
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت حساب با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت حساب با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> UpdateAccount(long UserId, Request_UpdateAccountDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateAccount");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی حساب ارسال نشده است");
            }
            if (model.AccountId <= 0)
            {
                return Op.Failed("شناسه حساب بدرستی ارسال نشده است");
            }
            if (
                (!model.AccountNumber.IsNotNull()) &&
                (!model.BankName.IsNotNull()) &&
                (!model.AccountType.IsNotNull()) &&
                (model.InterestRatePercent == null) &&
                (!model.ProjectName.IsNotNull()) &&
                (model.BelongsToCentralOffice == null) &&
                (!model.Description.IsNotNull()) &&
                (model.CompanyId == null)
              )
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی حساب ارسال نشده است");
            }
            if (model.AccountNumber.IsNotNull() && (model.AccountNumber.MultipleSpaceRemoverTrim().Length < 2 || model.AccountNumber.MultipleSpaceRemoverTrim().Length > 65))
            {
                return Op.Failed($"تعداد کاراکتر مجاز شماره حساب از {2.ToPersianNumber()} تا {65.ToPersianNumber()} کاراکتر است");
            }
            if (model.BankName.IsNotNull() && (model.BankName.MultipleSpaceRemoverTrim().Length < 2 || model.BankName.MultipleSpaceRemoverTrim().Length > 95))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام بانک از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.AccountType.IsNotNull() && (model.AccountType.MultipleSpaceRemoverTrim().Length < 2 || model.AccountType.MultipleSpaceRemoverTrim().Length > 95))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نوع حساب از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.InterestRatePercent != null && (model.InterestRatePercent < 0 || model.InterestRatePercent > 100))
            {
                return Op.Failed("درصد نرخ سود بدرستی ارسال نشده است");
            }
            if (model.ProjectName.IsNotNull() && (model.ProjectName.MultipleSpaceRemoverTrim().Length < 2 || model.ProjectName.MultipleSpaceRemoverTrim().Length > 95))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام پروژه از {2.ToPersianNumber()} تا {95.ToPersianNumber()} کاراکتر است");
            }
            if (model.Description.IsNotNull() && (model.Description.MultipleSpaceRemoverTrim().Length < 3 || model.Description.MultipleSpaceRemoverTrim().Length > 1995))
            {
                return Op.Failed($"تعداد کاراکتر مجاز توضیحات از {3.ToPersianNumber()} تا {1995.ToPersianNumber()} کاراکتر است");
            }
            if (model.CompanyId != null && model.CompanyId <= 0)
            {
                return Op.Failed("شناسه شرکت بدرستی ارسال نشده است");
            }

            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if ((user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN") || (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId)))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == model.AccountId, cancellationToken: cancellationToken);
                if (account == null)
                {
                    return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.CompanyId != null && !await _context.Companies.AnyAsync(x => x.CompanyId == model.CompanyId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه شرکت بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.AccountNumber.IsNotNull() && await _context.Accounts.AnyAsync(x => x.AccountNumber.ToLower() == model.AccountNumber.MultipleSpaceRemoverTrim().ToEnglishNumber().ToLower() && x.CompanyId == (model.CompanyId == null ? account.CompanyId : model.CompanyId) && x.AccountId != account.AccountId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت حساب با شماره حساب تکراری وجود ندارد", HttpStatusCode.Conflict);
                }
                if (model.AccountNumber.IsNotNull())
                {
                    account.AccountNumber = model.AccountNumber.MultipleSpaceRemoverTrim().ToEnglishNumber();
                }
                if (model.BankName.IsNotNull())
                {
                    account.BankName = model.BankName.MultipleSpaceRemoverTrim();
                }
                if (model.AccountType.IsNotNull())
                {
                    account.AccountType = model.AccountType.MultipleSpaceRemoverTrim();
                }
                if (model.InterestRatePercent != null)
                {
                    account.InterestRatePercent = Convert.ToDecimal(model.InterestRatePercent);
                }
                if (model.ProjectName.IsNotNull())
                {
                    account.ProjectName = model.ProjectName.MultipleSpaceRemoverTrim();
                }
                if (model.BelongsToCentralOffice != null)
                {
                    account.BelongsToCentralOffice = model.BelongsToCentralOffice == true;
                }
                if (model.Description.IsNotNull())
                {
                    account.Description = model.Description.MultipleSpaceRemoverTrim();
                }
                if (model.CompanyId != null)
                {
                    account.CompanyId = Convert.ToInt64(model.CompanyId);
                }
                account.LastModificationDate = Op.OperationDate;
                account.LastModifier = UserId;
                _context.Entry<Account>(account).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی اطلاعات حساب با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات حساب با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteAccount(long UserId, Request_AccountIdDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteAccount");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای حذف حساب ارسال نشده است");
            }
            if (model.AccountId <= 0)
            {
                return Op.Failed("شناسه حساب بدرستی ارسال نشده است");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if ((user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN") || (user.Role.RoleName.ToUpper() == "ADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId)))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var account = await _context.Accounts.Include(x => x.BiDataEntries).FirstOrDefaultAsync(x => x.AccountId == model.AccountId, cancellationToken: cancellationToken);
                if (account == null)
                {
                    return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (account.BiDataEntries != null && account.BiDataEntries.Any())
                {
                    return Op.Failed("در صورت وجود رکورد مالی (BI) حساب، حذف امکان پذیر نمیباشد", HttpStatusCode.Forbidden);
                }
                _context.F_UserAccounts.RemoveRange(await _context.F_UserAccounts.Where(x => x.AccountId == account.AccountId).ToListAsync(cancellationToken: cancellationToken));
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف حساب با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف حساب با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_GetRoleDTO>> GetRoles(long UserId, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetRoleDTO> Op = new("GetRoles");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            try
            {
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                return Op.Succeed("دریافت لیست نقش های کاربری با موفقیت انجام شد", (from role in await _context.Roles.ToListAsync(cancellationToken: cancellationToken)
                                                                                    select new Response_GetRoleDTO
                                                                                    {
                                                                                        DisplayName = role.DisplayName,
                                                                                        RoleId = role.RoleId,
                                                                                        RoleName = role.RoleName,
                                                                                    }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست نقش های کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_GetAccountDTO>> GetAccount(long UserId, long AccountId, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetAccountDTO> Op = new("GetAccount");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (AccountId <= 0)
            {
                return Op.Failed("شناسه حساب بدرستی ارسال نشده است");
            }
            try
            {
                var financeDataEntry_SubModule = await _context.SubModules.FirstOrDefaultAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken);
                if (financeDataEntry_SubModule == null)
                {
                    return Op.Failed("دریافت اطلاعات دسترسی ماژول با مشکل مواجه شده است", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && !user.F_UserSubModules.Any(x => x.SubModuleId == financeDataEntry_SubModule.SubModuleId))
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (user.Role.RoleName.ToUpper() == "SUPERADMIN" || user.Role.RoleName.ToUpper() == "ADMIN")
                {
                    var account = await _context.Accounts.Include(x => x.Company).FirstOrDefaultAsync(x => x.AccountId == AccountId, cancellationToken: cancellationToken);
                    if (account == null)
                    {
                        return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    return Op.Succeed("دریافت اطلاعات حساب با موفقیت انجام شد", new Response_GetAccountDTO
                    {
                        AccountId = account.AccountId,
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType,
                        BankName = account.BankName,
                        BelongsToCentralOffice = account.BelongsToCentralOffice,
                        CompanyCode = account.Company.CompanyCode,
                        CompanyId = account.CompanyId,
                        CompanyName = account.Company.Name,
                        CompanyNumber = account.Company.CompanyNumber,
                        CreationDate = account.CreationDate,
                        Description = account.Description,
                        InterestRatePercent = account.InterestRatePercent,
                        LastModificationDate = account.LastModificationDate,
                        ProjectName = account.ProjectName,
                    });
                }
                else
                {
                    var account = (from acc in await _context.Accounts.Include(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                   join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                   on acc.AccountId equals f_userAccount.AccountId
                                   where f_userAccount.UserId == UserId && acc.AccountId == AccountId
                                   select acc).FirstOrDefault();
                    if (account == null)
                    {
                        return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    return Op.Succeed("دریافت اطلاعات حساب با موفقیت انجام شد", new Response_GetAccountDTO
                    {
                        AccountId = account.AccountId,
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType,
                        BankName = account.BankName,
                        BelongsToCentralOffice = account.BelongsToCentralOffice,
                        CompanyCode = account.Company.CompanyCode,
                        CompanyId = account.CompanyId,
                        CompanyName = account.Company.Name,
                        CompanyNumber = account.Company.CompanyNumber,
                        CreationDate = account.CreationDate,
                        Description = account.Description,
                        InterestRatePercent = account.InterestRatePercent,
                        LastModificationDate = account.LastModificationDate,
                        ProjectName = account.ProjectName,
                    });
                }
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات حساب با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
