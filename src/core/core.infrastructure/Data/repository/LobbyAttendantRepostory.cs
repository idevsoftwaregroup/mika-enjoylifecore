using core.application.Contract.API.DTO.LobbyAttendant;
using core.application.Contract.API.DTO.LobbyAttendant.Filter;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.entity.partyModels;
using core.infrastructure.Data.persist;
using IdentityProvider.Infrastructure.Framework.Security.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace core.infrastructure.Data.repository
{
    public class LobbyAttendantRepostory : ILobbyAttendantRepostory
    {
        private readonly EnjoyLifeContext _context;
        private readonly IPasswordHasher _passwordHasher;
        public LobbyAttendantRepostory(EnjoyLifeContext context, IPasswordHasher passwordHasher)
        {
            this._context = context;
            this._passwordHasher = passwordHasher;
        }
        public async Task<OperationResult<object>> CreateLobbyAttendant(int AdminId, Request_CreateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateLobbyAttendant");
            if (AdminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت ثبت ارسال نشده است");
            }
            if (!model.UserName.IsNotNull())
            {
                return Op.Failed("ارسال نام کاربری اجباری است");
            }
            if (model.UserName.MultipleSpaceRemoverTrim().Length < 2 || model.UserName.MultipleSpaceRemoverTrim().Length > 45)
            {
                return Op.Failed("نام کاربری بدرستی ارسال نشده است");
            }
            if (!model.Password.IsNotNull())
            {
                return Op.Failed("ارسال رمز عبور اجباری است");
            }
            if (model.Password.Length < 4)
            {
                return Op.Failed($"حداقل کاراکتر مجاز رمز عبور {4.ToPersianNumber()} کاراکتر است");
            }
            if (!model.FirstName.IsNotNull())
            {
                return Op.Failed("ارسال نام اجباری است");
            }
            if (model.FirstName.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed("نام بدرستی ارسال نشده است");
            }
            if (!model.LastName.IsNotNull())
            {
                return Op.Failed("ارسال نام خانوادگی اجباری است");
            }
            if (model.LastName.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed("نام خانوادگی بدرستی ارسال نشده است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == AdminId && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (await _context.LobbyAttendants.AnyAsync(x => x.UserName.ToLower() == model.UserName.MultipleSpaceRemoverTrim().ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت لابی من با نام کاربری تکراری وجود ندارد", HttpStatusCode.Conflict);
                }
                await _context.LobbyAttendants.AddAsync(new domain.entity.partyModels.LobbyAttendantModel
                {
                    Active = true,
                    CreationDate = Op.OperationDate,
                    Creator = AdminId,
                    FirstName = model.FirstName.MultipleSpaceRemoverTrim(),
                    LastName = model.LastName.MultipleSpaceRemoverTrim(),
                    Password = _passwordHasher.Hash(model.Password),
                    PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null,
                    UserName = model.UserName.MultipleSpaceRemoverTrim(),

                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت لابی من با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت لابی من با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteLobbyAttendant(Request_LobbyAttendantIdDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteLobbyAttendant");
            if (model == null || model.LobbyAttendantId <= 0)
            {
                return Op.Failed("شناسه لابی من بدرستی ارسال نشده است");
            }
            try
            {
                var lobbyMan = await _context.LobbyAttendants.FirstOrDefaultAsync(x => x.LobbyAttendantId == model.LobbyAttendantId, cancellationToken: cancellationToken);
                if (lobbyMan == null)
                {
                    return Op.Failed("شناسه لابی من بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                _context.LobbyAttendants.Remove(lobbyMan);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف لابی من با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف لابی من با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Reponse_GetLobbyAttendantDTO>> GetLobbyAttendants(Filter_GetLobbyAttendantDTO filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Reponse_GetLobbyAttendantDTO> Op = new("GetLobbyAttendants");
            try
            {
                var result = (from u in await _context.LobbyAttendants.ToListAsync(cancellationToken: cancellationToken)
                              where filter == null || (
                                (!filter.Search.IsNotNull() || (u.UserName.ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || $"{u.FirstName} {u.LastName}".ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()) || $"{(u.PhoneNumber.IsNotNull() ? u.PhoneNumber : "")}".ToLower().Contains(filter.Search.MultipleSpaceRemoverTrim().ToLower()))) &&
                                (filter.Active == null || u.Active == Convert.ToBoolean(filter.Active))
                              )
                              select new Reponse_GetLobbyAttendantDTO
                              {
                                  Active = u.Active,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  LobbyAttendantId = u.LobbyAttendantId,
                                  PhoneNumber = u.PhoneNumber,
                                  UserName = u.UserName,
                                  FullName = $"{u.FirstName} {u.LastName}"
                              }).ToList();
                if (result == null || !result.Any())
                {
                    return Op.Succeed("دریافت اطلاعات لابی من ها با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات لابی من ها با موفقیت انجام شد", result);
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات لابی من ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LobbyAttendantUnitsDTO> Op = new("GetUnitsForLobbyAttendant");
            var loginOperation = await LoginLobbyAttendant(model, cancellationToken);
            if (!loginOperation.Success)
            {
                return Op.Failed(loginOperation.Message, loginOperation.ExMessage, loginOperation.Status);
            }
            try
            {
                return Op.Succeed("دریافت اطلاعات واحدها با موفقیت انجام شد", (from unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                                                               where unit.IsDelete != true
                                                                               orderby unit.Floor
                                                                               select new Response_LobbyAttendantUnitsDTO
                                                                               {
                                                                                   Name = unit.Name,
                                                                                   UnitId = unit.Id
                                                                               }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات واحد با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant_Authorized(CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LobbyAttendantUnitsDTO> Op = new("GetUnitsForLobbyAttendant_Authorized");
            try
            {
                return Op.Succeed("دریافت اطلاعات واحدها با موفقیت انجام شد", (from unit in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                                                               where unit.IsDelete != true
                                                                               orderby unit.Floor
                                                                               select new Response_LobbyAttendantUnitsDTO
                                                                               {
                                                                                   Name = unit.Name,
                                                                                   UnitId = unit.Id
                                                                               }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات واحد با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant(int UnitId, Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LobbyAttendantUsersInUnitDTO> Op = new("GetUsersInUnitForLobbyAttendant");
            if (UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            var loginOperation = await LoginLobbyAttendant(model, cancellationToken);
            if (!loginOperation.Success)
            {
                return Op.Failed(loginOperation.Message, loginOperation.ExMessage, loginOperation.Status);
            }
            try
            {
                if (!await _context.Units.AnyAsync(x => x.Id == UnitId && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                List<int> userIDs = new();
                userIDs.AddRange((from resident in await _context.Residents.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                  where resident.IsDelete != true && resident.User.IsDelete != true && resident.Unit.Id == UnitId
                                  select resident.User.Id).ToList());
                userIDs.AddRange((from owner in await _context.Owners.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                  where owner.IsDelete != true && owner.User.IsDelete != true && owner.Unit.Id == UnitId
                                  select owner.User.Id).ToList());
                if (userIDs == null || !userIDs.Distinct().Any())
                {
                    return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                                        join id in userIDs.Distinct().ToList()
                                                                        on u.Id equals id
                                                                        select new Response_LobbyAttendantUsersInUnitDTO
                                                                        {
                                                                            FirstName = u.FirstName,
                                                                            Id = u.Id,
                                                                            LastName = u.LastName,
                                                                            PhoneNumber = u.PhoneNumber,
                                                                            FullName = $"{u.FirstName} {u.LastName}"
                                                                        }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات ساکنین/مالکین واحد با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant_Authorized(int UnitId, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LobbyAttendantUsersInUnitDTO> Op = new("GetUsersInUnitForLobbyAttendant_Authorized");
            try
            {
                if (!await _context.Units.AnyAsync(x => x.Id == UnitId && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                List<int> userIDs = new();
                userIDs.AddRange((from resident in await _context.Residents.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                  where resident.IsDelete != true && resident.User.IsDelete != true && resident.Unit.Id == UnitId
                                  select resident.User.Id).ToList());
                userIDs.AddRange((from owner in await _context.Owners.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                  where owner.IsDelete != true && owner.User.IsDelete != true && owner.Unit.Id == UnitId
                                  select owner.User.Id).ToList());
                if (userIDs == null || !userIDs.Distinct().Any())
                {
                    return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات با موفقیت انجام شد", (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                                        join id in userIDs.Distinct().ToList()
                                                                        on u.Id equals id
                                                                        select new Response_LobbyAttendantUsersInUnitDTO
                                                                        {
                                                                            FirstName = u.FirstName,
                                                                            Id = u.Id,
                                                                            LastName = u.LastName,
                                                                            PhoneNumber = u.PhoneNumber,
                                                                            FullName = $"{u.FirstName} {u.LastName}"
                                                                        }).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات ساکنین/مالکین واحد با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<object>> LoginLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("LoginLobbyAttendant");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت ورود ارسال نشده است");
            }
            if (!model.Username.IsNotNull() || model.Username.MultipleSpaceRemoverTrim().Length < 2 || model.Username.MultipleSpaceRemoverTrim().Length > 45)
            {
                return Op.Failed("نام کاربری بدرستی ارسال نشده است");
            }
            if (!model.Password.IsNotNull() || model.Password.Length < 4)
            {
                return Op.Failed("رمز عبور بدرستی ارسال نشده است");
            }
            try
            {
                var lobbyMan = await _context.LobbyAttendants.FirstOrDefaultAsync(x => x.UserName.ToLower() == model.Username.MultipleSpaceRemoverTrim().ToLower(), cancellationToken: cancellationToken);
                if (lobbyMan == null)
                {
                    return Op.Failed("اطلاعات ورود بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!_passwordHasher.Check(lobbyMan.Password, model.Password).Verified)
                {
                    return Op.Failed("اطلاعات ورود بدرستی ارسال نشده است", HttpStatusCode.Conflict);
                }
                if (!lobbyMan.Active)
                {
                    return Op.Failed("دسترسی ورود برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                return Op.Succeed("ورود با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ورود لابی من با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant(Request_LobbyAttendandGetTokenDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LoginLobbyAttendantDTO> Op = new("SecondLoginLobbyAttendant");
            if (model == null || model.UserId <= 0)
            {
                return Op.Failed("اطلاعات بدرستی ارسال نشده است");
            }
            var loginOperation = await LoginLobbyAttendant(new Request_LoginLobbyAttendantDTO
            {
                Password = model.Password,
                Username = model.Username,
            }, cancellationToken);
            if (!loginOperation.Success)
            {
                return Op.Failed(loginOperation.Message, loginOperation.ExMessage, loginOperation.Status);
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IsDelete != true && x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                return Op.Succeed("اطلاعات کاربری با موفقیت دریافت شد", new Response_LoginLobbyAttendantDTO
                {
                    Email = user.Email,
                    IsLobbyAttendant = true,
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.Id,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant_Authorized(Request_LobbyAttendantUserIdDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_LoginLobbyAttendantDTO> Op = new("SecondLoginLobbyAttendant_Authorized");
            if (model == null || model.UserId <= 0)
            {
                return Op.Failed("اطلاعات بدرستی ارسال نشده است");
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IsDelete != true && x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                return Op.Succeed("اطلاعات کاربری با موفقیت دریافت شد", new Response_LoginLobbyAttendantDTO
                {
                    Email = user.Email,
                    IsLobbyAttendant = true,
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.Id,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<object>> UpdateLobbyAttendant(int AdminId, Request_UpdateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateLobbyAttendant");
            if (AdminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت بروزرسانی ارسال نشده است");
            }
            if (model.LobbyAttendantId <= 0)
            {
                return Op.Failed("شناسه لابی من بدرستی ارسال نشده است");
            }
            if (model.UserName.IsNotNull() && (model.UserName.MultipleSpaceRemoverTrim().Length < 2 || model.UserName.MultipleSpaceRemoverTrim().Length > 45))
            {
                return Op.Failed("نام کاربری بدرستی ارسال نشده است");
            }
            if (model.Password.IsNotNull() && model.Password.Length < 4)
            {
                return Op.Failed($"حداقل کاراکتر مجاز رمز عبور {4.ToPersianNumber()} کاراکتر است");
            }
            if (model.FirstName.IsNotNull() && model.FirstName.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed("نام بدرستی ارسال نشده است");
            }
            if (model.LastName.IsNotNull() && model.LastName.MultipleSpaceRemoverTrim().Length > 35)
            {
                return Op.Failed("نام خانوادگی بدرستی ارسال نشده است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == AdminId && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var lobbyMan = await _context.LobbyAttendants.FirstOrDefaultAsync(x => x.LobbyAttendantId == model.LobbyAttendantId, cancellationToken: cancellationToken);
                if (lobbyMan == null)
                {
                    return Op.Failed("شناسه لابی من بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.UserName.IsNotNull() && await _context.LobbyAttendants.AnyAsync(x => x.UserName.ToLower() == model.UserName.MultipleSpaceRemoverTrim().ToLower() && x.LobbyAttendantId != model.LobbyAttendantId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان بروزرسانی لابی من با نام کاربری تکراری وجود ندارد", HttpStatusCode.Conflict);
                }
                if (model.UserName.IsNotNull())
                {
                    lobbyMan.UserName = model.UserName.MultipleSpaceRemoverTrim();
                }
                if (model.Password.IsNotNull())
                {
                    lobbyMan.Password = _passwordHasher.Hash(model.Password);
                }
                lobbyMan.Active = model.Active;
                if (model.FirstName.IsNotNull())
                {
                    lobbyMan.FirstName = model.FirstName.MultipleSpaceRemoverTrim();
                }
                if (model.LastName.IsNotNull())
                {
                    lobbyMan.LastName = model.LastName.MultipleSpaceRemoverTrim();
                }
                lobbyMan.PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null;
                lobbyMan.LastModifier = AdminId;
                lobbyMan.LastModificationDate = Op.OperationDate;
                _context.Entry<LobbyAttendantModel>(lobbyMan).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی اطلاعات لابی من با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات لابی من با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }
    }
}
