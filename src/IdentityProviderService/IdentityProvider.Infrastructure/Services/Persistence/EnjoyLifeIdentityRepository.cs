using IdentityProvider.Application.Contracts.Authentication.OTPVerification;
using IdentityProvider.Application.Contracts.UserInfo;
using IdentityProvider.Application.Framework;
using IdentityProvider.Application.Helper;
using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Domain.DomainModelDTOs;
using IdentityProvider.Domain.DomainModelDTOs.Users;
using IdentityProvider.Domain.Models.EnjoyLifeRole;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using IdentityProvider.Infrastructure.Framework.Security.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace IdentityProvider.Infrastructure.Services.Persistence;

public class EnjoyLifeIdentityRepository : IEnjoyLifeIdentityRepository
{
    private readonly EnjoyLifeIdentityDbContext _context;
    private readonly UserManager<EnjoyLifeUser> _userManager;
    private readonly RoleManager<EnjoyLifeRole> _roleManager;
    private readonly SignInManager<EnjoyLifeUser> _signInManager;
    private readonly IPasswordHasher _passwordHasher;
    public EnjoyLifeIdentityRepository(EnjoyLifeIdentityDbContext context, UserManager<EnjoyLifeUser> userManager, RoleManager<EnjoyLifeRole> roleManager, SignInManager<EnjoyLifeUser> signInManager, IPasswordHasher passwordHasher)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _passwordHasher = passwordHasher;
    }
    public async Task SeedAdmin()
    {
        var user = new EnjoyLifeUser() { Email = "Admin@Enjoylife.com", PhoneNumber = "09101354601", PhoneNumberConfirmed = true, UserName = "hooman" };

        await _userManager.CreateAsync(user);
        await _roleManager.CreateAsync(new EnjoyLifeRole() { Name = "Admin" });
        await _userManager.AddToRoleAsync(user, "Admin");
    }
    public async Task<EnjoyLifeUser?> GetUserByPhoneNumber(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _context.Users.Where(u => u.PhoneNumber == phoneNumber).SingleOrDefaultAsync(cancellationToken);
    }

    public bool UserExistsByPhoneNumber(string phoneNumber)
    {
        return _context.Users.Any(u => u.PhoneNumber == phoneNumber && u.PhoneNumberConfirmed == true);
    }
    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(EnjoyLifeUser user)
    {
        List<Claim> claims = new();
        var defaultClaims = (await _signInManager.ClaimsFactory.CreateAsync(user)).Claims;
        claims.AddRange(defaultClaims);
        claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
        //claims.Add(new Claim(ClaimTypes.Role, user.CoreId));


        return claims;
    }

    //public async Task<IEnumerable<Claim>> GetUserClaimsAsync(EnjoyLifeUser user)
    //{
    //    List<Claim> claims = new List<Claim>();
    //    var defaultClaims = (await _signInManager.ClaimsFactory.CreateAsync(user)).Claims;
    //    claims.AddRange(defaultClaims);
    //    claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
    //    var userRoles = await _userManager.GetRolesAsync(user);
    //    foreach (var role in userRoles)
    //    {
    //        claims.Add(new Claim(ClaimTypes.Role, role));
    //    }

    //    return claims;
    //}


    public async Task<EnjoyLifeUser> CreateUserAsync(EnjoyLifeUser enjoyLifeUser, CancellationToken cancellationToken)
    {
        await _userManager.CreateAsync(enjoyLifeUser);

        return enjoyLifeUser;
    }

    public async Task<EnjoyLifeUser?> GetUserById(int Id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.Where(u => u.Id == Id).SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<EnjoyLifeUser> UpdateUserAsync(EnjoyLifeUser user, CancellationToken cancellationToken = default)
    {
        await _userManager.UpdateAsync(user);

        return user;
    }
    public async Task DeleteUserAsync(EnjoyLifeUser user)
    {
        await _userManager.DeleteAsync(user);
    }

    public async Task<OperationResult<object>> CreateUser(Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateUser");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
        {
            return Op.Failed("ارسال نام کاربر اجباری است");
        }
        if (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname))
        {
            return Op.Failed("ارسال نام خانوادگی کاربر اجباری است");
        }
        if ((!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber)) && (!Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09")))
        {
            return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
        }
        if (model.CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En(), cancellationToken: cancellationToken))
            {
                return Op.Failed("شماره موبایل ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => x.Email.ToLower() == model.Email.Fa2En().ToLower(), cancellationToken: cancellationToken))
            {
                return Op.Failed("آدرس ایمیل ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            if (await _context.Users.AnyAsync(x => x.CoreId == model.CoreId, cancellationToken: cancellationToken))
            {
                return Op.Failed("شناسه کاربر ارسال شده تکراری است", HttpStatusCode.Conflict);
            }
            long maxFakePhoneNumber = 0;
            if (await _context.Users.AnyAsync(x => !x.PhoneNumber.StartsWith("09"), cancellationToken: cancellationToken))
            {
                maxFakePhoneNumber = await _context.Users.Where(x => !x.PhoneNumber.StartsWith("09")).MaxAsync(x => Convert.ToInt64(x.PhoneNumber), cancellationToken: cancellationToken);
            }
            var userName = $"{model.Name} {model.Lastname}";
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower(), cancellationToken: cancellationToken))
            {
                userName = $"{userName}_{maxFakePhoneNumber + 1}";
            }
            await _context.Users.AddAsync(new EnjoyLifeUser
            {
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CoreId = model.CoreId,
                Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null,
                EmailConfirmed = true,
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim().ToUpper() : null,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                PasswordHash = !string.IsNullOrEmpty(model.Password) && !string.IsNullOrWhiteSpace(model.Password) ? _passwordHasher.Hash(model.Password) : null,
                PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? model.PhoneNumber.Fa2En() : (maxFakePhoneNumber + 1).ToString("00000000000"),
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت کاربر با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateUsers(List<Request_CreateUserDomainDTO> model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateUsers");
        if (model == null || !model.Any())
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (model.Any(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)))
        {
            return Op.Failed($"ارسال نام کاربر اجباری است ، خطلا در ردیف : {model.FindIndex(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)) + 1}");
        }
        if (model.Any(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)))
        {
            return Op.Failed($"ارسال نام خانوادگی کاربر اجباری است ، خطلا در ردیف : {model.FindIndex(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)) + 1}");
        }
        if (model.Any(x => (!string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber)) && (!Regex.IsMatch(x.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.Fa2En().StartsWith("09"))))
        {
            return Op.Failed($"شماره موبایل کاربر بدرستی وارد شده است ، خطلا در ردیف : {model.FindIndex(x => (!string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber)) && (!Regex.IsMatch(x.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.Fa2En().StartsWith("09"))) + 1}");
        }
        if (model.Any(x => x.CoreId <= 0))
        {
            return Op.Failed($"شناسه کاربر بدرستی ارسال نشده است ، خطلا در ردیف : {model.FindIndex(x => x.CoreId <= 0) + 1}");
        }
        if (model.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber)).Select(x => x.PhoneNumber).Distinct().ToList().Count != model.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber)).ToList().Count)
        {
            return Op.Failed("ثبت شماره تماس تکراری امکانپذیر نمیباشد");
        }
        if (model.Select(x => x.CoreId).Distinct().ToList().Count != model.Count)
        {
            return Op.Failed("ثبت شناسه تکراری کاربر امکانپذیر نمیباشد");
        }
        try
        {
            var dbUsers = await _context.Users.ToListAsync(cancellationToken: cancellationToken);
            if (model.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber)).Any(x => dbUsers.Any(y => y.PhoneNumber == x.PhoneNumber)))
            {
                return Op.Failed("ثبت شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (model.Any(x => dbUsers.Any(y => y.CoreId == x.CoreId)))
            {
                return Op.Failed("ثبت شناسه کاربری تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            long fakeMaxPhoneNumber = 0;
            if (dbUsers.Any(x => !x.PhoneNumber.StartsWith("09")))
            {
                fakeMaxPhoneNumber = dbUsers.Where(x => !x.PhoneNumber.StartsWith("09")).Max(x => Convert.ToInt64(x.PhoneNumber));
            }
            await _context.Users.AddRangeAsync(model.Select(x =>
            {
                if (string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber))
                {
                    fakeMaxPhoneNumber++;
                }
                return new EnjoyLifeUser
                {
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CoreId = x.CoreId,
                    Email = !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) ? x.Email.Fa2En().Trim() : null,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    LockoutEnd = null,
                    NormalizedEmail = !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) ? x.Email.Fa2En().Trim().ToUpper() : null,
                    NormalizedUserName = $"{x.Name} {x.Lastname}".ToUpper(),
                    UserName = $"{x.Name} {x.Lastname}",
                    PasswordHash = null,
                    PhoneNumber = !string.IsNullOrEmpty(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.PhoneNumber) ? x.PhoneNumber.Fa2En() : fakeMaxPhoneNumber.ToString("00000000000"),
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                };
            }), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("ثبت لیست کاربران با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ثبت لیست کاربران با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> SendOTPEmail(string emailOrPhoneNumber, string otpValue, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("SendOTPEmail");
        if (!emailOrPhoneNumber.IsNotNull())
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        if (!emailOrPhoneNumber.IsEmail(false) && !emailOrPhoneNumber.IsMobile())
        {
            return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == emailOrPhoneNumber.Fa2En().ToLower() || (x.PhoneNumber.ToLower() == emailOrPhoneNumber.Fa2En().ToLower())), cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شماره موبایل کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!user.Email.IsNotNull())
            {
                return Op.Failed("آدرس ایمیلی برای اطلاعات کاربر ارسال شده وجود ندارد", HttpStatusCode.NotFound);
            }
            if (!user.Email.IsEmail(false))
            {
                return Op.Failed("آدرس ایمیل برای اطلاعات کاربری ارسال شده نامعتبر است", HttpStatusCode.NotAcceptable);
            }
            MailMessage mail = new()
            {
                From = new MailAddress("mikapartners.2024@gmail.com", "Enjoy Life"),
                IsBodyHtml = true,
                Body = $@"<div
                  style=""
                    font-family: Verdana, Geneva, Tahoma, sans-serif;
                    min-width: 1000px;
                    overflow: auto;
                    line-height: 2;
                  ""
                  dir=""rtl""
                >
                  <div style=""margin: 50px auto; width: 70%; padding: 20px 0"">
                    <div
                      style=""
                        border-bottom: 1px solid #eee;
                        display: flex;
                        justify-content: space-between;
                        align-items: center;
                      ""
                    >
                      <a
                        href=""https://enjoylife.ir/""
                        target=""_blank""
                        style=""
                          font-size: 1.4em;
                          color: #00466a;
                          text-decoration: none;
                          font-weight: 600;
                        ""
                        >Enjoy Life</a
                      >
                    </div>
                    <p style=""font-size: 1.1em"">سلام ,</p>
                    <p>با تشکر از انتخاب شما.</p>
                    <p>
                      برای ورود به اپلیکیشن Enjoy Life از کد اعتبارسنجی زیر استفاده نمایید. توجه
                      داشته باشید اعتبار این کد 2 دقیقه میباشد.
                    </p>
                    <h2
                      style=""
                        background: #ffcc00;
                        margin: 0 auto;
                        width: max-content;
                        padding: 0 10px;
                        color: #00466a;
                        border-radius: 4px;
                      ""
                    >
                      {otpValue}
                    </h2>
                    <p style=""font-size: 0.9em"" dir=""ltr"">Enjoy Life</p>
                    <hr style=""border: none; border-top: 1px solid #eee"" />
                    <div
                      style=""
                        float: right;
                        padding: 8px 0;
                        color: #aaa;
                        font-size: 0.8em;
                        line-height: 1;
                        font-weight: 300;
                      ""
                    >
                      <p>گروه ساختمانی میکا</p>
                    </div>
                  </div>
                </div>
                ",
                Subject = "Enjoy Life - Verification OTP",
            };
            mail.To.Add(user.Email);
            SmtpClient SmtpServer = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("mikapartners.2024@gmail.com", "hjziyuhnsbhsjdcp"),
                //SmtpServer.UseDefaultCredentials = true;
                EnableSsl = true
            };
            SmtpServer.Send(mail);

            return Op.Succeed("ارسال کد اعتبارسنجی به ایمیل کاربری با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ارسال کد اعتبارسنجی با ایمیل با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> UpdateUser(Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("UpdateUser");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت بروزرسانی کاربر ارسال نشده است");
        }
        if (model.CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        if ((string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name)) && (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname)) && (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email)) && (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber)))
        {
            return Op.Failed("اطلاعاتی جهت بروزرسانی کاربر ارسال نشده است");
        }
        if ((!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber)) && (!Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09")))
        {
            return Op.Failed("شماره موبایل کاربر بدرستی ارسال شده است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.CoreId == model.CoreId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En() && x.CoreId != model.CoreId, cancellationToken: cancellationToken))
            {
                return Op.Failed("امکان ثبت کاربر با شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.CoreId != model.CoreId, cancellationToken: cancellationToken))
            {
                return Op.Failed("امکان ثبت کاربر با آدرس ایمیل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
            }
            string newName = string.Empty;
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
            {
                newName = model.Name.Trim();
            }
            if (!string.IsNullOrEmpty(model.Lastname) && !string.IsNullOrWhiteSpace(model.Lastname))
            {
                newName = !string.IsNullOrEmpty(newName) && !string.IsNullOrWhiteSpace(newName) ? $"{newName} {model.Lastname.Trim()}" : model.Lastname.Trim();
            }
            if (!string.IsNullOrEmpty(newName) && !string.IsNullOrWhiteSpace(newName))
            {
                if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == newName.ToLower() && x.CoreId != model.CoreId, cancellationToken: cancellationToken))
                {
                    long maxFakePhoneNumber = 0;
                    if (await _context.Users.AnyAsync(x => !x.PhoneNumber.StartsWith("09"), cancellationToken: cancellationToken))
                    {
                        maxFakePhoneNumber = await _context.Users.Where(x => !x.PhoneNumber.StartsWith("09")).MaxAsync(x => Convert.ToInt64(x.PhoneNumber), cancellationToken: cancellationToken);
                    }
                    user.UserName = $"{newName}_{maxFakePhoneNumber + 1}";
                    user.NormalizedUserName = $"{newName}_{maxFakePhoneNumber + 1}".ToUpper();
                }
                else
                {
                    user.UserName = newName;
                    user.NormalizedUserName = newName.ToUpper();
                }

            }
            if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber.Fa2En();
            }
            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email))
            {
                user.Email = model.Email.Fa2En();
            }
            if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = _passwordHasher.Hash(model.Password);
            }
            _context.Entry<EnjoyLifeUser>(user).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("بروزرسانی اطلاعات کاربر با موفقیت انجام شد");
        }
        catch (Exception ex)
        {

            return Op.Failed("بروزرسانی کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteUser(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteUser");
        if (model == null || model.CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.CoreId == model.CoreId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Succeed("حذف کاربر با موفقیت انجام شد");
            }
            _context.UserClaims.RemoveRange(await _context.UserClaims.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken: cancellationToken));
            _context.UserLogins.RemoveRange(await _context.UserLogins.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken: cancellationToken));
            _context.UserRoles.RemoveRange(await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken: cancellationToken));
            _context.UserTokens.RemoveRange(await _context.UserTokens.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken: cancellationToken));
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("حذف کاربر با موفقیت انجام شد");

        }
        catch (Exception ex)
        {
            return Op.Failed("حذف کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<EnjoyLifeUser>> LoginByPassword(string emailOrPhoneNumber, string password, CancellationToken cancellationToken = default)
    {
        OperationResult<EnjoyLifeUser> Op = new("LoginByPassword");
        if (!emailOrPhoneNumber.IsNotNull())
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        if (!emailOrPhoneNumber.IsEmail(false) && !emailOrPhoneNumber.IsMobile())
        {
            return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است");
        }
        try
        {

            var user = await _context.Users.FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == emailOrPhoneNumber.Fa2En().ToLower() || (x.PhoneNumber.ToLower() == emailOrPhoneNumber.Fa2En().ToLower())), cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("اطلاعات کاربری ارسال شده نامعتبر است", HttpStatusCode.NotFound);
            }
            if (!user.PasswordHash.IsNotNull())
            {
                return Op.Failed("رمز عبور برای کاربری با اطلاعات ارسال شده غیر فعال است", HttpStatusCode.NotFound);
            }
            if (!_passwordHasher.Check(user.PasswordHash, password).Verified)
            {
                return Op.Failed("اطلاعات کاربری ارسال شده نامعتبر است", HttpStatusCode.Forbidden);
            }
            return Op.Succeed("ورود با رمز عبور با موفقیت انجام شد", user);

        }
        catch (Exception ex)
        {
            return Op.Failed("ورود با رمز عبور با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<EnjoyLifeUser>> CheckUserForLoginByOTP(string emailOrPhoneNumber, CancellationToken cancellationToken = default)
    {
        OperationResult<EnjoyLifeUser> Op = new("CheckUserForLoginByOTP");
        if (!emailOrPhoneNumber.IsNotNull())
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        if (!emailOrPhoneNumber.IsEmail(false) && !emailOrPhoneNumber.IsMobile())
        {
            return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == emailOrPhoneNumber.Fa2En().ToLower() || (x.PhoneNumber.ToLower() == emailOrPhoneNumber.Fa2En().ToLower())), cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است", HttpStatusCode.NotFound);
            }
            return Op.Succeed("کاربر با اطلاعات وارد شده برای ورود با کد یکبار مصرف مجاز است", user);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<EnjoyLifeUser>> GetUserEmailOrPhoneNumber(string emailOrPhoneNumber, CancellationToken cancellationToken = default)
    {
        OperationResult<EnjoyLifeUser> Op = new("GetUserInfoByEmailOrPhoneNumber");
        if (!emailOrPhoneNumber.IsNotNull())
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        if (!emailOrPhoneNumber.IsEmail(false) && !emailOrPhoneNumber.IsMobile())
        {
            return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == emailOrPhoneNumber.Fa2En().ToLower() || (x.PhoneNumber.ToLower() == emailOrPhoneNumber.Fa2En().ToLower())), cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است", HttpStatusCode.NotFound);
            }
            return Op.Succeed("دریافت اطلاعات کاربری با موفقیت انجام شد", user);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<EnjoyLifeUser>> LoginLobbyMan(int CoreId, CancellationToken cancellationToken = default)
    {
        OperationResult<EnjoyLifeUser> Op = new("LoginLobbyMan");
        if (CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.CoreId == CoreId, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
            }
            return Op.Succeed("دریافت اطلاعات کاربری با موفقیت انجام شد", user);
        }
        catch (Exception ex)
        {
            return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> CreateOrUpdateIdentityUser(Request_CreateOrUpdateIdentityUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("CreateOrUpdateIdentityUser");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
        {
            return Op.Failed("ارسال نام کاربر اجباری است");
        }
        if (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname))
        {
            return Op.Failed("ارسال نام خانوادگی کاربر اجباری است");
        }
        if ((!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber)) && (!Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09")))
        {
            return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
        }
        if (model.CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.CoreId == model.CoreId, cancellationToken);
            long maxFakePhoneNumber = 0;
            if (await _context.Users.AnyAsync(x => !x.PhoneNumber.StartsWith("09"), cancellationToken: cancellationToken))
            {
                maxFakePhoneNumber = await _context.Users.Where(x => !x.PhoneNumber.StartsWith("09")).MaxAsync(x => Convert.ToInt64(x.PhoneNumber), cancellationToken: cancellationToken);
            }
            var userName = $"{model.Name} {model.Lastname}";
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower(), cancellationToken: cancellationToken))
            {
                userName = $"{userName}_{maxFakePhoneNumber + 1}";
            }
            if (user == null)
            {
                await _context.Users.AddAsync(new EnjoyLifeUser
                {
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CoreId = model.CoreId,
                    Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    LockoutEnd = null,
                    NormalizedEmail = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim().ToUpper() : null,
                    UserName = userName,
                    NormalizedUserName = userName.ToUpper(),
                    PasswordHash = !string.IsNullOrEmpty(model.Password) && !string.IsNullOrWhiteSpace(model.Password) ? _passwordHasher.Hash(model.Password) : null,
                    PhoneNumber = model.PhoneNumber.Fa2En(),
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                }, cancellationToken);
            }
            else
            {
                user.Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null;
                user.NormalizedEmail = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim().ToUpper() : null;
                user.UserName = userName;
                user.NormalizedUserName = userName.ToUpper();
                if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = _passwordHasher.Hash(model.Password);
                }
                user.PhoneNumber = model.PhoneNumber.Fa2En();
                _context.Entry<EnjoyLifeUser>(user).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("مدیریت کاربر با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("مدیریت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<object>> DeleteIdentityUser(Request_CoreId model, CancellationToken cancellationToken = default)
    {
        OperationResult<object> Op = new("DeleteIdentityUser");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
        }
        if (model.CoreId <= 0)
        {
            return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
        }
        try
        {
            var users = await _context.Users.Where(x => x.CoreId == model.CoreId).ToListAsync(cancellationToken: cancellationToken);
            if (users == null || !users.Any())
            {
                return Op.Succeed("عملیات با موفقیت انجام شد ، کاربری برای حذف وجود ندارد", HttpStatusCode.NoContent);
            }
            _context.UserClaims.RemoveRange((from c in await _context.UserClaims.ToListAsync(cancellationToken: cancellationToken)
                                             join u in users
                                             on c.UserId equals u.Id
                                             select c).ToList());
            _context.UserTokens.RemoveRange((from t in await _context.UserTokens.ToListAsync(cancellationToken: cancellationToken)
                                             join u in users
                                             on t.UserId equals u.Id
                                             select t).ToList());
            _context.UserRoles.RemoveRange((from r in await _context.UserRoles.ToListAsync(cancellationToken: cancellationToken)
                                            join u in users
                                            on r.UserId equals u.Id
                                            select r).ToList());
            _context.UserLogins.RemoveRange((from l in await _context.UserLogins.ToListAsync(cancellationToken: cancellationToken)
                                             join u in users
                                             on l.UserId equals u.Id
                                             select l).ToList());
            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync(cancellationToken);
            return Op.Succeed("حذف کاربر با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("حذف کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
        }

    }
}
