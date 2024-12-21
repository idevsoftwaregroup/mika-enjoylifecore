using Microsoft.EntityFrameworkCore;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Domain.Contracts.DTOs.Modules.Middle;
using Mika.Domain.Contracts.DTOs.Users;
using Mika.Domain.Contracts.DTOs.Users.Filter;
using Mika.Domain.Contracts.DTOs.Users.Middle;
using Mika.Domain.Entities;
using Mika.Domain.Entities.RelationalEntities;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Framework.Services.Interfaces;
using Mika.Framework.Utilities;
using Mika.Infastructure.Data;
using Mika.Infastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Infastructure.Services.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        private readonly IPasswordHasher _passwordHasher;
        public UserRepository(Context context, IPasswordHasher passwordHasher)
        {
            this._context = context;
            this._passwordHasher = passwordHasher;
        }
        public async Task<OperationResult<Response_AuthenticationDTO>> Login(Request_LoginDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_AuthenticationDTO> Op = new("Login");
            if (model == null || !model.Username.IsNotNull() || !model.Password.IsNotNull())
            {
                return Op.Failed("ارسال نام کاربری یا رمز عبور برای ورود اجباری است");
            }
            try
            {
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserName.ToLower() == model.Username.ToLower(), cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (!this._passwordHasher.Check(user.Password, model.Password).Verified)
                {
                    return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                List<Middle_Authentication_ModuleDTO> resultModules = new();
                var subModules = (from f_userSubModule in await _context.F_UserSubModules.ToListAsync(cancellationToken: cancellationToken)
                                  join subModule in await _context.SubModules.Include(x => x.Module).ToListAsync(cancellationToken: cancellationToken)
                                  on f_userSubModule.SubModuleId equals subModule.SubModuleId
                                  where f_userSubModule.UserId == user.UserId
                                  select subModule).ToList();
                if (subModules != null && subModules.Any())
                {
                    for (int i = 0; i < subModules.Count; i++)
                    {
                        var moduleInList = resultModules.FirstOrDefault(x => x.Module == subModules[i].Module.ModuleName);
                        if (moduleInList == null)
                        {
                            resultModules.Add(new Middle_Authentication_ModuleDTO
                            {
                                Module = subModules[i].Module.ModuleName,
                                Title = subModules[i].Module.DisplayName,
                                SubModules = new List<Middle_Authentication_SubModuleDTO> { new() {
                                    SubModule= subModules[i].SubModuleName,
                                    Title = subModules[i].SubDisplayName
                                }}
                            });
                        }
                        else
                        {
                            moduleInList.SubModules.Add(new Middle_Authentication_SubModuleDTO
                            {
                                SubModule = subModules[i].SubModuleName,
                                Title = subModules[i].SubDisplayName
                            });
                        }
                    }
                }
                return Op.Succeed("ورود با موفقیت انجام شد", new Response_AuthenticationDTO
                {
                    Avatar = user.Avatar,
                    Email = user.Email,
                    FullName = $"{user.Name}{(user.LastName.IsNotNull() ? $" {user.LastName}" : "")}",
                    LastName = user.LastName,
                    Modules = resultModules,
                    Name = user.Name,
                    PersonnelNumber = user.PersonnelNumber,
                    PhoneNumber = user.PhoneNumber,
                    Position = user.Position,
                    RoleDisplayName = user.Role.DisplayName,
                    RoleName = user.Role.RoleName,
                    ShortName = $"{user.Name[..1]}{(user.LastName.IsNotNull() ? $" {user.LastName[..1]}" : "")}",
                    UserId = user.UserId,
                    UserName = user.UserName,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("ورود با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<Response_AuthenticationDTO>> UpdateProfile(long UserId, Request_UpdateSelfUserDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_AuthenticationDTO> Op = new("UpdateProfile");
            if (UserId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (
              !model.UserName.IsNotNull() &&
              !model.Password.IsNotNull() &&
              !model.Name.IsNotNull() &&
              !model.LastName.IsNotNull() &&
              !model.PhoneNumber.IsNotNull() &&
              !model.Email.IsNotNull() &&
              !model.Position.IsNotNull() &&
              !model.PersonnelNumber.IsNotNull() &&
              !model.Avatar.IsNotNull()
              )
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.UserName.IsNotNull())
            {
                if (model.UserName.Contains(' '))
                {
                    return Op.Failed("امکان بروزرسانی نام کاربری با 'فاصله' وجود ندارد");
                }
                if (model.UserName.Length < 2 || model.UserName.Length > 85)
                {
                    return Op.Failed($"تعداد کاراکتر مجاز نام کاربری از {2.ToPersianNumber()} تا {85.ToPersianNumber()} کاراکتر است");
                }
            }
            if (model.Password.IsNotNull() && (model.Password.Length < 4 || model.Password.Length > 64))
            {
                return Op.Failed($"تعداد کاراکتر مجاز رمز عبور از {4.ToPersianNumber()} تا {64.ToPersianNumber()} کاراکتر است");
            }
            if (model.Name.IsNotNull() && (model.Name.Length < 2 || model.Name.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.LastName.IsNotNull() && (model.LastName.Length < 2 || model.LastName.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام خانوادگی از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشد است");
            }
            if (model.Email.IsNotNull() && !model.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل بدرستی ارسال نشد است");
            }
            if (model.Position.IsNotNull() && (model.Position.Length < 2 || model.Position.Length > 42))
            {
                return Op.Failed($"تعداد کاراکتر مجاز پوزیشن از {2.ToPersianNumber()} تا {42.ToPersianNumber()} کاراکتر است");
            }
            if (model.PersonnelNumber.IsNotNull() && (!model.PersonnelNumber.IsNumeric(false) || model.PhoneNumber.Length < 2 || model.PhoneNumber.Length > 20))
            {
                return Op.Failed("شماره پرسنلی کاربر بدرستی ارسال نشده است");
            }
            if (model.Avatar.IsNotNull() && (model.Avatar.Length > 395))
            {
                return Op.Failed("آدرس تصویر بدرستی ارسال نشده است");
            }
            try
            {

                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربری بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!user.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (model.UserName.IsNotNull() && await _context.Users.AnyAsync(x => x.UserName.ToLower() == model.UserName.ToLower() && x.UserId != UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("نام کاربری ارسال شده تکراری است", HttpStatusCode.Conflict);
                }
                if (model.UserName.IsNotNull())
                {
                    user.UserName = model.UserName;
                }
                if (model.Password.IsNotNull())
                {
                    user.Password = _passwordHasher.Hash(model.Password);
                }
                if (model.Name.IsNotNull())
                {
                    user.Name = model.Name.MultipleSpaceRemoverTrim();
                }
                if (model.LastName.IsNotNull())
                {
                    user.LastName = model.LastName.MultipleSpaceRemoverTrim();
                }
                if (model.PhoneNumber.IsNotNull())
                {
                    user.PhoneNumber = model.PhoneNumber.ToEnglishNumber();
                }
                if (model.Email.IsNotNull())
                {
                    user.Email = model.Email.ToEnglishNumber();
                }
                if (model.Position.IsNotNull())
                {
                    user.Position = model.Position.MultipleSpaceRemoverTrim();
                }
                if (model.PersonnelNumber.IsNotNull())
                {
                    user.PersonnelNumber = model.PersonnelNumber.ToEnglishNumber();
                }
                if (model.Avatar.IsNotNull())
                {
                    user.Avatar = model.Avatar;
                }
                user.LastModificationDate = Op.OperationDate;
                user.LastModifier = UserId;
                _context.Entry<User>(user).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                List<Middle_Authentication_ModuleDTO> resultModules = new();
                var subModules = (from f_userSubModule in await _context.F_UserSubModules.ToListAsync(cancellationToken: cancellationToken)
                                  join subModule in await _context.SubModules.Include(x => x.Module).ToListAsync(cancellationToken: cancellationToken)
                                  on f_userSubModule.SubModuleId equals subModule.SubModuleId
                                  where f_userSubModule.UserId == user.UserId
                                  select subModule).ToList();
                if (subModules != null && subModules.Any())
                {
                    for (int i = 0; i < subModules.Count; i++)
                    {
                        var moduleInList = resultModules.FirstOrDefault(x => x.Module == subModules[i].Module.ModuleName);
                        if (moduleInList == null)
                        {
                            resultModules.Add(new Middle_Authentication_ModuleDTO
                            {
                                Module = subModules[i].Module.ModuleName,
                                Title = subModules[i].Module.DisplayName,
                                SubModules = new List<Middle_Authentication_SubModuleDTO> { new() {
                                    SubModule= subModules[i].SubModuleName,
                                    Title = subModules[i].SubDisplayName
                                }}
                            });
                        }
                        else
                        {
                            moduleInList.SubModules.Add(new Middle_Authentication_SubModuleDTO
                            {
                                SubModule = subModules[i].SubModuleName,
                                Title = subModules[i].SubDisplayName
                            });
                        }
                    }
                }
                return Op.Succeed("بروزرسانی اطلاعات پروفایل با موفقیت انجام شد", new Response_AuthenticationDTO
                {
                    Avatar = user.Avatar,
                    Email = user.Email,
                    FullName = $"{user.Name}{(user.LastName.IsNotNull() ? $" {user.LastName}" : "")}",
                    LastName = user.LastName,
                    Modules = resultModules,
                    Name = user.Name,
                    PersonnelNumber = user.PersonnelNumber,
                    PhoneNumber = user.PhoneNumber,
                    Position = user.Position,
                    RoleDisplayName = user.Role.DisplayName,
                    RoleName = user.Role.RoleName,
                    ShortName = $"{user.Name[..1]}{(user.LastName.IsNotNull() ? $" {user.LastName[..1]}" : "")}",
                    UserId = user.UserId,
                    UserName = user.UserName,
                });

            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات پروفایل با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> CreateAdmin(long SuperAdminId, Request_CreateAdminDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateAdmin");
            if (SuperAdminId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ادمین ارسال نشده است");
            }
            if (!model.UserName.IsNotNull())
            {
                return Op.Failed("ارسال نام کاربری اجباری است");
            }
            if (model.UserName.Contains(' '))
            {
                return Op.Failed("امکان ثبت نام کاربری با 'فاصله' وجود ندارد");
            }
            if (model.UserName.Length < 2 || model.UserName.Length > 85)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام کاربری از {2.ToPersianNumber()} تا {85.ToPersianNumber()} کاراکتر است");
            }
            if (!model.Password.IsNotNull())
            {
                return Op.Failed("ارسال رمز عبور اجباری است");
            }
            if (model.Password.Length < 4 || model.Password.Length > 64)
            {
                return Op.Failed($"تعداد کاراکتر مجاز رمز عبور از {4.ToPersianNumber()} تا {64.ToPersianNumber()} کاراکتر است");
            }
            if (!model.Name.IsNotNull())
            {
                return Op.Failed("ارسال نام اجباری است");
            }
            if (model.Name.Length < 2 || model.Name.Length > 32)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.LastName.IsNotNull() && (model.LastName.Length < 2 || model.LastName.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام خانوادگی از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشد است");
            }
            if (model.Email.IsNotNull() && !model.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل بدرستی ارسال نشد است");
            }
            if (model.Position.IsNotNull() && (model.Position.Length < 2 || model.Position.Length > 42))
            {
                return Op.Failed($"تعداد کاراکتر مجاز پوزیشن از {2.ToPersianNumber()} تا {42.ToPersianNumber()} کاراکتر است");
            }
            if (model.PersonnelNumber.IsNotNull() && (!model.PersonnelNumber.IsNumeric(false) || model.PhoneNumber.Length < 2 || model.PhoneNumber.Length > 20))
            {
                return Op.Failed("شماره پرسنلی کاربر بدرستی ارسال نشده است");
            }
            if (model.Avatar.IsNotNull() && (model.Avatar.Length > 395))
            {
                return Op.Failed("آدرس تصویر بدرستی ارسال نشده است");
            }
            if (model.PrimaryParentId != null && model.PrimaryParentId <= 0)
            {
                return Op.Failed("شناسه سرپرست اصلی بدرستی ارسال نشده است");
            }
            if (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any())
            {
                if (model.SecondaryParentIDs.Any(x => x <= 0))
                {
                    return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {model.SecondaryParentIDs.FindIndex(x => x <= 0) + 1}");
                }
                if (model.PrimaryParentId != null && model.SecondaryParentIDs.Any(x => x == model.PrimaryParentId))
                {
                    return Op.Failed($"امکان تعریف یک سرپرست مشترکت اصلی و فرعی وجود ندارد، خطا در ردیف : {model.SecondaryParentIDs.FindIndex(x => x == model.PrimaryParentId) + 1}");
                }
                if (model.SecondaryParentIDs.Distinct().ToList().Count != model.SecondaryParentIDs.Count)
                {
                    return Op.Failed("امکان ارسال شناسه تکراری در لیست سرپرست های فرعی وجود ندارد");
                }
            }
            if (model.ModuleIDs == null || !model.ModuleIDs.Any())
            {
                return Op.Failed("ارسال حداقل یک ماژول برای کاربری ادمین اجباری است");
            }
            if (model.ModuleIDs.Any(x => x <= 0))
            {
                return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {model.ModuleIDs.FindIndex(x => x <= 0) + 1} لیست ماژول ها");
            }
            try
            {

                var superadmin = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == SuperAdminId, cancellationToken: cancellationToken);
                if (superadmin == null)
                {
                    return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (superadmin.Role.RoleName.ToUpper() != "SUPERADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (!superadmin.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == model.UserName.ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت ادمین با نام کاربری تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }

                var adminRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName.ToUpper() == "ADMIN", cancellationToken: cancellationToken);
                if (adminRole == null)
                {
                    return Op.Failed("دریافت نقش کاربری ادمین با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }

                if (model.PrimaryParentId != null || (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any()))
                {
                    var dbUserIDs = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                     where u.Active
                                     select u.UserId).ToList();
                    if (model.PrimaryParentId != null && !dbUserIDs.Any(x => x == model.PrimaryParentId))
                    {
                        return Op.Failed("شناسه سرپرست اصلی بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                    }
                    if (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any())
                    {
                        for (var i = 0; i < model.SecondaryParentIDs.Count; i++)
                        {
                            if (!dbUserIDs.Any(x => x == model.SecondaryParentIDs[i]))
                            {
                                return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {i + 1}", HttpStatusCode.NotFound);
                            }
                        }
                    }
                }
                var dbModules = await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken);
                var resultSubModuleIDs = new List<int>();
                for (var i = 0; i < model.ModuleIDs.Distinct().ToList().Count; i++)
                {
                    var module = dbModules.FirstOrDefault(x => x.ModuleId == model.ModuleIDs.Distinct().ToList()[i]);
                    if (module == null)
                    {
                        return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست ماژول ها");
                    }
                    resultSubModuleIDs.AddRange(module.SubModules.Select(x => x.SubModuleId).ToList());
                }
                var userData = new User
                {
                    Active = true,
                    Avatar = model.Avatar.IsNotNull() ? model.Avatar : null,
                    CreationDate = Op.OperationDate,
                    Creator = SuperAdminId,
                    Email = model.Email.IsNotNull() ? model.Email.ToEnglishNumber() : null,
                    LastName = model.LastName.IsNotNull() ? model.LastName.MultipleSpaceRemoverTrim() : null,
                    Name = model.Name.MultipleSpaceRemoverTrim(),
                    Password = this._passwordHasher.Hash(model.Password),
                    PersonnelNumber = model.PersonnelNumber.IsNotNull() ? model.PersonnelNumber.ToEnglishNumber() : null,
                    PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null,
                    Position = model.Position.IsNotNull() ? model.Position.MultipleSpaceRemoverTrim() : null,
                    RoleId = adminRole.RoleId,
                    UserName = model.UserName,
                    PrimaryParentId = model.PrimaryParentId,
                    SecodaryParentIDs = model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any() ? string.Join(",", model.SecondaryParentIDs.Distinct().ToList()) : null
                };
                await _context.Users.AddAsync(userData, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await _context.F_UserSubModules.AddRangeAsync(resultSubModuleIDs.Distinct().Select(x => new F_UserSubModule
                {
                    SubModuleId = x,
                    UserId = userData.UserId,
                }).ToList(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت ادمین با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت ادمین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> CreateUser(long SuperAdminOrAdminId, Request_CreateUserDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("CreateUser");
            if (SuperAdminOrAdminId <= 0)
            {
                return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت کاربر ارسال نشده است");
            }
            if (!model.UserName.IsNotNull())
            {
                return Op.Failed("ارسال نام کاربری اجباری است");
            }
            if (model.UserName.Contains(' '))
            {
                return Op.Failed("امکان ثبت نام کاربری با 'فاصله' وجود ندارد");
            }
            if (model.UserName.Length < 2 || model.UserName.Length > 85)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام کاربری از {2.ToPersianNumber()} تا {85.ToPersianNumber()} کاراکتر است");
            }
            if (!model.Password.IsNotNull())
            {
                return Op.Failed("ارسال رمز عبور اجباری است");
            }
            if (model.Password.Length < 4 || model.Password.Length > 64)
            {
                return Op.Failed($"تعداد کاراکتر مجاز رمز عبور از {4.ToPersianNumber()} تا {64.ToPersianNumber()} کاراکتر است");
            }
            if (!model.Name.IsNotNull())
            {
                return Op.Failed("ارسال نام اجباری است");
            }
            if (model.Name.Length < 2 || model.Name.Length > 32)
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.LastName.IsNotNull() && (model.LastName.Length < 2 || model.LastName.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام خانوادگی از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشد است");
            }
            if (model.Email.IsNotNull() && !model.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل بدرستی ارسال نشد است");
            }
            if (model.Position.IsNotNull() && (model.Position.Length < 2 || model.Position.Length > 42))
            {
                return Op.Failed($"تعداد کاراکتر مجاز پوزیشن از {2.ToPersianNumber()} تا {42.ToPersianNumber()} کاراکتر است");
            }
            if (model.PersonnelNumber.IsNotNull() && (!model.PersonnelNumber.IsNumeric(false) || model.PhoneNumber.Length < 2 || model.PhoneNumber.Length > 20))
            {
                return Op.Failed("شماره پرسنلی کاربر بدرستی ارسال نشده است");
            }
            if (model.Avatar.IsNotNull() && (model.Avatar.Length > 395))
            {
                return Op.Failed("آدرس تصویر بدرستی ارسال نشده است");
            }
            if (model.Modules == null || !model.Modules.Any())
            {
                return Op.Failed("ارسال حداقل یک ماژول برای کاربری ادمین اجباری است");
            }
            if (model.Modules.Any(x => x.ModuleId <= 0))
            {
                return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {model.Modules.FindIndex(x => x.ModuleId <= 0) + 1} لیست ماژول ها");
            }
            if (model.Modules.Any(x => x.SubModuleIDs == null || !x.SubModuleIDs.Any()))
            {
                return Op.Failed($"تعریف حداقل یک دسترسی از ماژول اجباری است. خطا در ردیف {model.Modules.FindIndex(x => x.SubModuleIDs == null || !x.SubModuleIDs.Any()) + 1} لیست ماژول ها");
            }
            if (model.Modules.Any(x => x.SubModuleIDs.Any(y => y <= 0)))
            {
                return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است. خطا در ردیف {model.Modules.FindIndex(x => x.SubModuleIDs.Any(y => y <= 0)) + 1} از لیست ماژول ها، ردیف  {model.Modules.First(x => x.SubModuleIDs.Any(y => y <= 0)).SubModuleIDs.FindIndex(x => x <= 0) + 1} از لیست دسترسی ماژول ها");
            }
            if (model.AccountIDs != null && model.AccountIDs.Any(x => x <= 0))
            {
                return Op.Failed($"شناسه حساب بدرستی ارسال نشده است. خطا در ردیف {model.AccountIDs.FindIndex(x => x <= 0) + 1} لیست حساب ها");
            }
            try
            {
                var creator = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == SuperAdminOrAdminId, cancellationToken: cancellationToken);
                if (creator == null)
                {
                    return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (creator.Role.RoleName.ToUpper() != "SUPERADMIN" && creator.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (!creator.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                var userRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName.ToUpper() == "USER", cancellationToken: cancellationToken);
                if (userRole == null)
                {
                    return Op.Failed("دریافت نقش های کاربری با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }

                List<Response_GetModuleDTO> acceptableModules = new();
                if (creator.Role.RoleName.ToUpper() == "SUPERADMIN")
                {
                    acceptableModules = (from m in await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken)
                                         select new Response_GetModuleDTO
                                         {
                                             ModuleId = m.ModuleId,
                                             ModuleName = m.ModuleName,
                                             DisplayName = m.DisplayName,
                                             SubModules = m.SubModules.Select(x => new Middle_SubModuleDTO
                                             {
                                                 SubModuleDisplayName = x.SubDisplayName,
                                                 SubModuleId = x.SubModuleId,
                                                 SubModuleName = x.SubModuleName,
                                             }).ToList()
                                         }).ToList();
                }
                else
                {
                    acceptableModules = (from m in await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken)
                                         join id in (from sub in await _context.SubModules.ToListAsync(cancellationToken: cancellationToken)
                                                     join f_sub in creator.F_UserSubModules
                                                     on sub.SubModuleId equals f_sub.SubModuleId
                                                     select sub.ModuleId).Distinct().ToList()
                                         on m.ModuleId equals id
                                         select new Response_GetModuleDTO
                                         {
                                             ModuleId = m.ModuleId,
                                             ModuleName = m.ModuleName,
                                             DisplayName = m.DisplayName,
                                             SubModules = m.SubModules.Select(x => new Middle_SubModuleDTO
                                             {
                                                 SubModuleDisplayName = x.SubDisplayName,
                                                 SubModuleId = x.SubModuleId,
                                                 SubModuleName = x.SubModuleName,
                                             }).ToList()
                                         }).ToList();
                }

                bool accessToAddAccounts = false;
                List<int> subModuleIDs = new();
                for (int i = 0; i < model.Modules.Count; i++)
                {
                    var existsModuleInList = acceptableModules.FirstOrDefault(x => x.ModuleId == model.Modules[i].ModuleId);
                    if (existsModuleInList == null)
                    {
                        return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست ماژول ها");
                    }
                    for (int j = 0; j < model.Modules[i].SubModuleIDs.Distinct().ToList().Count; j++)
                    {
                        var existSubModuleInList = existsModuleInList.SubModules.FirstOrDefault(x => x.SubModuleId == model.Modules[i].SubModuleIDs[j]);
                        if (existSubModuleInList == null)
                        {
                            return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} از لیست ماژول ها، ردیف  {j + 1} از لیست دسترسی ماژول ها");
                        }
                        subModuleIDs.Add(existSubModuleInList.SubModuleId);
                        if (existsModuleInList.ModuleName.ToUpper() == "BI" && existSubModuleInList.SubModuleName.ToUpper() == "FINANCEDATAENTRY")
                        {
                            accessToAddAccounts = true;
                        }
                    }
                }

                if (!accessToAddAccounts)
                {
                    if (model.AccountIDs != null && model.AccountIDs.Any())
                    {
                        return Op.Failed($"ثبت حساب در صورت عدم تعریف دسترسی ماژول '{(await _context.SubModules.FirstAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken)).SubDisplayName}' امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    if (model.AccountIDs != null && model.AccountIDs.Any())
                    {
                        var dbAccountIDs = (from acc in await _context.Accounts.ToListAsync(cancellationToken: cancellationToken) select acc.AccountId).ToList();

                        for (var i = 0; i < model.AccountIDs.Distinct().ToList().Count; i++)
                        {
                            if (!dbAccountIDs.Any(x => x == model.AccountIDs.Distinct().ToList()[i]))
                            {
                                return Op.Failed($"شناسه حساب بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست حساب ها");
                            }
                        }
                    }
                }
                if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == model.UserName.ToLower(), cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت کاربر با نام کاربری تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                var userData = new User
                {
                    Active = true,
                    Avatar = model.Avatar.IsNotNull() ? model.Avatar : null,
                    CreationDate = Op.OperationDate,
                    Creator = SuperAdminOrAdminId,
                    Email = model.Email.IsNotNull() ? model.Email.ToEnglishNumber() : null,
                    LastName = model.LastName.IsNotNull() ? model.LastName.MultipleSpaceRemoverTrim() : null,
                    Name = model.Name.MultipleSpaceRemoverTrim(),
                    Password = this._passwordHasher.Hash(model.Password),
                    PersonnelNumber = model.PersonnelNumber.IsNotNull() ? model.PersonnelNumber.ToEnglishNumber() : null,
                    PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null,
                    Position = model.Position.IsNotNull() ? model.Position.MultipleSpaceRemoverTrim() : null,
                    RoleId = userRole.RoleId,
                    UserName = model.UserName
                };
                await _context.Users.AddAsync(userData, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await _context.F_UserSubModules.AddRangeAsync(subModuleIDs.Distinct().Select(x => new F_UserSubModule
                {
                    SubModuleId = x,
                    UserId = userData.UserId
                }).ToList(), cancellationToken);

                if (accessToAddAccounts && model.AccountIDs != null && model.AccountIDs.Any())
                {
                    await _context.F_UserAccounts.AddRangeAsync(model.AccountIDs.Distinct().Select(x => new F_UserAccount
                    {
                        AccountId = x,
                        UserId = userData.UserId
                    }).ToList(), cancellationToken);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت کاربر با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> UpdateUser(long SuperAdminOrAdminId, Request_UpdateUserDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateUser");
            if (SuperAdminOrAdminId <= 0)
            {
                return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی کاربر ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            if (model.UserName.IsNotNull())
            {
                if (model.UserName.Contains(' '))
                {
                    return Op.Failed("امکان ثبت نام کاربری با 'فاصله' وجود ندارد");
                }
                if (model.UserName.Length < 2 || model.UserName.Length > 85)
                {
                    return Op.Failed($"تعداد کاراکتر مجاز نام کاربری از {2.ToPersianNumber()} تا {85.ToPersianNumber()} کاراکتر است");
                }
            }
            if (model.Password.IsNotNull() && (model.Password.Length < 4 || model.Password.Length > 64))
            {
                return Op.Failed($"تعداد کاراکتر مجاز رمز عبور از {4.ToPersianNumber()} تا {64.ToPersianNumber()} کاراکتر است");
            }
            if (model.Name.IsNotNull() && (model.Name.Length < 2 || model.Name.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.LastName.IsNotNull() && (model.LastName.Length < 2 || model.LastName.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام خانوادگی از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشد است");
            }
            if (model.Email.IsNotNull() && !model.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل بدرستی ارسال نشد است");
            }
            if (model.Position.IsNotNull() && (model.Position.Length < 2 || model.Position.Length > 42))
            {
                return Op.Failed($"تعداد کاراکتر مجاز پوزیشن از {2.ToPersianNumber()} تا {42.ToPersianNumber()} کاراکتر است");
            }
            if (model.PersonnelNumber.IsNotNull() && (!model.PersonnelNumber.IsNumeric(false) || model.PhoneNumber.Length < 2 || model.PhoneNumber.Length > 20))
            {
                return Op.Failed("شماره پرسنلی کاربر بدرستی ارسال نشده است");
            }
            if (model.Avatar.IsNotNull() && (model.Avatar.Length > 395))
            {
                return Op.Failed("آدرس تصویر بدرستی ارسال نشده است");
            }
            if (model.Modules == null || !model.Modules.Any())
            {
                return Op.Failed("ارسال حداقل یک ماژول برای کاربری ادمین اجباری است");
            }
            if (model.Modules.Any(x => x.ModuleId <= 0))
            {
                return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {model.Modules.FindIndex(x => x.ModuleId <= 0) + 1} لیست ماژول ها");
            }
            if (model.Modules.Any(x => x.SubModuleIDs == null || !x.SubModuleIDs.Any()))
            {
                return Op.Failed($"تعریف حداقل یک دسترسی از ماژول اجباری است. خطا در ردیف {model.Modules.FindIndex(x => x.SubModuleIDs == null || !x.SubModuleIDs.Any()) + 1} لیست ماژول ها");
            }
            if (model.Modules.Any(x => x.SubModuleIDs.Any(y => y <= 0)))
            {
                return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است. خطا در ردیف {model.Modules.FindIndex(x => x.SubModuleIDs.Any(y => y <= 0)) + 1} از لیست ماژول ها، ردیف  {model.Modules.First(x => x.SubModuleIDs.Any(y => y <= 0)).SubModuleIDs.FindIndex(x => x <= 0) + 1} از لیست دسترسی ماژول ها");
            }
            if (model.AccountIDs != null && model.AccountIDs.Any(x => x <= 0))
            {
                return Op.Failed($"شناسه حساب بدرستی ارسال نشده است. خطا در ردیف {model.AccountIDs.FindIndex(x => x <= 0) + 1} لیست حساب ها");
            }
            try
            {

                var creator = await _context.Users.Include(x => x.Role).Include(x => x.F_UserSubModules).FirstOrDefaultAsync(x => x.UserId == SuperAdminOrAdminId, cancellationToken: cancellationToken);
                if (creator == null)
                {
                    return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (creator.Role.RoleName.ToUpper() != "SUPERADMIN" && creator.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (!creator.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == model.UserId, cancellationToken: cancellationToken);
                if (user == null || user.Role.RoleName.ToUpper() != "USER")
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                List<Response_GetModuleDTO> acceptableModules = new();
                if (creator.Role.RoleName.ToUpper() == "SUPERADMIN")
                {
                    acceptableModules = (from m in await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken)
                                         select new Response_GetModuleDTO
                                         {
                                             ModuleId = m.ModuleId,
                                             ModuleName = m.ModuleName,
                                             DisplayName = m.DisplayName,
                                             SubModules = m.SubModules.Select(x => new Middle_SubModuleDTO
                                             {
                                                 SubModuleDisplayName = x.SubDisplayName,
                                                 SubModuleId = x.SubModuleId,
                                                 SubModuleName = x.SubModuleName,
                                             }).ToList()
                                         }).ToList();
                }
                else
                {
                    acceptableModules = (from m in await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken)
                                         join id in (from sub in await _context.SubModules.ToListAsync(cancellationToken: cancellationToken)
                                                     join f_sub in creator.F_UserSubModules
                                                     on sub.SubModuleId equals f_sub.SubModuleId
                                                     select sub.ModuleId).Distinct().ToList()
                                         on m.ModuleId equals id
                                         select new Response_GetModuleDTO
                                         {
                                             ModuleId = m.ModuleId,
                                             ModuleName = m.ModuleName,
                                             DisplayName = m.DisplayName,
                                             SubModules = m.SubModules.Select(x => new Middle_SubModuleDTO
                                             {
                                                 SubModuleDisplayName = x.SubDisplayName,
                                                 SubModuleId = x.SubModuleId,
                                                 SubModuleName = x.SubModuleName,
                                             }).ToList()
                                         }).ToList();
                }

                bool accessToAddAccounts = false;
                List<int> subModuleIDs = new();
                for (int i = 0; i < model.Modules.Count; i++)
                {
                    var existsModuleInList = acceptableModules.FirstOrDefault(x => x.ModuleId == model.Modules[i].ModuleId);
                    if (existsModuleInList == null)
                    {
                        return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست ماژول ها");
                    }
                    for (int j = 0; j < model.Modules[i].SubModuleIDs.Distinct().ToList().Count; j++)
                    {
                        var existSubModuleInList = existsModuleInList.SubModules.FirstOrDefault(x => x.SubModuleId == model.Modules[i].SubModuleIDs[j]);
                        if (existSubModuleInList == null)
                        {
                            return Op.Failed($"شناسه دسترسی ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} از لیست ماژول ها، ردیف  {j + 1} از لیست دسترسی ماژول ها");
                        }
                        subModuleIDs.Add(existSubModuleInList.SubModuleId);
                        if (existsModuleInList.ModuleName.ToUpper() == "BI" && existSubModuleInList.SubModuleName.ToUpper() == "FINANCEDATAENTRY")
                        {
                            accessToAddAccounts = true;
                        }
                    }
                }

                if (!accessToAddAccounts)
                {
                    if (model.AccountIDs != null && model.AccountIDs.Any())
                    {
                        return Op.Failed($"بروزرسانی حساب در صورت عدم تعریف دسترسی ماژول '{(await _context.SubModules.FirstAsync(x => x.SubModuleName.ToUpper() == "FINANCEDATAENTRY", cancellationToken: cancellationToken)).SubDisplayName}' امکانپذیر نمیباشد", HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    if (model.AccountIDs != null && model.AccountIDs.Any())
                    {
                        var dbAccountIDs = (from acc in await _context.Accounts.ToListAsync(cancellationToken: cancellationToken) select acc.AccountId).ToList();

                        for (var i = 0; i < model.AccountIDs.Distinct().ToList().Count; i++)
                        {
                            if (!dbAccountIDs.Any(x => x == model.AccountIDs.Distinct().ToList()[i]))
                            {
                                return Op.Failed($"شناسه حساب بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست حساب ها");
                            }
                        }
                    }
                }

                if (model.UserName.IsNotNull() && await _context.Users.AnyAsync(x => x.UserName.ToLower() == model.UserName.ToLower() && x.UserId != user.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("بروزرسانی کاربر با نام کاربری تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }

                if (model.UserName.IsNotNull())
                {
                    user.UserName = model.UserName;
                }
                if (model.Password.IsNotNull())
                {
                    user.Password = _passwordHasher.Hash(model.Password);
                }
                if (model.Active != null)
                {
                    user.Active = Convert.ToBoolean(model.Active);
                }
                if (model.Name.IsNotNull())
                {
                    user.Name = model.Name.MultipleSpaceRemoverTrim();
                }
                user.LastName = model.LastName.IsNotNull() ? model.LastName.MultipleSpaceRemoverTrim() : null;
                user.PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null;
                user.Email = model.Email.IsNotNull() ? model.Email.ToEnglishNumber() : null;
                user.Position = model.Position.IsNotNull() ? model.Position.MultipleSpaceRemoverTrim() : null;
                user.PersonnelNumber = model.PersonnelNumber.IsNotNull() ? model.PersonnelNumber.ToEnglishNumber() : null;
                if (model.Avatar.IsNotNull())
                {
                    user.Avatar = model.Avatar;
                }
                user.LastModificationDate = Op.OperationDate;
                user.LastModifier = SuperAdminOrAdminId;
                _context.Entry<User>(user).State = EntityState.Modified;

                _context.F_UserSubModules.RemoveRange(await _context.F_UserSubModules.Where(x => x.UserId == user.UserId).ToListAsync(cancellationToken: cancellationToken));
                await _context.F_UserSubModules.AddRangeAsync(subModuleIDs.Distinct().Select(x => new F_UserSubModule
                {
                    SubModuleId = x,
                    UserId = user.UserId
                }).ToList(), cancellationToken);
                _context.F_UserAccounts.RemoveRange(await _context.F_UserAccounts.Where(x => x.UserId == user.UserId).ToListAsync(cancellationToken: cancellationToken));
                if (accessToAddAccounts && model.AccountIDs != null && model.AccountIDs.Any())
                {
                    await _context.F_UserAccounts.AddRangeAsync(model.AccountIDs.Distinct().Select(x => new F_UserAccount
                    {
                        AccountId = x,
                        UserId = user.UserId
                    }).ToList(), cancellationToken);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی کاربر با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<object>> UpdateAdmin(long SuperAdminId, Request_UpdateAdminDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateAdmin");
            if (SuperAdminId <= 0)
            {
                return Op.Failed("شناسه کاربری بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ادمین ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            if (model.UserName.IsNotNull())
            {
                if (model.UserName.Contains(' '))
                {
                    return Op.Failed("امکان ثبت نام کاربری با 'فاصله' وجود ندارد");
                }
                if (model.UserName.Length < 2 || model.UserName.Length > 85)
                {
                    return Op.Failed($"تعداد کاراکتر مجاز نام کاربری از {2.ToPersianNumber()} تا {85.ToPersianNumber()} کاراکتر است");
                }
            }
            if (model.Password.IsNotNull() && (model.Password.Length < 4 || model.Password.Length > 64))
            {
                return Op.Failed($"تعداد کاراکتر مجاز رمز عبور از {4.ToPersianNumber()} تا {64.ToPersianNumber()} کاراکتر است");
            }
            if (model.Name.IsNotNull() && (model.Name.Length < 2 || model.Name.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.LastName.IsNotNull() && (model.LastName.Length < 2 || model.LastName.Length > 32))
            {
                return Op.Failed($"تعداد کاراکتر مجاز نام خانوادگی از {2.ToPersianNumber()} تا {32.ToPersianNumber()} کاراکتر است");
            }
            if (model.PhoneNumber.IsNotNull() && !model.PhoneNumber.IsMobile())
            {
                return Op.Failed("شماره تماس بدرستی ارسال نشد است");
            }
            if (model.Email.IsNotNull() && !model.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل بدرستی ارسال نشد است");
            }
            if (model.Position.IsNotNull() && (model.Position.Length < 2 || model.Position.Length > 42))
            {
                return Op.Failed($"تعداد کاراکتر مجاز پوزیشن از {2.ToPersianNumber()} تا {42.ToPersianNumber()} کاراکتر است");
            }
            if (model.PersonnelNumber.IsNotNull() && (!model.PersonnelNumber.IsNumeric(false) || model.PhoneNumber.Length < 2 || model.PhoneNumber.Length > 20))
            {
                return Op.Failed("شماره پرسنلی کاربر بدرستی ارسال نشده است");
            }
            if (model.Avatar.IsNotNull() && (model.Avatar.Length > 395))
            {
                return Op.Failed("آدرس تصویر بدرستی ارسال نشده است");
            }
            if (model.PrimaryParentId != null && model.PrimaryParentId <= 0)
            {
                return Op.Failed("شناسه سرپرست اصلی بدرستی ارسال نشده است");
            }
            if (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any())
            {
                if (model.SecondaryParentIDs.Any(x => x <= 0))
                {
                    return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {model.SecondaryParentIDs.FindIndex(x => x <= 0) + 1}");
                }
                if (model.PrimaryParentId != null && model.SecondaryParentIDs.Any(x => x == model.PrimaryParentId))
                {
                    return Op.Failed($"امکان تعریف یک سرپرست مشترکت اصلی و فرعی وجود ندارد، خطا در ردیف : {model.SecondaryParentIDs.FindIndex(x => x == model.PrimaryParentId) + 1}");
                }
                if (model.SecondaryParentIDs.Distinct().ToList().Count != model.SecondaryParentIDs.Count)
                {
                    return Op.Failed("امکان ارسال شناسه تکراری در لیست سرپرست های فرعی وجود ندارد");
                }
            }
            if (model.ModuleIDs == null || !model.ModuleIDs.Any())
            {
                return Op.Failed("ارسال حداقل یک ماژول برای کاربری ادمین اجباری است");
            }
            if (model.ModuleIDs.Any(x => x <= 0))
            {
                return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {model.ModuleIDs.FindIndex(x => x <= 0) + 1} لیست ماژول ها");
            }
            try
            {
                var superadmin = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == SuperAdminId, cancellationToken: cancellationToken);
                if (superadmin == null)
                {
                    return Op.Failed("اطلاعات کاربر ثبت کننده بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (superadmin.Role.RoleName.ToUpper() != "SUPERADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (!superadmin.Active)
                {
                    return Op.Failed("کاربری شما غیر فعال است", HttpStatusCode.Forbidden);
                }
                var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == model.UserId, cancellationToken: cancellationToken);
                if (user == null || user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.UserName.IsNotNull() && await _context.Users.AnyAsync(x => x.UserName.ToLower() == model.UserName.ToLower() && x.UserId != user.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("ثبت ادمین با نام کاربری تکراری امکانپذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (model.PrimaryParentId != null || (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any()))
                {
                    var dbUsers = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                   where u.Active
                                   select new Middle_SimpleUserDTO
                                   {
                                       PrimaryParentId = u.PrimaryParentId,
                                       RoleId = u.RoleId,
                                       UserId = u.UserId,
                                       SecodaryParentIDs = u.SecodaryParentIDs.IsNotNull() ? u.SecodaryParentIDs.Split(",").Select(x => Convert.ToInt64(x)).ToList() : null
                                   }).ToList();
                    if (model.PrimaryParentId != null)
                    {
                        var parent = dbUsers.FirstOrDefault(x => x.UserId == model.PrimaryParentId);
                        if (parent == null)
                        {
                            return Op.Failed("شناسه سرپرست اصلی بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                        }
                        if (parent.PrimaryParentId != null && parent.PrimaryParentId == model.UserId)
                        {
                            return Op.Failed("شناسه سرپرست اصلی بدرستی ارسال نشده است", HttpStatusCode.Conflict);
                        }
                    }
                    if (model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any())
                    {
                        for (var i = 0; i < model.SecondaryParentIDs.Count; i++)
                        {
                            var parent = dbUsers.FirstOrDefault(x => x.UserId == model.SecondaryParentIDs[i]);
                            if (parent == null)
                            {
                                return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {i + 1}", HttpStatusCode.NotFound);
                            }
                            if (parent.PrimaryParentId != null && parent.PrimaryParentId == model.UserId)
                            {
                                return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {i + 1}", HttpStatusCode.Conflict);
                            }
                            if (parent.SecodaryParentIDs != null && parent.SecodaryParentIDs.Any(x => x == model.UserId))
                            {
                                return Op.Failed($"شناسه سرپرست فرعی بدرستی ارسال نشده است، خطا در ردیف : {i + 1}", HttpStatusCode.Conflict);
                            }
                        }
                    }
                }
                var dbModules = await _context.Modules.Include(x => x.SubModules).ToListAsync(cancellationToken: cancellationToken);
                var resultSubModuleIDs = new List<int>();
                for (var i = 0; i < model.ModuleIDs.Distinct().ToList().Count; i++)
                {
                    var module = dbModules.FirstOrDefault(x => x.ModuleId == model.ModuleIDs.Distinct().ToList()[i]);
                    if (module == null)
                    {
                        return Op.Failed($"شناسه ماژول بدرستی ارسال نشده است. خطا در ردیف {i + 1} لیست ماژول ها");
                    }
                    resultSubModuleIDs.AddRange(module.SubModules.Select(x => x.SubModuleId).ToList());
                }

                if (model.UserName.IsNotNull())
                {
                    user.UserName = model.UserName;
                }
                if (model.Password.IsNotNull())
                {
                    user.Password = _passwordHasher.Hash(model.Password);
                }
                if (model.Active != null)
                {
                    user.Active = Convert.ToBoolean(model.Active);
                }
                if (model.Name.IsNotNull())
                {
                    user.Name = model.Name.MultipleSpaceRemoverTrim();
                }
                user.LastName = model.LastName.IsNotNull() ? model.LastName.MultipleSpaceRemoverTrim() : null;
                user.PhoneNumber = model.PhoneNumber.IsNotNull() ? model.PhoneNumber.ToEnglishNumber() : null;
                user.Email = model.Email.IsNotNull() ? model.Email.ToEnglishNumber() : null;
                user.Position = model.Position.IsNotNull() ? model.Position.MultipleSpaceRemoverTrim() : null;
                user.PersonnelNumber = model.PersonnelNumber.IsNotNull() ? model.PersonnelNumber.ToEnglishNumber() : null;
                if (model.Avatar.IsNotNull())
                {
                    user.Avatar = model.Avatar;
                }
                user.PrimaryParentId = model.PrimaryParentId;
                user.SecodaryParentIDs = model.SecondaryParentIDs != null && model.SecondaryParentIDs.Any() ? string.Join(",", model.SecondaryParentIDs.Distinct().ToList()) : null;
                user.LastModificationDate = Op.OperationDate;
                user.LastModifier = SuperAdminId;
                _context.Entry<User>(user).State = EntityState.Modified;

                _context.F_UserSubModules.RemoveRange(await _context.F_UserSubModules.Where(x => x.UserId == user.UserId).ToListAsync(cancellationToken: cancellationToken));
                await _context.F_UserSubModules.AddRangeAsync(resultSubModuleIDs.Distinct().Select(x => new F_UserSubModule
                {
                    SubModuleId = x,
                    UserId = user.UserId,
                }).ToList(), cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Op.Succeed("بروزرسانی اطلاعات ادمین با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات ادمین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PagedList<Respone_GetUserDTO>>> GetUsers(long UserId, Filter_GetUserDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            OperationResult<PagedList<Respone_GetUserDTO>> Op = new("GetUsers");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (filter != null)
            {
                if (filter.UserId != null && filter.UserId <= 0)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
                }
                if (filter.RoleId != null && filter.RoleId <= 0)
                {
                    return Op.Failed("شناسه نقش کاربری بدرستی ارسال نشده است");
                }
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
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                if (filter != null)
                {
                    if (filter.RoleId != null && !await _context.Roles.AnyAsync(x => x.RoleId == filter.RoleId, cancellationToken: cancellationToken))
                    {
                        return Op.Failed("شناسه نقش کاربری بدرستی ارسال نشده است");
                    }
                    if (filter.UserId != null && !await _context.Users.AnyAsync(x => x.UserId == UserId, cancellationToken: cancellationToken))
                    {
                        return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
                    }
                }
                var result = new List<Respone_GetUserDTO>();
                var safeSearchString = filter != null && filter.Search.IsNotNull() ? filter.Search.MultipleSpaceRemoverTrim().ToLower() : null;
                long totalCount = Convert.ToInt64((from u in await _context.Users.Include(x => x.Role).Include(x => x.F_UserAccounts).ThenInclude(x => x.Account).ToListAsync(cancellationToken: cancellationToken)
                                                   where (u.Role.RoleName.ToUpper() != "SUPERADMIN") && (user.Role.RoleName.ToUpper() == "SUPERADMIN" || u.Role.RoleName.ToUpper() != "ADMIN") &&
                                                      (filter == null || (
                                                              (filter.UserId == null || u.UserId == filter.UserId) &&
                                                              (filter.Active == null || u.Active == Convert.ToBoolean(filter.Active)) &&
                                                              (filter.RoleId == null || u.RoleId == filter.RoleId) &&
                                                              (filter.CreationDate == null || u.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                                              (!safeSearchString.IsNotNull() || u.UserName.ToLower().Contains(safeSearchString) || $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}".ToLower().Contains(safeSearchString) || (u.PhoneNumber.IsNotNull() ? u.PhoneNumber : "").ToLower().Contains(safeSearchString) || (u.Email.IsNotNull() ? u.Email : "").ToLower().Contains(safeSearchString) || (u.Position.IsNotNull() ? u.Position : "").ToLower().Contains(safeSearchString) || (u.PersonnelNumber.IsNotNull() ? u.PersonnelNumber : "").ToLower().Contains(safeSearchString))
                                                          )
                                                      )
                                                   select u.UserId).ToList().Count);
                if (totalCount == 0)
                {
                    return Op.Succeed("دریافت لیست کاربران با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }

                var allModules = await _context.Modules.ToListAsync(cancellationToken: cancellationToken);
                var allUsers = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                select new Middle_ParentUserDTO
                                {
                                    FullName = $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}",
                                    Position = u.Position,
                                    UserId = u.UserId,
                                    UserName = u.UserName,
                                }).ToList();
                if (page == null || page.Contain == null || page.Contain <= 0)
                {
                    result = (from u in await _context.Users.Include(x => x.Role).Include(x => x.F_UserAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Company).Include(x => x.F_UserSubModules).ThenInclude(x => x.SubModule).ToListAsync(cancellationToken: cancellationToken)
                              where (u.Role.RoleName.ToUpper() != "SUPERADMIN") && (user.Role.RoleName.ToUpper() == "SUPERADMIN" || u.Role.RoleName.ToUpper() != "ADMIN") &&
                                 (filter == null || (
                                         (filter.UserId == null || u.UserId == filter.UserId) &&
                                         (filter.Active == null || u.Active == Convert.ToBoolean(filter.Active)) &&
                                         (filter.RoleId == null || u.RoleId == filter.RoleId) &&
                                         (filter.CreationDate == null || u.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                         (!safeSearchString.IsNotNull() || u.UserName.ToLower().Contains(safeSearchString) || $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}".ToLower().Contains(safeSearchString) || (u.PhoneNumber.IsNotNull() ? u.PhoneNumber : "").ToLower().Contains(safeSearchString) || (u.Email.IsNotNull() ? u.Email : "").ToLower().Contains(safeSearchString) || (u.Position.IsNotNull() ? u.Position : "").ToLower().Contains(safeSearchString) || (u.PersonnelNumber.IsNotNull() ? u.PersonnelNumber : "").ToLower().Contains(safeSearchString))
                                     )
                                 )
                              select new Respone_GetUserDTO
                              {
                                  Accounts = u.F_UserAccounts == null || !u.F_UserAccounts.Any() ? null : u.F_UserAccounts.Select(x => new Domain.Contracts.DTOs.Account.Response_GetAccountDTO
                                  {
                                      AccountId = x.AccountId,
                                      AccountNumber = x.Account.AccountNumber,
                                      AccountType = x.Account.AccountType,
                                      BankName = x.Account.BankName,
                                      BelongsToCentralOffice = x.Account.BelongsToCentralOffice,
                                      CompanyCode = x.Account.Company.CompanyCode,
                                      CompanyId = x.Account.CompanyId,
                                      CompanyName = x.Account.Company.Name,
                                      CompanyNumber = x.Account.Company.CompanyNumber,
                                      CreationDate = x.Account.CreationDate,
                                      Description = x.Account.Description,
                                      InterestRatePercent = x.Account.InterestRatePercent,
                                      LastModificationDate = x.Account.LastModificationDate,
                                      ProjectName = x.Account.ProjectName,
                                  }).ToList(),
                                  Active = u.Active,
                                  Avatar = u.Avatar,
                                  CreationDate = u.CreationDate,
                                  Email = u.Email,
                                  FullName = $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}",
                                  LastModificationDate = u.LastModificationDate,
                                  LastName = u.LastName,
                                  Modules = _GetModulesBySubModules(allModules, u.F_UserSubModules == null || !u.F_UserSubModules.Any() ? null : u.F_UserSubModules.Select(x => x.SubModule).ToList()),
                                  Name = u.Name,
                                  PersonnelNumber = u.PersonnelNumber,
                                  PhoneNumber = u.PhoneNumber,
                                  Position = u.Position,
                                  RoleDisplayName = u.Role.DisplayName,
                                  RoleId = u.RoleId,
                                  RoleName = u.Role.RoleName,
                                  UserId = u.UserId,
                                  UserName = u.UserName,
                                  PrimaryParent = u.PrimaryParentId == null ? null : allUsers.FirstOrDefault(x => x.UserId == u.PrimaryParentId),
                                  SecondaryParents = _GetSecondaryParentsByIDs(allUsers, u.SecodaryParentIDs)

                              }).OrderBy(x => x.RoleId).ThenByDescending(x => x.CreationDate).ToList();
                }
                else
                {
                    if (page.Number == null || page.Number <= 0)
                    {
                        page.Number = 1;
                    }
                    result = (from u in await _context.Users.Include(x => x.Role).Include(x => x.F_UserAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Company).Include(x => x.F_UserSubModules).ThenInclude(x => x.SubModule).ToListAsync(cancellationToken: cancellationToken)
                              where (u.Role.RoleName.ToUpper() != "SUPERADMIN") && (user.Role.RoleName.ToUpper() == "SUPERADMIN" || u.Role.RoleName.ToUpper() != "ADMIN") &&
                                 (filter == null || (
                                         (filter.UserId == null || u.UserId == filter.UserId) &&
                                         (filter.Active == null || u.Active == Convert.ToBoolean(filter.Active)) &&
                                         (filter.RoleId == null || u.RoleId == filter.RoleId) &&
                                         (filter.CreationDate == null || u.CreationDate.ResetTime() == filter.CreationDate.ResetTime()) &&
                                         (!safeSearchString.IsNotNull() || u.UserName.ToLower().Contains(safeSearchString) || $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}".ToLower().Contains(safeSearchString) || (u.PhoneNumber.IsNotNull() ? u.PhoneNumber : "").ToLower().Contains(safeSearchString) || (u.Email.IsNotNull() ? u.Email : "").ToLower().Contains(safeSearchString) || (u.Position.IsNotNull() ? u.Position : "").ToLower().Contains(safeSearchString) || (u.PersonnelNumber.IsNotNull() ? u.PersonnelNumber : "").ToLower().Contains(safeSearchString))
                                     )
                                 )
                              select new Respone_GetUserDTO
                              {
                                  Accounts = u.F_UserAccounts == null || !u.F_UserAccounts.Any() ? null : u.F_UserAccounts.Select(x => new Domain.Contracts.DTOs.Account.Response_GetAccountDTO
                                  {
                                      AccountId = x.AccountId,
                                      AccountNumber = x.Account.AccountNumber,
                                      AccountType = x.Account.AccountType,
                                      BankName = x.Account.BankName,
                                      BelongsToCentralOffice = x.Account.BelongsToCentralOffice,
                                      CompanyCode = x.Account.Company.CompanyCode,
                                      CompanyId = x.Account.CompanyId,
                                      CompanyName = x.Account.Company.Name,
                                      CompanyNumber = x.Account.Company.CompanyNumber,
                                      CreationDate = x.Account.CreationDate,
                                      Description = x.Account.Description,
                                      InterestRatePercent = x.Account.InterestRatePercent,
                                      LastModificationDate = x.Account.LastModificationDate,
                                      ProjectName = x.Account.ProjectName,
                                  }).ToList(),
                                  Active = u.Active,
                                  Avatar = u.Avatar,
                                  CreationDate = u.CreationDate,
                                  Email = u.Email,
                                  FullName = $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}",
                                  LastModificationDate = u.LastModificationDate,
                                  LastName = u.LastName,
                                  Modules = _GetModulesBySubModules(allModules, u.F_UserSubModules == null || !u.F_UserSubModules.Any() ? null : u.F_UserSubModules.Select(x => x.SubModule).ToList()),
                                  Name = u.Name,
                                  PersonnelNumber = u.PersonnelNumber,
                                  PhoneNumber = u.PhoneNumber,
                                  Position = u.Position,
                                  RoleDisplayName = u.Role.DisplayName,
                                  RoleId = u.RoleId,
                                  RoleName = u.Role.RoleName,
                                  UserId = u.UserId,
                                  UserName = u.UserName,
                                  PrimaryParent = u.PrimaryParentId == null ? null : allUsers.FirstOrDefault(x => x.UserId == u.PrimaryParentId),
                                  SecondaryParents = _GetSecondaryParentsByIDs(allUsers, u.SecodaryParentIDs)
                              }).OrderBy(x => x.RoleId).ThenByDescending(x => x.CreationDate).Skip((Convert.ToInt32(page.Number) - 1) * Convert.ToInt32(page.Contain)).Take(Convert.ToInt32(page.Contain)).ToList();
                }
                return Op.Succeed("دریافت لیست کاربران با موفقیت انجام شد", new PagedList<Respone_GetUserDTO>(result, totalCount, page));

            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست کاربران با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<Respone_GetUserDTO>> GetUser(long UserId, Request_UserIdDTO RequestModel, CancellationToken cancellationToken = default)
        {
            OperationResult<Respone_GetUserDTO> Op = new("GetUser");
            if (UserId <= 0)
            {
                return Op.Failed("اطلاعات کاربری بدرستی ارسال نشده است");
            }
            if (RequestModel == null || RequestModel.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
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
                if (user.Role.RoleName.ToUpper() != "SUPERADMIN" && user.Role.RoleName.ToUpper() != "ADMIN")
                {
                    return Op.Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden);
                }
                var resultUser = await _context.Users.Include(x => x.Role).Include(x => x.F_UserAccounts).ThenInclude(x => x.Account).ThenInclude(x => x.Company).Include(x => x.F_UserSubModules).ThenInclude(x => x.SubModule).FirstOrDefaultAsync(x => (x.UserId == RequestModel.UserId) && (x.Role.RoleName.ToUpper() != "SUPERADMIN") && (user.Role.RoleName.ToUpper() == "SUPERADMIN" || x.Role.RoleName.ToUpper() != "ADMIN"), cancellationToken: cancellationToken);
                if (resultUser == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var allModules = await _context.Modules.ToListAsync(cancellationToken: cancellationToken);
                var allUsers = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                select new Middle_ParentUserDTO
                                {
                                    FullName = $"{u.Name}{(u.LastName.IsNotNull() ? $" {u.LastName}" : "")}",
                                    Position = u.Position,
                                    UserId = u.UserId,
                                    UserName = u.UserName,
                                }).ToList();
                return Op.Succeed("دریافت اطلاعات کاربر با موفقیت انجام شد", new Respone_GetUserDTO
                {
                    Accounts = resultUser.F_UserAccounts == null || !resultUser.F_UserAccounts.Any() ? null : resultUser.F_UserAccounts.Select(x => new Domain.Contracts.DTOs.Account.Response_GetAccountDTO
                    {
                        AccountId = x.AccountId,
                        AccountNumber = x.Account.AccountNumber,
                        AccountType = x.Account.AccountType,
                        BankName = x.Account.BankName,
                        BelongsToCentralOffice = x.Account.BelongsToCentralOffice,
                        CompanyCode = x.Account.Company.CompanyCode,
                        CompanyId = x.Account.CompanyId,
                        CompanyName = x.Account.Company.Name,
                        CompanyNumber = x.Account.Company.CompanyNumber,
                        CreationDate = x.Account.CreationDate,
                        Description = x.Account.Description,
                        InterestRatePercent = x.Account.InterestRatePercent,
                        LastModificationDate = x.Account.LastModificationDate,
                        ProjectName = x.Account.ProjectName,
                    }).ToList(),
                    Active = resultUser.Active,
                    Avatar = resultUser.Avatar,
                    CreationDate = resultUser.CreationDate,
                    Email = resultUser.Email,
                    FullName = $"{resultUser.Name}{(resultUser.LastName.IsNotNull() ? $" {resultUser.LastName}" : "")}",
                    LastModificationDate = resultUser.LastModificationDate,
                    LastName = resultUser.LastName,
                    Modules = _GetModulesBySubModules(allModules, resultUser.F_UserSubModules == null || !resultUser.F_UserSubModules.Any() ? null : resultUser.F_UserSubModules.Select(x => x.SubModule).ToList()),
                    Name = resultUser.Name,
                    PersonnelNumber = resultUser.PersonnelNumber,
                    PhoneNumber = resultUser.PhoneNumber,
                    Position = resultUser.Position,
                    RoleDisplayName = resultUser.Role.DisplayName,
                    RoleId = resultUser.RoleId,
                    RoleName = resultUser.Role.RoleName,
                    UserId = resultUser.UserId,
                    UserName = resultUser.UserName,
                    PrimaryParent = resultUser.PrimaryParentId == null ? null : allUsers.FirstOrDefault(x => x.UserId == resultUser.PrimaryParentId),
                    SecondaryParents = _GetSecondaryParentsByIDs(allUsers, resultUser.SecodaryParentIDs)
                });

            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        private List<Response_GetModuleDTO> _GetModulesBySubModules(List<Module>? allModules, List<SubModule>? subModules)
        {
            var result = new List<Response_GetModuleDTO>();
            if (allModules == null || !allModules.Any() || subModules == null || !subModules.Any())
            {
                return result;
            }
            for (int i = 0; i < subModules.Count; i++)
            {
                var moduleInList = result.FirstOrDefault(x => x.ModuleId == subModules[i].ModuleId);
                if (moduleInList == null)
                {
                    result.Add(new Response_GetModuleDTO
                    {
                        ModuleId = subModules[i].ModuleId,
                        ModuleName = allModules.First(x => x.ModuleId == subModules[i].ModuleId).ModuleName,
                        DisplayName = allModules.First(x => x.ModuleId == subModules[i].ModuleId).DisplayName,
                        SubModules = new List<Middle_SubModuleDTO>()
                        {
                            new()
                            {
                                SubModuleId = subModules[i].SubModuleId,
                                SubModuleDisplayName = subModules[i].SubDisplayName,
                                SubModuleName = subModules[i].SubModuleName,
                            }
                        }
                    });
                }
                else
                {
                    moduleInList.SubModules.Add(new Middle_SubModuleDTO
                    {
                        SubModuleId = subModules[i].SubModuleId,
                        SubModuleDisplayName = subModules[i].SubDisplayName,
                        SubModuleName = subModules[i].SubModuleName,
                    });
                }
            }
            return result;
        }

        private List<Middle_ParentUserDTO>? _GetSecondaryParentsByIDs(List<Middle_ParentUserDTO>? allUsers, string IDs)
        {
            if (allUsers == null || !allUsers.Any() || !IDs.IsNotNull())
            {
                return null;
            }
            try
            {
                return (from user in allUsers
                        join id in IDs.Split(",").Select(x => Convert.ToInt64(x)).ToList()
                        on user.UserId equals id
                        select new Middle_ParentUserDTO
                        {
                            FullName = user.FullName,
                            Position = user.Position,
                            UserId = user.UserId,
                            UserName = user.UserName,
                        }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
