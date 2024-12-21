using Microsoft.EntityFrameworkCore;
using Mika.Domain.Contracts.DTOs.BiDataEntry;
using Mika.Domain.Contracts.DTOs.BiDataEntry.Filter;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Entities;
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
    public class BiRepository : IBiRepository
    {
        private readonly Context _context;
        public BiRepository(Context context)
        {
            this._context = context;
        }
        public async Task<OperationResult<object>> CreateBiDataEntry(long CreatorId, Request_CreateBiDataEntryDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateBiDataEntry");
            if (CreatorId <= 0)
            {
                return Op.Failed("اطلاعات کاربری ثبت کننده بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.BalanceOfThePreviousDay < 0)
            {
                return Op.Failed("موجود روز قبل نمیتواند کوچک تر از صفر باشد");
            }
            if (model.BlockadeAmount < 0)
            {
                return Op.Failed("مبلغ مسدودی نمیتواند کوچک تر از صفر باشد");
            }
            if (model.DepositDuringTheDay < 0)
            {
                return Op.Failed("واریز طی روز نمیتواند کوچک تر از صفر باشد");
            }
            if (model.WithdrawalDuringTheDay < 0)
            {
                return Op.Failed("برداشت طی روز نمیتواند کوچک تر از صفر باشد");
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
                var user = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == CreatorId, cancellationToken: cancellationToken);
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
                    if (!await _context.Accounts.AnyAsync(x => x.AccountId == model.AccountId, cancellationToken: cancellationToken))
                    {
                        return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    if (!(from acc in await _context.Accounts.ToListAsync(cancellationToken: cancellationToken)
                          join f_userAcc in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                          on acc.AccountId equals f_userAcc.AccountId
                          where f_userAcc.UserId == CreatorId
                          select acc).Any(x => x.AccountId == model.AccountId))
                    {
                        return Op.Failed("شناسه حساب بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                }

                long Calc_BalanceWithTheBank = (model.BalanceOfThePreviousDay + model.DepositDuringTheDay) - model.WithdrawalDuringTheDay;
                long Calc_WithdrawalBalance = Calc_BalanceWithTheBank + model.DefinitiveReceivablePayableDocs;
                long Calc_AccountBalance = Calc_WithdrawalBalance + model.OnTheWayReceivablePayableDocs;

                await _context.BiDataEntries.AddAsync(new BiDataEntry
                {
                    AccountBalance = Calc_AccountBalance,
                    AccountId = model.AccountId,
                    BalanceOfThePreviousDay = model.BalanceOfThePreviousDay,
                    BalanceWithTheBank = Calc_BalanceWithTheBank,
                    BlockadeAmount = model.BlockadeAmount,
                    CreationDate = Op.OperationDate,
                    CreatorId = CreatorId,
                    DefinitiveReceivablePayableDocs = model.DefinitiveReceivablePayableDocs,
                    DepositDuringTheDay = model.DepositDuringTheDay,
                    EntryDateTime = model.EntryDateTime,
                    OnTheWayReceivablePayableDocs = model.OnTheWayReceivablePayableDocs,
                    WithdrawalBalance = Calc_WithdrawalBalance,
                    WithdrawalDuringTheDay = model.WithdrawalDuringTheDay,
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت اطلاعات با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PagedList<Response_GetBiDataEntryDTO>>> GetBiDataEntries(long UserId, Filter_GetBiDataEntryDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            OperationResult<PagedList<Response_GetBiDataEntryDTO>> Op = new("GetBiDataEntries");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (filter != null)
            {
                if (filter.BiDataEntryId != null && filter.BiDataEntryId <= 0)
                {
                    return Op.Failed("شناسه اطلاعات بدرستی وارد نشده است");
                }
                if (filter.CreatorId != null && filter.CreatorId <= 0)
                {
                    return Op.Failed("شناسه ثبت کننده بدرستی وارد نشده است");
                }
                if (filter.RoleId != null && filter.RoleId <= 0)
                {
                    return Op.Failed("شناسه نقش ثبت کننده بدرستی وارد نشده است");
                }
                if (filter.AccountId != null && filter.AccountId <= 0)
                {
                    return Op.Failed("شناسه حساب بدرستی وارد نشده است");
                }
                if (filter.CompanyId != null && filter.CompanyId <= 0)
                {
                    return Op.Failed("شناسه شرکت بدرستی وارد نشده است");
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
                var safeSearchString = filter.Search.IsNotNull() ? filter.Search.MultipleSpaceRemoverTrim().ToLower() : null;
                long totalCount = 0;
                var result = new List<Response_GetBiDataEntryDTO>();
                if (user.Role.RoleName.ToUpper() == "SUPERADMIN" || user.Role.RoleName.ToUpper() == "ADMIN")
                {
                    totalCount = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  where filter == null || (
                                        (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                        (filter.CreatorId == null || data.CreatorId == filter.CreatorId) &&
                                        (filter.RoleId == null || data.Creator.RoleId == filter.RoleId) &&
                                        (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                        (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                        (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                        (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                        (!safeSearchString.IsNotNull() || data.Creator.UserName.ToLower().Contains(safeSearchString) || $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}".ToLower().Contains(safeSearchString) || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                    )
                                  select data.BiDataEntryId).ToList().Count;
                    if (totalCount == 0)
                    {
                        return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                    }
                    if (page == null || page.Contain == null || page.Contain <= 0)
                    {
                        result = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  where filter == null || (
                                        (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                        (filter.CreatorId == null || data.CreatorId == filter.CreatorId) &&
                                        (filter.RoleId == null || data.Creator.RoleId == filter.RoleId) &&
                                        (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                        (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                        (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                        (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                        (!safeSearchString.IsNotNull() || data.Creator.UserName.ToLower().Contains(safeSearchString) || $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}".ToLower().Contains(safeSearchString) || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                    )
                                  select new Response_GetBiDataEntryDTO
                                  {
                                      Account = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetAccountDTO
                                      {
                                          AccountId = data.AccountId,
                                          AccountNumber = data.Account.AccountNumber,
                                          AccountType = data.Account.AccountType,
                                          BankName = data.Account.BankName,
                                          ProjectName = data.Account.ProjectName,
                                      },
                                      AccountBalance = data.AccountBalance,
                                      BalanceOfThePreviousDay = data.BalanceOfThePreviousDay,
                                      BalanceWithTheBank = data.BalanceWithTheBank,
                                      BiDataEntryId = data.BiDataEntryId,
                                      BlockadeAmount = data.BlockadeAmount,
                                      Company = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCompanyDTO
                                      {
                                          CompanyCode = data.Account.Company.CompanyCode,
                                          CompanyId = data.Account.CompanyId,
                                          CompanyName = data.Account.Company.Name,
                                          CompanyNumber = data.Account.Company.CompanyNumber,
                                      },
                                      CreationDate = data.CreationDate,
                                      Creator = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCreatorDTO
                                      {
                                          Avatar = data.Creator.Avatar,
                                          LastName = data.Creator.LastName,
                                          FullName = $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}",
                                          ShortName = $"{data.Creator.Name[..1]}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName[..1]}" : "")}",
                                          Name = data.Creator.Name,
                                          RoleDisplayName = data.Creator.Role.DisplayName,
                                          RoleId = data.Creator.RoleId,
                                          RoleName = data.Creator.Role.RoleName,
                                          UserId = data.CreatorId,
                                          UserName = data.Creator.UserName,
                                      },
                                      DefinitiveReceivablePayableDocs = data.DefinitiveReceivablePayableDocs,
                                      DepositDuringTheDay = data.DepositDuringTheDay,
                                      EntryDateTime = data.EntryDateTime,
                                      LastModificationDate = data.LastModificationDate,
                                      OnTheWayReceivablePayableDocs = data.OnTheWayReceivablePayableDocs,
                                      WithdrawalBalance = data.WithdrawalBalance,
                                      WithdrawalDuringTheDay = data.WithdrawalDuringTheDay
                                  }).OrderByDescending(x => x.EntryDateTime).ThenBy(x => x.Account.AccountId).ToList();
                    }
                    else
                    {
                        if (page.Number == null || page.Number <= 0)
                        {
                            page.Number = 1;
                        }
                        result = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  where filter == null || (
                                        (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                        (filter.CreatorId == null || data.CreatorId == filter.CreatorId) &&
                                        (filter.RoleId == null || data.Creator.RoleId == filter.RoleId) &&
                                        (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                        (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                        (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                        (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                        (!safeSearchString.IsNotNull() || data.Creator.UserName.ToLower().Contains(safeSearchString) || $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}".ToLower().Contains(safeSearchString) || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                    )
                                  select new Response_GetBiDataEntryDTO
                                  {
                                      Account = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetAccountDTO
                                      {
                                          AccountId = data.AccountId,
                                          AccountNumber = data.Account.AccountNumber,
                                          AccountType = data.Account.AccountType,
                                          BankName = data.Account.BankName,
                                          ProjectName = data.Account.ProjectName,
                                      },
                                      AccountBalance = data.AccountBalance,
                                      BalanceOfThePreviousDay = data.BalanceOfThePreviousDay,
                                      BalanceWithTheBank = data.BalanceWithTheBank,
                                      BiDataEntryId = data.BiDataEntryId,
                                      BlockadeAmount = data.BlockadeAmount,
                                      Company = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCompanyDTO
                                      {
                                          CompanyCode = data.Account.Company.CompanyCode,
                                          CompanyId = data.Account.CompanyId,
                                          CompanyName = data.Account.Company.Name,
                                          CompanyNumber = data.Account.Company.CompanyNumber,
                                      },
                                      CreationDate = data.CreationDate,
                                      Creator = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCreatorDTO
                                      {
                                          Avatar = data.Creator.Avatar,
                                          LastName = data.Creator.LastName,
                                          FullName = $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}",
                                          ShortName = $"{data.Creator.Name[..1]}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName[..1]}" : "")}",
                                          Name = data.Creator.Name,
                                          RoleDisplayName = data.Creator.Role.DisplayName,
                                          RoleId = data.Creator.RoleId,
                                          RoleName = data.Creator.Role.RoleName,
                                          UserId = data.CreatorId,
                                          UserName = data.Creator.UserName,
                                      },
                                      DefinitiveReceivablePayableDocs = data.DefinitiveReceivablePayableDocs,
                                      DepositDuringTheDay = data.DepositDuringTheDay,
                                      EntryDateTime = data.EntryDateTime,
                                      LastModificationDate = data.LastModificationDate,
                                      OnTheWayReceivablePayableDocs = data.OnTheWayReceivablePayableDocs,
                                      WithdrawalBalance = data.WithdrawalBalance,
                                      WithdrawalDuringTheDay = data.WithdrawalDuringTheDay
                                  }).OrderByDescending(x => x.EntryDateTime).ThenBy(x => x.Account.AccountId).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                    }
                }
                else
                {
                    totalCount = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                  on data.AccountId equals f_userAccount.AccountId
                                  where (f_userAccount.UserId == UserId) && (filter == null ||
                                            (
                                                (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                                (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                                (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                                (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                                (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                                (!safeSearchString.IsNotNull() || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                            )
                                        )
                                  select data.BiDataEntryId).ToList().Count;
                    if (totalCount == 0)
                    {
                        return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                    }
                    if (page == null || page.Contain == null || page.Contain <= 0)
                    {
                        result = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                  on data.AccountId equals f_userAccount.AccountId
                                  where (f_userAccount.UserId == UserId) && (filter == null ||
                                            (
                                                (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                                (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                                (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                                (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                                (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                                (!safeSearchString.IsNotNull() || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                            )
                                        )
                                  select new Response_GetBiDataEntryDTO
                                  {
                                      Account = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetAccountDTO
                                      {
                                          AccountId = data.AccountId,
                                          AccountNumber = data.Account.AccountNumber,
                                          AccountType = data.Account.AccountType,
                                          BankName = data.Account.BankName,
                                          ProjectName = data.Account.ProjectName,
                                      },
                                      AccountBalance = data.AccountBalance,
                                      BalanceOfThePreviousDay = data.BalanceOfThePreviousDay,
                                      BalanceWithTheBank = data.BalanceWithTheBank,
                                      BiDataEntryId = data.BiDataEntryId,
                                      BlockadeAmount = data.BlockadeAmount,
                                      Company = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCompanyDTO
                                      {
                                          CompanyCode = data.Account.Company.CompanyCode,
                                          CompanyId = data.Account.CompanyId,
                                          CompanyName = data.Account.Company.Name,
                                          CompanyNumber = data.Account.Company.CompanyNumber,
                                      },
                                      CreationDate = data.CreationDate,
                                      Creator = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCreatorDTO
                                      {
                                          Avatar = data.Creator.Avatar,
                                          LastName = data.Creator.LastName,
                                          FullName = $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}",
                                          ShortName = $"{data.Creator.Name[..1]}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName[..1]}" : "")}",
                                          Name = data.Creator.Name,
                                          RoleDisplayName = data.Creator.Role.DisplayName,
                                          RoleId = data.Creator.RoleId,
                                          RoleName = data.Creator.Role.RoleName,
                                          UserId = data.CreatorId,
                                          UserName = data.Creator.UserName,
                                      },
                                      DefinitiveReceivablePayableDocs = data.DefinitiveReceivablePayableDocs,
                                      DepositDuringTheDay = data.DepositDuringTheDay,
                                      EntryDateTime = data.EntryDateTime,
                                      LastModificationDate = data.LastModificationDate,
                                      OnTheWayReceivablePayableDocs = data.OnTheWayReceivablePayableDocs,
                                      WithdrawalBalance = data.WithdrawalBalance,
                                      WithdrawalDuringTheDay = data.WithdrawalDuringTheDay
                                  }).OrderByDescending(x => x.EntryDateTime).ThenBy(x => x.Account.AccountId).ToList();
                    }
                    else
                    {
                        if (page.Number == null || page.Number <= 0)
                        {
                            page.Number = 1;
                        }
                        result = (from data in await _context.BiDataEntries.Include(x => x.Creator).ThenInclude(x => x.Role).Include(x => x.Account).ThenInclude(x => x.Company).ToListAsync(cancellationToken: cancellationToken)
                                  join f_userAccount in await _context.F_UserAccounts.ToListAsync(cancellationToken: cancellationToken)
                                  on data.AccountId equals f_userAccount.AccountId
                                  where (f_userAccount.UserId == UserId) && (filter == null ||
                                            (
                                                (filter.BiDataEntryId == null || data.BiDataEntryId == filter.BiDataEntryId) &&
                                                (filter.AccountId == null || data.AccountId == filter.AccountId) &&
                                                (filter.CompanyId == null || data.Account.CompanyId == filter.CompanyId) &&
                                                (filter.EntryDateTime == null || data.EntryDateTime.ResetTime() == filter.EntryDateTime.ResetTime()) &&
                                                (filter.CreationDate == null || data.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                                (!safeSearchString.IsNotNull() || data.Account.AccountNumber.ToLower().Contains(safeSearchString) || data.Account.BankName.ToLower().Contains(safeSearchString) || (data.Account.AccountType.IsNotNull() ? data.Account.AccountType : "").ToLower().Contains(safeSearchString) || data.Account.ProjectName.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyNumber.ToLower().Contains(safeSearchString) || data.Account.Company.CompanyCode.ToLower().Contains(safeSearchString) || data.Account.Company.Name.ToLower().Contains(safeSearchString))
                                            )
                                        )
                                  select new Response_GetBiDataEntryDTO
                                  {
                                      Account = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetAccountDTO
                                      {
                                          AccountId = data.AccountId,
                                          AccountNumber = data.Account.AccountNumber,
                                          AccountType = data.Account.AccountType,
                                          BankName = data.Account.BankName,
                                          ProjectName = data.Account.ProjectName,
                                      },
                                      AccountBalance = data.AccountBalance,
                                      BalanceOfThePreviousDay = data.BalanceOfThePreviousDay,
                                      BalanceWithTheBank = data.BalanceWithTheBank,
                                      BiDataEntryId = data.BiDataEntryId,
                                      BlockadeAmount = data.BlockadeAmount,
                                      Company = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCompanyDTO
                                      {
                                          CompanyCode = data.Account.Company.CompanyCode,
                                          CompanyId = data.Account.CompanyId,
                                          CompanyName = data.Account.Company.Name,
                                          CompanyNumber = data.Account.Company.CompanyNumber,
                                      },
                                      CreationDate = data.CreationDate,
                                      Creator = new Domain.Contracts.DTOs.BiDataEntry.Middle.Middle_GetCreatorDTO
                                      {
                                          Avatar = data.Creator.Avatar,
                                          LastName = data.Creator.LastName,
                                          FullName = $"{data.Creator.Name}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName}" : "")}",
                                          ShortName = $"{data.Creator.Name[..1]}{(data.Creator.LastName.IsNotNull() ? $" {data.Creator.LastName[..1]}" : "")}",
                                          Name = data.Creator.Name,
                                          RoleDisplayName = data.Creator.Role.DisplayName,
                                          RoleId = data.Creator.RoleId,
                                          RoleName = data.Creator.Role.RoleName,
                                          UserId = data.CreatorId,
                                          UserName = data.Creator.UserName,
                                      },
                                      DefinitiveReceivablePayableDocs = data.DefinitiveReceivablePayableDocs,
                                      DepositDuringTheDay = data.DepositDuringTheDay,
                                      EntryDateTime = data.EntryDateTime,
                                      LastModificationDate = data.LastModificationDate,
                                      OnTheWayReceivablePayableDocs = data.OnTheWayReceivablePayableDocs,
                                      WithdrawalBalance = data.WithdrawalBalance,
                                      WithdrawalDuringTheDay = data.WithdrawalDuringTheDay
                                  }).OrderByDescending(x => x.EntryDateTime).ThenBy(x => x.Account.AccountId).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                    }
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", new PagedList<Response_GetBiDataEntryDTO>(result, totalCount, page));
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }
    }
}
