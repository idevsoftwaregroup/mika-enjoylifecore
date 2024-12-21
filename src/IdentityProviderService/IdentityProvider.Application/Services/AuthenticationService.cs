using IdentityProvider.Application.Contracts.Authentication.BackDoor;
using IdentityProvider.Application.Contracts.Authentication.Login;
using IdentityProvider.Application.Contracts.Authentication.OTPVerification;
using IdentityProvider.Application.Contracts.Authentication.Register;
using IdentityProvider.Application.Exceptions;
using IdentityProvider.Application.Framework;
using IdentityProvider.Application.Helper;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Domain.DomainModelDTOs;
using IdentityProvider.Domain.DomainModelDTOs.Users;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityProvider.Application.Services;

public class AuthenticationService : IAuthenticationService
{

    private readonly IEnjoyLifeIdentityRepository _enjoyLifeIdentityRepository;
    private readonly IOTPService _otpService;
    private readonly IMessagingService _messagingService;
    private readonly IJWTTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IEnjoyLifeIdentityRepository enjoyLifeIdentityRepository, IOTPService otpService, IMessagingService messagingService, IJWTTokenGenerator jwtTokenGenerator, ILogger<AuthenticationService> logger)
    {
        _enjoyLifeIdentityRepository = enjoyLifeIdentityRepository;
        _otpService = otpService;
        _messagingService = messagingService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestDTO, CancellationToken cancellationToken)
    {
        if (requestDTO.Username is null)
        {
            requestDTO.Username = requestDTO.PhoneNumber;
        }
        EnjoyLifeUser enjoyLifeUser = new EnjoyLifeUser()
        {
            PhoneNumber = requestDTO.PhoneNumber.Fa2En(),
            PhoneNumberConfirmed = true,
            UserName = requestDTO.Username,
            Email = requestDTO.Email,
            CoreId = requestDTO.CoreId,
        };

        await _enjoyLifeIdentityRepository.CreateUserAsync(enjoyLifeUser, cancellationToken);

        return new RegisterResponseDTO()
        {
            Id = enjoyLifeUser.Id,
            PhoneNumber = enjoyLifeUser.PhoneNumber,
            Username = enjoyLifeUser.UserName,
            Email = enjoyLifeUser.Email,
            CoreId = enjoyLifeUser.CoreId,
        };
    }
    public async Task<OperationResult<object>> LoginByOTP(LoginRequestDTO requestDTO, CancellationToken cancellationToken)
    {
        OperationResult<object> Op = new("LoginByOTP");
        if (requestDTO == null)
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        var checkOperation = await _enjoyLifeIdentityRepository.CheckUserForLoginByOTP(requestDTO.EmailOrPhoneNumber, cancellationToken);
        if (!checkOperation.Success)
        {
            return Op.Failed(checkOperation.Message, checkOperation.ExMessage, checkOperation.Status);
        }
        try
        {
            string otpvalue = _otpService.GenerateOTP(requestDTO.EmailOrPhoneNumber.Fa2En());
            if (!otpvalue.IsNotNull())
            {
                return Op.Failed("ساخت کد اعتبارسنجی برای ورود با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
            }
            var sendEmailOperation = await _enjoyLifeIdentityRepository.SendOTPEmail(requestDTO.EmailOrPhoneNumber.Fa2En(), otpvalue.Fa2En(), cancellationToken);
            if (!sendEmailOperation.Success && (checkOperation.Object == null || !checkOperation.Object.PhoneNumber.IsNotNull()))
            {
                return Op.Failed("ایمیل و شماره تماس برای ارسال کد اعتبارسنجی برای کاربر تعریف نشده است", System.Net.HttpStatusCode.NotFound);
            }
            if (checkOperation.Object != null && checkOperation.Object.PhoneNumber.IsNotNull())
            {
                await _messagingService.SendOTPMessage(checkOperation.Object.PhoneNumber, otpvalue.Fa2En(), cancellationToken);
            }
            return Op.Succeed("درخواست کد اعتبارسنجی برای ورود با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return Op.Failed("ورود با کد اعتبارسنجی با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<UserInfoResponseDTO>> VerifyOTP(OTPVerificationRequestDTO requestDTO, CancellationToken cancellationToken)
    {
        OperationResult<UserInfoResponseDTO> Op = new("VerifyOTP");
        if (requestDTO == null)
        {
            return Op.Failed("اطلاعاتی جهت ورود با کد اعتبارسنجی ارسال نشده است");
        }
        if (!requestDTO.EmailOrPhoneNumber.IsNotNull())
        {
            return Op.Failed("ارسال شماره موبایل یا ایمیل اجباری است");
        }
        if (!requestDTO.EmailOrPhoneNumber.IsEmail(false) && !requestDTO.EmailOrPhoneNumber.IsMobile())
        {
            return Op.Failed("شماره موبایل یا ایمیل ارسال شده نامعتبر است");
        }
        if (!requestDTO.OTPValue.IsNotNull())
        {
            return Op.Failed("ارسال کد اعتبارسنجی اجباری است");
        }
        if (!requestDTO.OTPValue.IsNumeric(false) || requestDTO.OTPValue.Length != 4)
        {
            return Op.Failed("کد اعتبارسنجی ارسال شده نامعتبر است");
        }

        try
        {
            #region TEMP BACKDOOR DATA

            var backDoorListData = new List<BackDoorDTO>()
            {
                new() {Phonenumber="09102300732",OTP="8763"},
                new() {Phonenumber="09120208775",OTP="3859"},
                new() {Phonenumber="09122664565",OTP="2670"},
                new() {Phonenumber="09127989261",OTP="6997"},
                new() {Phonenumber="09128909102",OTP="9025"},
                new() {Phonenumber="09129426985",OTP="3212"},
                new() {Phonenumber="09386865883",OTP="7927"},
            };

            if (backDoorListData.Any(x => x.Phonenumber == requestDTO.EmailOrPhoneNumber.Fa2En() && x.OTP == requestDTO.OTPValue.Fa2En()))
            {
                var getUserOperation = await _enjoyLifeIdentityRepository.GetUserEmailOrPhoneNumber(requestDTO.EmailOrPhoneNumber, cancellationToken);
                if (!getUserOperation.Success)
                {
                    return Op.Failed(getUserOperation.Message, getUserOperation.ExMessage, getUserOperation.Status);
                }
                if (getUserOperation.Object == null)
                {
                    return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", System.Net.HttpStatusCode.NotFound);
                }
                var token = await _jwtTokenGenerator.GenerateTokenAsync(getUserOperation.Object);
                if (!token.IsNotNull())
                {
                    return Op.Failed("ساخت توکن ورود با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
                }
                _logger.LogInformation($"token is {token}");
                return Op.Succeed("ورود با کد اعتبارسنجی با موفقیت انجام شد", new UserInfoResponseDTO
                {
                    Email = getUserOperation.Object.Email,
                    PhoneNumber = getUserOperation.Object.PhoneNumber,
                    Token = token,
                    UserId = getUserOperation.Object.CoreId
                });
            }

            #endregion

            else
            {
                var checkOTP = _otpService.ValidateOTP(requestDTO.EmailOrPhoneNumber.Fa2En(), requestDTO.OTPValue.Fa2En());
                if (!checkOTP)
                {
                    return Op.Failed("کد ارسال شده نامعتبر است");
                }
                var getUserOperation = await _enjoyLifeIdentityRepository.GetUserEmailOrPhoneNumber(requestDTO.EmailOrPhoneNumber, cancellationToken);
                if (!getUserOperation.Success)
                {
                    return Op.Failed(getUserOperation.Message, getUserOperation.ExMessage, getUserOperation.Status);
                }
                if (getUserOperation.Object == null)
                {
                    return Op.Failed("دریافت اطلاعات کاربری با مشکل مواجه شده است", System.Net.HttpStatusCode.NotFound);
                }
                var token = await _jwtTokenGenerator.GenerateTokenAsync(getUserOperation.Object);
                if (!token.IsNotNull())
                {
                    return Op.Failed("ساخت توکن ورود با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
                }
                _logger.LogInformation($"token is {token}");
                return Op.Succeed("ورود با کد اعتبارسنجی با موفقیت انجام شد", new UserInfoResponseDTO
                {
                    Email = getUserOperation.Object.Email,
                    PhoneNumber = getUserOperation.Object.PhoneNumber,
                    Token = token,
                    UserId = getUserOperation.Object.CoreId,
                    IsLobbyAttendant = false
                });
            }

        }
        catch (Exception ex)
        {
            return Op.Failed("ورود با کد اعتبارسنجی با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }

    }

    public async Task SeedAdmin()
    {
        await _enjoyLifeIdentityRepository.SeedAdmin();
    }

    public async Task<OperationResult<object>> CreateUser(Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.CreateUser(model, cancellationToken);
    }

    public async Task<OperationResult<object>> CreateUsers(List<Request_CreateUserDomainDTO> model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.CreateUsers(model, cancellationToken);
    }

    public async Task<OperationResult<object>> UpdateUser(Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.UpdateUser(model, cancellationToken);
    }

    public async Task<OperationResult<object>> DeleteUser(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.DeleteUser(model, cancellationToken);
    }

    public async Task<OperationResult<UserInfoResponseDTO>> LoginByPassword(Request_LoginByPasswrodDomainDTO model, CancellationToken cancellationToken = default)
    {
        var Op = new OperationResult<UserInfoResponseDTO>("LoginByPassword");
        if (model == null)
        {
            return Op.Failed("اطلاعاتی جهت ورود با رمز عبور ارسال نشده است");
        }
        var operation = await _enjoyLifeIdentityRepository.LoginByPassword(model.EmailOrPhonenumber, model.Password, cancellationToken);
        if (!operation.Success)
        {
            return Op.Failed(operation.Message, operation.ExMessage, operation.Status);
        }
        try
        {
            var token = await _jwtTokenGenerator.GenerateTokenAsync(operation.Object);
            if (!token.IsNotNull())
            {
                return Op.Failed("ساخت توکن ورود با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
            }
            _logger.LogInformation($"token is {token}");
            return Op.Succeed(operation.Message, new UserInfoResponseDTO()
            {
                Token = token,
                UserId = operation.Object.CoreId,
                Email = operation.Object.Email,
                PhoneNumber = operation.Object.PhoneNumber,
                IsLobbyAttendant = false
            });
        }
        catch (Exception ex)
        {

            return Op.Failed("ورود با رمز عبوربا مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<UserInfoResponseDTO>> LoginLobbyMan(int CoreId, CancellationToken cancellationToken = default)
    {
        OperationResult<UserInfoResponseDTO> Op = new("LoginLobbyMan");
        var loginOperation = await _enjoyLifeIdentityRepository.LoginLobbyMan(CoreId, cancellationToken);
        if (!loginOperation.Success)
        {
            return Op.Failed(loginOperation.Message, loginOperation.ExMessage, loginOperation.Status);
        }
        try
        {
            var token = await _jwtTokenGenerator.GenerateTokenAsync(loginOperation.Object);
            if (!token.IsNotNull())
            {
                return Op.Failed("ساخت توکن ورود با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
            }
            _logger.LogInformation($"token is {token}");
            return Op.Succeed(loginOperation.Message, new UserInfoResponseDTO()
            {
                Token = token,
                UserId = loginOperation.Object.CoreId,
                Email = loginOperation.Object.Email,
                PhoneNumber = loginOperation.Object.PhoneNumber,
                IsLobbyAttendant = true
            });
        }
        catch (Exception ex)
        {
            return Op.Failed("ورود لابی من با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }

    }

    public async Task<OperationResult<object>> CreateOrUpdateIdentityUser(Request_CreateOrUpdateIdentityUserDomainDTO model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.CreateOrUpdateIdentityUser(model, cancellationToken);
    }

    public async Task<OperationResult<object>> DeleteIdentityUser(Request_CoreId model, CancellationToken cancellationToken = default)
    {
        return await _enjoyLifeIdentityRepository.DeleteIdentityUser(model, cancellationToken);
    }
}
