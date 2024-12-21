using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.application.Helper;
using core.domain.DomainModelDTOs.UserDTOs;
using core.domain.entity.enums;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;
using core.domain.entity.WebSocketModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace core.infrastructure.Data.repository
{
    public class UserRepository : IUserRepository
    {
        private readonly EnjoyLifeContext _context;

        public UserRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            return await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<UserModel>> GetAllUserAsync()
        {
            return await _context.Users.Where(x => x.IsDelete != true).ToListAsync();
        }
        public async Task<UserModel> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetUsersPhoneNumberAsync(int ComplexId)
        {
            List<string> phoneNumbersR = _context.Residents
                                        .Where(r => r.Unit.ComplexId == ComplexId)
                                        .Select(r => r.User.PhoneNumber)
                                        .ToList();
            List<string> phoneNumbersO = _context.Owners
                            .Where(r => r.Unit.ComplexId == ComplexId)
                            .Select(r => r.User.PhoneNumber)
                            .ToList();
            return phoneNumbersR.Union(phoneNumbersO).Distinct().ToList();
        }


        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<int> AddUserAsync(UserModel user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateUserAsync(UserModel user)
        {
            if (await _context.Users.AnyAsync(x => x.Id == user.Id))
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync();
            }
            else return -1;
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    return await _context.SaveChangesAsync();
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

        public async Task<OperationResult<object>> SetUserConnection(int userId, string connection)
        {
            OperationResult<object> Op = new("SetUserConnection");
            if (userId <= 0 || string.IsNullOrEmpty(connection) || string.IsNullOrWhiteSpace(connection))
            {
                return Op.Failed("شناسه کاربر یا شناسه اتصال بدرستی ارسال نشده است");
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return Op.Failed("کاربری با شناسه ارسال شده وجود ندارد");
                }
                var connectionObject = await _context.UserConnections.FirstOrDefaultAsync(x => x.UserId == userId);
                if (connectionObject == null)
                {
                    await _context.UserConnections.AddAsync(new domain.entity.WebSocketModels.UserConnectionModel
                    {
                        Connection = connection,
                        ConnectionDate = Op.OperationDate,
                        UserId = userId,
                    });
                }
                else
                {
                    connectionObject.Connection = connection;
                    connectionObject.ConnectionDate = Op.OperationDate;
                    _context.Entry<UserConnectionModel>(connectionObject).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                return Op.Succeed("ثبت اتصال کاربر با موفقیت انجام شد", new
                {
                    Connection = connection,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت اتصال کاربر با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> RemoveUserConnection(int userId)
        {
            OperationResult<object> Op = new("RemoveUserConnection");
            if (userId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return Op.Failed("کاربری با شناسه ارسال شده وجود ندارد");
                }
                var connectionObject = await _context.UserConnections.FirstOrDefaultAsync(x => x.UserId == userId);
                if (connectionObject != null)
                {
                    connectionObject.Connection = null;
                    connectionObject.ConnectionDate = null;
                    _context.Entry<UserConnectionModel>(connectionObject).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return Op.Succeed("حذف اتصال کاربر با موفقیت انجام شد");
            }
            catch (Exception ex)
            {

                return Op.Failed("حذف اتصال کاربر با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> RemoveUserConnection(string connection)
        {
            OperationResult<object> Op = new("RemoveUserConnection");
            if (string.IsNullOrEmpty(connection) || string.IsNullOrWhiteSpace(connection))
            {
                return Op.Failed("شناسه اتصال بدرستی ارسال نشده است");
            }
            try
            {

                var connectionObject = await _context.UserConnections.FirstOrDefaultAsync(x => x.Connection == connection);
                if (connectionObject != null)
                {
                    connectionObject.Connection = null;
                    connectionObject.ConnectionDate = null;
                    _context.Entry<UserConnectionModel>(connectionObject).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return Op.Succeed("حذف اتصال کاربر با موفقیت انجام شد");
            }
            catch (Exception ex)
            {

                return Op.Failed("حذف اتصال کاربر با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<List<string>> GetAllConnections()
        {
            try
            {
                var connections = (from connection in await _context.UserConnections.ToListAsync()
                                   select connection.Connection).Distinct().ToList();
                return connections == null || !connections.Any() ? new List<string>() : connections;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetOtherConnections(int userId)
        {
            try
            {
                var connections = (from connection in await _context.UserConnections.ToListAsync()
                                   where connection.UserId != userId
                                   select connection.Connection).Distinct().ToList();
                return connections == null || !connections.Any() ? new List<string>() : connections;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<OperationResult<Response_CreatedUserDomainDTO>> CreateUserAndResident(int adminId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreatedUserDomainDTO> Op = new("CreateUserAndResident");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
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
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }
            if (model.Unit == null)
            {
                return Op.Failed("ثبت اطلاعات واحد اجباری است");
            }
            if (model.Unit.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (model.Unit.FromDate != null && model.Unit.ToDate != null && model.Unit.FromDate > model.Unit.ToDate)
            {
                return Op.Failed("تاریخ شروع و پایان سکونت کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken);
                if (unit == null)
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با آدرس ایمیل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                await _context.Users.AddAsync(new UserModel
                {
                    FirstName = model.Name.Trim(),
                    LastName = model.Lastname.Trim(),
                    Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null,
                    PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? model.PhoneNumber.Fa2En() : string.Empty,
                    PhoneNumberConfirmed = true,
                    NationalID = !string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) ? model.NationalID.Fa2En().Trim() : null,
                    Address = !string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address) ? model.Address.Trim() : null,
                    Age = model.Age,
                    Gender = model.Gender,
                    IsDelete = false,
                    CreatedDate = Op.OperationDate,
                    CreatedBy = adminId.ToString(),
                    ModifyDate = Op.OperationDate,
                    ModifyBy = adminId.ToString(),
                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var addedUser = await _context.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true : x.CreatedDate == Op.OperationDate && x.ModifyDate == Op.OperationDate, cancellationToken: cancellationToken);
                if (addedUser == null)
                {
                    return Op.Failed("ثبت ساکن جدید با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }

                if (model.IsHiddenPhoneNumber != null && model.IsHiddenPhoneNumber == true)
                {
                    await _context.UserLookUp.AddAsync(new UserLookUpModel
                    {
                        BooleanValue = true,
                        Key = "IsHiddenPhoneNumber",
                        UserId = addedUser.Id
                    }, cancellationToken);
                }

                await _context.Residents.AddAsync(new domain.entity.partyModels.ResidentModel
                {
                    FromDate = model.Unit.FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.FromDate),
                    ToDate = model.Unit.ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.ToDate),
                    IsHead = model.Unit.IsHead,
                    Unit = unit,
                    User = addedUser,
                    Renting = model.Unit.Renting,
                    IsDelete = false,
                    CreatedDate = Op.OperationDate,
                    CreatedBy = adminId.ToString(),
                    ModifyDate = Op.OperationDate,
                    ModifyBy = adminId.ToString(),

                }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت ساکن جدید با موفقیت انجام شد", new Response_CreatedUserDomainDTO
                {
                    UserId = addedUser.Id,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Fa2En(),
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت ساکن جدید با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreatedUserDomainDTO>> CreateUserAndOwner(int adminId, Request_CreateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreatedUserDomainDTO> Op = new("CreateUserAndOwner");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
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
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID, "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }
            if (model.Unit == null)
            {
                return Op.Failed("ثبت اطلاعات واحد اجباری است");
            }
            if (model.Unit.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (model.Unit.FromDate != null && model.Unit.ToDate != null && model.Unit.FromDate > model.Unit.ToDate)
            {
                return Op.Failed("تاریخ شروع و پایان سکونت کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken);
                if (unit == null)
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با آدرس ایمیل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                await _context.Users.AddAsync(new UserModel
                {
                    FirstName = model.Name.Trim(),
                    LastName = model.Lastname.Trim(),
                    Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null,
                    PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? model.PhoneNumber.Fa2En() : string.Empty,
                    PhoneNumberConfirmed = true,
                    NationalID = !string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) ? model.NationalID.Fa2En().Trim() : null,
                    Address = !string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address) ? model.Address.Trim() : null,
                    Age = model.Age,
                    Gender = model.Gender,
                    IsDelete = false,
                    CreatedDate = Op.OperationDate,
                    CreatedBy = adminId.ToString(),
                    ModifyDate = Op.OperationDate,
                    ModifyBy = adminId.ToString(),
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var addedUser = await _context.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true : x.CreatedDate == Op.OperationDate && x.ModifyDate == Op.OperationDate, cancellationToken: cancellationToken);
                if (addedUser == null)
                {
                    return Op.Failed("ثبت مالک جدید با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                if (model.IsHiddenPhoneNumber != null && model.IsHiddenPhoneNumber == true)
                {
                    await _context.UserLookUp.AddAsync(new UserLookUpModel
                    {
                        BooleanValue = true,
                        Key = "IsHiddenPhoneNumber",
                        UserId = addedUser.Id
                    }, cancellationToken);
                }
                var unitOwner = await _context.Owners.Include(x => x.Unit).FirstOrDefaultAsync(x => x.Unit.Id == unit.Id, cancellationToken: cancellationToken);
                if (unitOwner == null)
                {
                    await _context.Owners.AddAsync(new OwnerModel
                    {
                        FromDate = model.Unit.FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.FromDate),
                        ToDate = model.Unit.ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.ToDate),
                        Unit = unit,
                        User = addedUser,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                        Percentage = 100,
                    }, cancellationToken);

                }
                else
                {
                    unitOwner.User = addedUser;
                    unitOwner.ModifyDate = Op.OperationDate;
                    unitOwner.ModifyBy = adminId.ToString();
                    _context.Entry<OwnerModel>(unitOwner).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت مالک جدید با موفقیت انجام شد", new Response_CreatedUserDomainDTO()
                {
                    UserId = addedUser.Id,
                    PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) ? model.PhoneNumber.Fa2En() : null,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت مالک جدید با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreatedUserDomainDTO>> UpdateUserAndResident(int adminId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreatedUserDomainDTO> Op = new("UpdateUserAndResident");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (
                (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name)) &&
                (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname)) &&
                (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber)) &&
                (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email)) &&
                (string.IsNullOrEmpty(model.NationalID) || string.IsNullOrWhiteSpace(model.NationalID)) &&
                (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address)) &&
                (model.Age == null) &&
                (model.Gender == null) &&
                (
                   model.Unit == null ||
                   (model.Unit.IsHead == null && model.Unit.Renting == null && model.Unit.FromDate == null && model.Unit.ToDate == null)
                ) &&
                (model.IsHiddenPhoneNumber == null)
              )
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            if ((!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber)) && (!Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09")))
            {
                return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
            }
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }
            if (model.Unit != null && model.Unit.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (model.Unit != null && model.Unit.FromDate != null && model.Unit.ToDate != null && model.Unit.FromDate > model.Unit.ToDate)
            {
                return Op.Failed("تاریخ شروع و پایان سکونت کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber.ToLower() == model.PhoneNumber.Fa2En().ToLower() && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان بروزرسانی کاربر با شماره موبایل تکراری امکانپذیر نیست", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان بروزرسانی کاربر با آدرس ایمیل تکراری امکان پذیر نیست", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.Unit != null && !await _context.Units.AnyAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("اطلاعات واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
                {
                    user.FirstName = model.Name.Trim();
                }
                if (!string.IsNullOrEmpty(model.Lastname) && !string.IsNullOrWhiteSpace(model.Lastname))
                {
                    user.LastName = model.Lastname.Trim();
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email))
                {
                    user.Email = model.Email.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID))
                {
                    user.NationalID = model.NationalID.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address))
                {
                    user.Address = model.Address.Trim();
                }
                if (model.Age != null)
                {
                    user.Age = model.Age;
                }
                if (model.Gender != null)
                {
                    user.Gender = model.Gender;
                }
                user.ModifyBy = adminId.ToString();
                user.ModifyDate = Op.OperationDate;
                _context.Entry(user).State = EntityState.Modified;

                await _context.SaveChangesAsync(cancellationToken);

                if (model.IsHiddenPhoneNumber != null)
                {
                    var lookUp = await _context.UserLookUp.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.Key.ToLower() == "IsHiddenPhoneNumber".ToLower(), cancellationToken: cancellationToken);
                    if (lookUp == null)
                    {
                        await _context.UserLookUp.AddAsync(new UserLookUpModel
                        {
                            BooleanValue = model.IsHiddenPhoneNumber,
                            Key = "IsHiddenPhoneNumber",
                            UserId = model.UserId
                        }, cancellationToken);
                    }
                    else
                    {
                        lookUp.BooleanValue = model.IsHiddenPhoneNumber;
                        _context.Entry<UserLookUpModel>(lookUp).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (model.Unit != null)
                {
                    _context.RemoveRange(await _context.Residents.Include(x => x.User).Where(x => x.User.Id == user.Id).ToListAsync(cancellationToken: cancellationToken));

                    await _context.SaveChangesAsync(cancellationToken);

                    var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken);

                    await _context.Residents.AddAsync(new ResidentModel
                    {
                        FromDate = model.Unit.FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.FromDate),
                        ToDate = model.Unit.ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.ToDate),
                        IsHead = model.Unit.IsHead != null && Convert.ToBoolean(model.Unit.IsHead),
                        Unit = unit,
                        User = user,
                        Renting = model.Unit.Renting != null && Convert.ToBoolean(model.Unit.Renting),
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                    }, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Op.Succeed("بروزرسانی اطلاعات ساکن با موفقیت انجام شد", new Response_CreatedUserDomainDTO
                {
                    UserId = user.Id,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Fa2En(),
                });

            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات ساکن با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreatedUserDomainDTO>> UpdateUserAndOwner(int adminId, Request_UpdateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreatedUserDomainDTO> Op = new("UpdateUserAndOwner");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (
               (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name)) &&
               (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname)) &&
               (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber)) &&
               (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email)) &&
               (string.IsNullOrEmpty(model.NationalID) || string.IsNullOrWhiteSpace(model.NationalID)) &&
               (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address)) &&
               (model.Age == null) &&
               (model.Gender == null) &&
               (
                  model.Unit == null ||
                  (model.Unit.FromDate == null && model.Unit.ToDate == null)
               ) &&
               (model.IsHiddenPhoneNumber == null)
             )
            {
                return Op.Failed("اطلاعاتی برای بروزرسانی ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            if ((!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber)) && (!Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09")))
            {
                return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
            }
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }
            if (model.Unit != null && model.Unit.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if (model.Unit != null && model.Unit.FromDate != null && model.Unit.ToDate != null && model.Unit.FromDate > model.Unit.ToDate)
            {
                return Op.Failed("تاریخ شروع و پایان سکونت کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber) && await _context.Users.AnyAsync(x => x.PhoneNumber.ToLower() == model.PhoneNumber.Fa2En().ToLower() && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان بروزرسانی کاربر با شماره موبایل تکراری امکانپذیر نیست", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان بروزرسانی کاربر با آدرس ایمیل تکراری امکان پذیر نیست", HttpStatusCode.Conflict);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (model.Unit != null && !await _context.Units.AnyAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("اطلاعات واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name))
                {
                    user.FirstName = model.Name.Trim();
                }
                if (!string.IsNullOrEmpty(model.Lastname) && !string.IsNullOrWhiteSpace(model.Lastname))
                {
                    user.LastName = model.Lastname.Trim();
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email))
                {
                    user.Email = model.Email.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID))
                {
                    user.NationalID = model.NationalID.Fa2En().Trim();
                }
                if (!string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address))
                {
                    user.Address = model.Address.Trim();
                }
                if (model.Age != null)
                {
                    user.Age = model.Age;
                }
                if (model.Gender != null)
                {
                    user.Gender = model.Gender;
                }
                user.ModifyBy = adminId.ToString();
                user.ModifyDate = Op.OperationDate;
                _context.Entry(user).State = EntityState.Modified;

                await _context.SaveChangesAsync(cancellationToken);

                if (model.IsHiddenPhoneNumber != null)
                {
                    var lookUp = await _context.UserLookUp.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.Key.ToLower() == "IsHiddenPhoneNumber".ToLower(), cancellationToken: cancellationToken);
                    if (lookUp == null)
                    {
                        await _context.UserLookUp.AddAsync(new UserLookUpModel
                        {
                            BooleanValue = model.IsHiddenPhoneNumber,
                            Key = "IsHiddenPhoneNumber",
                            UserId = model.UserId
                        }, cancellationToken);
                    }
                    else
                    {
                        lookUp.BooleanValue = model.IsHiddenPhoneNumber;
                        _context.Entry<UserLookUpModel>(lookUp).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (model.Unit != null)
                {
                    _context.Owners.RemoveRange(await _context.Owners.Include(x => x.Unit).Where(x => x.Unit.Id == model.Unit.UnitId).ToListAsync(cancellationToken: cancellationToken));

                    await _context.SaveChangesAsync(cancellationToken);

                    var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.Unit.UnitId, cancellationToken: cancellationToken);
                    await _context.Owners.AddAsync(new OwnerModel
                    {
                        FromDate = model.Unit.FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.FromDate),
                        ToDate = model.Unit.ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Unit.ToDate),
                        Unit = unit,
                        User = user,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                        Percentage = 100,
                    }, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Op.Succeed("بروزرسانی اطلاعات مالک با موفقیت انجام شد", new Response_CreatedUserDomainDTO
                {
                    UserId = user.Id,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Fa2En(),
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی اطلاعات مالک با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<object>> DeleteUserAndOwnerResident(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteUserAndOwnerResident");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای حذف کاربر ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                user.IsDelete = true;
                user.ModifyBy = adminId.ToString();
                user.ModifyDate = Op.OperationDate;
                _context.Entry<UserModel>(user).State = EntityState.Modified;

                _context.Owners.RemoveRange(await _context.Owners.Include(x => x.User).Where(x => x.User.Id == model.UserId).ToListAsync(cancellationToken: cancellationToken));
                _context.Residents.RemoveRange(await _context.Residents.Include(x => x.User).Where(x => x.User.Id == model.UserId).ToListAsync(cancellationToken: cancellationToken));
                await _context.SaveChangesAsync(cancellationToken);

                return Op.Succeed("حذف کاربر با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("حذف کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }
        public async Task<OperationResult<object>> DeleteOwnership(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteOwnership");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای حذف کاربر ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                _context.Owners.RemoveRange(await _context.Owners.Include(x => x.User).Where(x => x.User.Id == model.UserId).ToListAsync(cancellationToken: cancellationToken));
                await _context.SaveChangesAsync(cancellationToken);

                return Op.Succeed("حذف مالکیت کاربر با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("حذف مالکیت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteResidence(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteResidence");
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای حذف کاربر ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                _context.Residents.RemoveRange(await _context.Residents.Include(x => x.User).Where(x => x.User.Id == model.UserId).ToListAsync(cancellationToken: cancellationToken));
                await _context.SaveChangesAsync(cancellationToken);

                return Op.Succeed("حذف سکونت کاربر با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("حذف سکونت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreatedResidentOwnerOfUnitDmainDTO>> CreateResidentsOwnersOfUnit(int adminId, Request_CreateResidentsOwnersOfUnitDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreatedResidentOwnerOfUnitDmainDTO> Op = new("CreateResidentsOwnersOfUnit");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            if ((model.Owners == null || !model.Owners.Any()) && (model.Residents == null || !model.Residents.Any()))
            {
                return Op.Failed("ارسال حداقل اطلاعات مالکین یا ساکنین اجباری است");
            }

            var safeGenderList = new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }.ToList();


            if (model.Owners != null && model.Owners.Any())
            {
                if (model.Owners.Any(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)))
                {
                    return Op.Failed($"ارسال نام کاربر مالک اجباری است ، خطلا در ردیف : {model.Owners.FindIndex(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)) + 1}");
                }
                if (model.Owners.Any(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)))
                {
                    return Op.Failed($"ارسال نام خانوادگی مالک کاربر اجباری است ، خطلا در ردیف : {model.Owners.FindIndex(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)) + 1}");
                }
                if (model.Owners.Any(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber)))
                {
                    return Op.Failed($"ارسال شماره موبایل کاربر مالک اجباری است ، خطلا در ردیف : {model.Owners.FindIndex(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber)) + 1}");
                }
                if (model.Owners.Any(x => !Regex.IsMatch(x.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.StartsWith("09")))
                {
                    return Op.Failed($"شماره موبایل کاربر مالک بدرستی وارد شده است ، خطلا در ردیف : {model.Owners.FindIndex(x => !Regex.IsMatch(x.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.StartsWith("09")) + 1}");
                }
                if (model.Owners.Any(x => !string.IsNullOrEmpty(x.NationalID) && !string.IsNullOrWhiteSpace(x.NationalID) && !Regex.IsMatch(x.NationalID, "^[0-9]*$", RegexOptions.IgnoreCase)))
                {
                    return Op.Failed($"کد ملی کاربر مالک بدرستی وارد شده است ، خطلا در ردیف : {model.Owners.FindIndex(x => !string.IsNullOrEmpty(x.NationalID) && !string.IsNullOrWhiteSpace(x.NationalID) && !Regex.IsMatch(x.NationalID, "^[0-9]*$", RegexOptions.IgnoreCase)) + 1}");
                }
                if (model.Owners.Any(x => x.Age != null && x.Age <= 0))
                {
                    return Op.Failed($"سن کاربر مالک بدرستی وارد شده است ، خطلا در ردیف : {model.Owners.FindIndex(x => x.Age != null && x.Age <= 0) + 1}");
                }
                if (model.Owners.Any(x => x.Gender != null && !safeGenderList.Any(y => y == (int)x.Gender)))
                {
                    return Op.Failed($"جنسیت کاربر مالک بدرستی وارد شده است ، خطلا در ردیف : {model.Owners.FindIndex(x => x.Gender != null && !safeGenderList.Any(y => y == (int)x.Gender)) + 1}");
                }
                if (model.Owners.Any(x => x.FromDate != null && x.ToDate != null && x.FromDate > x.ToDate))
                {
                    return Op.Failed($"تاریخ شروع و پایان مالکیت بدرستی وارد شده است ، خطلا در ردیف : {model.Owners.FindIndex(x => x.FromDate != null && x.ToDate != null && x.FromDate > x.ToDate) + 1}");
                }
                if (model.Owners.Select(x => x.PhoneNumber).Distinct().ToList().Count != model.Owners.Count)
                {
                    return Op.Failed("ارسال شماره موبایل تکراری برای مالکین امکانپذیر نمیباشد");
                }
            }
            if (model.Residents != null && model.Residents.Any())
            {
                if (model.Residents.Any(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)))
                {
                    return Op.Failed($"ارسال نام کاربر ساکن اجباری است ، خطلا در ردیف : {model.Residents.FindIndex(x => string.IsNullOrEmpty(x.Name) || string.IsNullOrWhiteSpace(x.Name)) + 1}");
                }
                if (model.Residents.Any(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)))
                {
                    return Op.Failed($"ارسال نام خانوادگی ساکن کاربر اجباری است ، خطلا در ردیف : {model.Residents.FindIndex(x => string.IsNullOrEmpty(x.Lastname) || string.IsNullOrWhiteSpace(x.Lastname)) + 1}");
                }
                if (model.Residents.Any(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber)))
                {
                    return Op.Failed($"ارسال شماره موبایل کاربر ساکن اجباری است ، خطلا در ردیف : {model.Residents.FindIndex(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber)) + 1}");
                }
                if (model.Residents.Any(x => !Regex.IsMatch(x.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.StartsWith("09")))
                {
                    return Op.Failed($"شماره موبایل کاربر ساکن بدرستی وارد شده است ، خطلا در ردیف : {model.Residents.FindIndex(x => !Regex.IsMatch(x.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.StartsWith("09")) + 1}");
                }
                if (model.Residents.Any(x => !string.IsNullOrEmpty(x.NationalID) && !string.IsNullOrWhiteSpace(x.NationalID) && !Regex.IsMatch(x.NationalID, "^[0-9]*$", RegexOptions.IgnoreCase)))
                {
                    return Op.Failed($"کد ملی کاربر ساکن بدرستی وارد شده است ، خطلا در ردیف : {model.Residents.FindIndex(x => !string.IsNullOrEmpty(x.NationalID) && !string.IsNullOrWhiteSpace(x.NationalID) && !Regex.IsMatch(x.NationalID, "^[0-9]*$", RegexOptions.IgnoreCase)) + 1}");
                }
                if (model.Residents.Any(x => x.Age != null && x.Age <= 0))
                {
                    return Op.Failed($"سن کاربر ساکن بدرستی وارد شده است ، خطلا در ردیف : {model.Residents.FindIndex(x => x.Age != null && x.Age <= 0) + 1}");
                }
                if (model.Residents.Any(x => x.Gender != null && !safeGenderList.Any(y => y == (int)x.Gender)))
                {
                    return Op.Failed($"جنسیت کاربر ساکن بدرستی وارد شده است ، خطلا در ردیف : {model.Residents.FindIndex(x => x.Gender != null && !safeGenderList.Any(y => y == (int)x.Gender)) + 1}");
                }
                if (model.Residents.Any(x => x.FromDate != null && x.ToDate != null && x.FromDate > x.ToDate))
                {
                    return Op.Failed($"تاریخ شروع و پایان سکونت بدرستی وارد شده است ، خطلا در ردیف : {model.Residents.FindIndex(x => x.FromDate != null && x.ToDate != null && x.FromDate > x.ToDate) + 1}");
                }
                if (model.Residents.Select(x => x.PhoneNumber).Distinct().ToList().Count != model.Residents.Count)
                {
                    return Op.Failed("ارسال شماره موبایل تکراری برای ساکنین امکانپذیر نمیباشد");
                }
            }

            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id == model.UnitId, cancellationToken: cancellationToken);
                if (unit == null)
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }

                List<Response_CreatedResidentOwnerOfUnitDmainDTO> result = new();

                if (model.Owners != null && model.Owners.Any())
                {
                    for (int i = 0; i < model.Owners.Count; i++)
                    {

                        var owner = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Owners[i].PhoneNumber, cancellationToken: cancellationToken);

                        if (owner == null)
                        {
                            await _context.Users.AddAsync(new UserModel
                            {
                                FirstName = model.Owners[i].Name.Trim(),
                                LastName = model.Owners[i].Lastname.Trim(),
                                Email = !string.IsNullOrEmpty(model.Owners[i].Email) && !string.IsNullOrWhiteSpace(model.Owners[i].Email) ? model.Owners[i].Email.Trim() : null,
                                PhoneNumber = model.Owners[i].PhoneNumber,
                                PhoneNumberConfirmed = true,
                                NationalID = !string.IsNullOrEmpty(model.Owners[i].NationalID) && !string.IsNullOrWhiteSpace(model.Owners[i].NationalID) ? model.Owners[i].NationalID.Trim() : null,
                                Address = !string.IsNullOrEmpty(model.Owners[i].Address) && !string.IsNullOrWhiteSpace(model.Owners[i].Address) ? model.Owners[i].Address.Trim() : null,
                                Age = model.Owners[i].Age,
                                Gender = model.Owners[i].Gender,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);

                            var addedOwner = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Owners[i].PhoneNumber, cancellationToken: cancellationToken);
                            if (addedOwner == null)
                            {
                                return Op.Failed("ثبت اطلاعات مالک با مشکل مواجه شده است", HttpStatusCode.InternalServerError);
                            }

                            await _context.Owners.AddAsync(new domain.entity.partyModels.OwnerModel
                            {
                                FromDate = model.Owners[i].FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Owners[i].FromDate),
                                ToDate = model.Owners[i].ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Owners[i].ToDate),
                                Unit = unit,
                                User = addedOwner,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                                Percentage = 100,
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);

                            result.Add(new Response_CreatedResidentOwnerOfUnitDmainDTO
                            {
                                CoreId = addedOwner.Id,
                                Email = addedOwner.Email,
                                Lastname = addedOwner.LastName,
                                Name = addedOwner.FirstName,
                                PhoneNumber = addedOwner.PhoneNumber,
                            });
                        }
                        else
                        {
                            if (await _context.Owners.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.User == owner && x.Unit == unit, cancellationToken: cancellationToken))
                            {
                                return Op.Failed("ثبت مالک تکراری امکان پذیر نمیباشد");
                            }
                            await _context.Owners.AddAsync(new domain.entity.partyModels.OwnerModel
                            {
                                FromDate = model.Owners[i].FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Owners[i].FromDate),
                                ToDate = model.Owners[i].ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Owners[i].ToDate),
                                Unit = unit,
                                User = owner,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                                Percentage = 100,
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
                if (model.Residents != null && model.Residents.Any())
                {
                    for (int i = 0; i < model.Residents.Count; i++)
                    {

                        var resident = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Residents[i].PhoneNumber, cancellationToken: cancellationToken);

                        if (resident == null)
                        {
                            await _context.Users.AddAsync(new UserModel
                            {
                                FirstName = model.Residents[i].Name.Trim(),
                                LastName = model.Residents[i].Lastname.Trim(),
                                Email = !string.IsNullOrEmpty(model.Residents[i].Email) && !string.IsNullOrWhiteSpace(model.Residents[i].Email) ? model.Residents[i].Email.Trim() : null,
                                PhoneNumber = model.Residents[i].PhoneNumber,
                                PhoneNumberConfirmed = true,
                                NationalID = !string.IsNullOrEmpty(model.Residents[i].NationalID) && !string.IsNullOrWhiteSpace(model.Residents[i].NationalID) ? model.Residents[i].NationalID.Trim() : null,
                                Address = !string.IsNullOrEmpty(model.Residents[i].Address) && !string.IsNullOrWhiteSpace(model.Residents[i].Address) ? model.Residents[i].Address.Trim() : null,
                                Age = model.Residents[i].Age,
                                Gender = model.Residents[i].Gender,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);

                            var addedResident = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Residents[i].PhoneNumber, cancellationToken: cancellationToken);
                            if (addedResident == null)
                            {
                                return Op.Failed("ثبت اطلاعات ساکن با مشکل مواجه شده است", HttpStatusCode.InternalServerError);
                            }

                            await _context.Residents.AddAsync(new domain.entity.partyModels.ResidentModel
                            {
                                FromDate = model.Residents[i].FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Residents[i].FromDate),
                                ToDate = model.Residents[i].ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Residents[i].ToDate),
                                IsHead = model.Residents[i].IsHead,
                                Unit = unit,
                                User = addedResident,
                                Renting = model.Residents[i].Renting,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);

                            result.Add(new Response_CreatedResidentOwnerOfUnitDmainDTO
                            {
                                CoreId = addedResident.Id,
                                Email = addedResident.Email,
                                Lastname = addedResident.LastName,
                                Name = addedResident.FirstName,
                                PhoneNumber = addedResident.PhoneNumber,
                            });
                        }
                        else
                        {
                            if (await _context.Residents.Include(x => x.User).Include(x => x.Unit).AnyAsync(x => x.User == resident && x.Unit == unit, cancellationToken: cancellationToken))
                            {
                                return Op.Failed("ثبت ساکن تکراری امکان پذیر نمیباشد");
                            }
                            await _context.Residents.AddAsync(new domain.entity.partyModels.ResidentModel
                            {
                                FromDate = model.Residents[i].FromDate == null ? Op.OperationDate : Convert.ToDateTime(model.Residents[i].FromDate),
                                ToDate = model.Residents[i].ToDate == null ? Op.OperationDate : Convert.ToDateTime(model.Residents[i].ToDate),
                                IsHead = model.Residents[i].IsHead,
                                Unit = unit,
                                User = resident,
                                Renting = model.Residents[i].Renting,
                                IsDelete = false,
                                CreatedDate = Op.OperationDate,
                                CreatedBy = adminId.ToString(),
                                ModifyDate = Op.OperationDate,
                                ModifyBy = adminId.ToString(),
                            }, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                return Op.Succeed("ثبت لیست ساکنین و مالکین با موفقیت انجام شد", result);
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت لیست ساکنین و مالکین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }

        public async Task<OperationResult<Response_GetUsersByUnitDomainDTO>> GetUsersByUnitId(Request_UnitIdDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetUsersByUnitDomainDTO> Op = new("GetUsersByUnitId");
            if (model == null)
            {
                return Op.Failed("اطلاعات واحد ارسال نشده است");
            }
            if (model.UnitId <= 0)
            {
                return Op.Failed("شناسه واحد بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Units.AnyAsync(x => x.Id == model.UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }

                List<Response_GetUsersByUnitDomainDTO> result = new();

                result.AddRange((from owner in await _context.Owners.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                 where owner.Unit.Id == model.UnitId && owner.User != null
                                 select new Response_GetUsersByUnitDomainDTO
                                 {
                                     FirstName = owner.User.FirstName,
                                     LastName = owner.User.LastName,
                                     FullName = $"{owner.User.FirstName} {owner.User.LastName}",
                                     Gender = owner.User.Gender,
                                     Id = owner.User.Id,
                                     PhoneNumber = owner.User.PhoneNumber,
                                 }).ToList());

                result.AddRange((from resident in await _context.Residents.Include(x => x.Unit).Include(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                                 where resident.Unit.Id == model.UnitId && resident.User != null
                                 select new Response_GetUsersByUnitDomainDTO
                                 {
                                     FirstName = resident.User.FirstName,
                                     LastName = resident.User.LastName,
                                     FullName = $"{resident.User.FirstName} {resident.User.LastName}",
                                     Gender = resident.User.Gender,
                                     Id = resident.User.Id,
                                     PhoneNumber = resident.User.PhoneNumber,
                                 }).ToList());
                if (result == null || !result.Any())
                {
                    return Op.Succeed("دریافت اطلاعات کاربران با موفقیت انجام شد، اطلاعاتی جهتنمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت اطلاعات کاربران با موفقیت انجام شد", result.GroupBy(x => x.Id).Select(x => x.First()).ToList());
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست کاربران با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> CreateCoreUser(int adminId, Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreateOrModifiedUserIdDomainDTO> Op = new("CreateCoreUser");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
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
            if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber) || !Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09"))
            {
                return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
            }
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }

            if ((model.Residency == null || !model.Residency.Any()) && (model.Ownership == null || !model.Ownership.Any()))
            {
                return Op.Failed("ثبت حداقل یک واحد بعنوان مالک یا ساکن اجباری است");
            }
            if (model.Residency != null && model.Residency.Any())
            {
                if (model.Residency.Any(x => x.UnitId <= 0))
                {
                    return Op.Failed($"شناسه واحد برای سکونت بدرستی ارسال نشده است ، خطا در ردیف {(model.Residency.FindIndex(x => x.UnitId <= 0) + 1)} از لیست واحد های سکونت");
                }
                if (model.Residency.Select(x => x.UnitId).ToList().Count != model.Residency.Select(x => x.UnitId).Distinct().ToList().Count)
                {
                    return Op.Failed("ثبت واحد برای سکونت با شناسه تکراری امکانپذیر نمیباشد");
                }

            }
            if (model.Ownership != null && model.Ownership.Any())
            {
                if (model.Ownership.Any(x => x.UnitId <= 0))
                {
                    return Op.Failed($"شناسه واحد برای مالکیت بدرستی ارسال نشده است ، خطا در ردیف {(model.Ownership.FindIndex(x => x.UnitId <= 0) + 1)} از لیست واحد های مالکیت");
                }
                if (model.Ownership.Select(x => x.UnitId).ToList().Count != model.Ownership.Select(x => x.UnitId).Distinct().ToList().Count)
                {
                    return Op.Failed("ثبت واحد برای مالکیت با شناسه تکراری امکانپذیر نمیباشد");
                }
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var allUnits = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                select u).ToList();
                if (allUnits == null || !allUnits.Any())
                {
                    return Op.Failed("دریافت اطلاعات واحد با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                if (model.Residency != null && model.Residency.Any())
                {
                    for (int i = 0; i < model.Residency.Count; i++)
                    {
                        if (!allUnits.Any(x => x.Id == model.Residency[i].UnitId))
                        {
                            return Op.Failed($"شناسه واحد برای سکونت بدرستی ارسال نشده است ، خطا در ردیف {(i + 1)} از لیست واحد های سکونت", HttpStatusCode.NotFound);
                        }
                    }
                }
                if (model.Ownership != null && model.Ownership.Any())
                {
                    for (int i = 0; i < model.Ownership.Count; i++)
                    {
                        if (!allUnits.Any(x => x.Id == model.Ownership[i].UnitId))
                        {
                            return Op.Failed($"شناسه واحد برای مالکیت بدرستی ارسال نشده است ، خطا در ردیف {(i + 1)} از لیست واحد های مالکیت", HttpStatusCode.NotFound);
                        }
                    }
                }
                if (await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با آدرس ایمیل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                var userModel = new UserModel
                {
                    FirstName = model.Name.Trim(),
                    LastName = model.Lastname.Trim(),
                    Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null,
                    PhoneNumber = model.PhoneNumber.Fa2En(),
                    PhoneNumberConfirmed = true,
                    NationalID = !string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) ? model.NationalID.Fa2En().Trim() : null,
                    Address = !string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address) ? model.Address.Trim() : null,
                    Age = model.Age,
                    Gender = model.Gender,
                    IsDelete = false,
                    CreatedDate = Op.OperationDate,
                    CreatedBy = adminId.ToString(),
                    ModifyDate = Op.OperationDate,
                    ModifyBy = adminId.ToString(),
                };
                await _context.Users.AddAsync(userModel, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                if (model.IsHiddenPhoneNumber != null && model.IsHiddenPhoneNumber == true)
                {
                    await _context.UserLookUp.AddAsync(new UserLookUpModel
                    {
                        BooleanValue = true,
                        Key = "IsHiddenPhoneNumber",
                        UserId = userModel.Id
                    }, cancellationToken);

                }
                if (model.Residency != null && model.Residency.Any())
                {
                    await _context.Residents.AddRangeAsync(model.Residency.Select(x => new ResidentModel
                    {
                        FromDate = Op.OperationDate,
                        ToDate = Op.OperationDate,
                        IsHead = x.IsHead,
                        Unit = allUnits.First(y => y.Id == x.UnitId),
                        User = userModel,
                        Renting = x.Renting,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                    }).ToList(), cancellationToken);

                }
                if (model.Ownership != null && model.Ownership.Any())
                {
                    await _context.Owners.AddRangeAsync(model.Ownership.Select(x => new OwnerModel
                    {
                        FromDate = Op.OperationDate,
                        ToDate = Op.OperationDate,
                        Unit = allUnits.First(y => y.Id == x.UnitId),
                        User = userModel,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                    }).ToList(), cancellationToken);

                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت کاربر با موفقیت انجام شد", new Response_CreateOrModifiedUserIdDomainDTO
                {
                    PhoneNumber = userModel.PhoneNumber,
                    UserId = userModel.Id,
                });

            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> UpdateCoreUser(int adminId, Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_CreateOrModifiedUserIdDomainDTO> Op = new("UpdateCoreUser");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
            {
                return Op.Failed("ارسال نام کاربر اجباری است");
            }
            if (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrWhiteSpace(model.Lastname))
            {
                return Op.Failed("ارسال نام خانوادگی کاربر اجباری است");
            }
            if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber) || !Regex.IsMatch(model.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || model.PhoneNumber.Length != 11 || !model.PhoneNumber.Fa2En().StartsWith("09"))
            {
                return Op.Failed("شماره موبایل کاربر بدرستی وارد شده است");
            }
            if (!string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) && !Regex.IsMatch(model.NationalID.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase))
            {
                return Op.Failed("کد ملی کاربر بدرستی ارسال نشده است");
            }
            if (model.Age != null && model.Age <= 0)
            {
                return Op.Failed("سن کاربر بدرستی وارد ارسال نشده است");
            }
            if (model.Gender != null &&
                !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }
                .Any(x => x == (int)model.Gender))
            {
                return Op.Failed("جنسیت کاربر بدرستی ارسال نشده است");
            }

            if ((model.Residency == null || !model.Residency.Any()) && (model.Ownership == null || !model.Ownership.Any()))
            {
                return Op.Failed("ثبت حداقل یک واحد بعنوان مالک یا ساکن اجباری است");
            }
            if (model.Residency != null && model.Residency.Any())
            {
                if (model.Residency.Any(x => x.UnitId <= 0))
                {
                    return Op.Failed($"شناسه واحد برای سکونت بدرستی ارسال نشده است ، خطا در ردیف {(model.Residency.FindIndex(x => x.UnitId <= 0) + 1)} از لیست واحد های سکونت");
                }
                if (model.Residency.Select(x => x.UnitId).ToList().Count != model.Residency.Select(x => x.UnitId).Distinct().ToList().Count)
                {
                    return Op.Failed("ثبت واحد برای سکونت با شناسه تکراری امکانپذیر نمیباشد");
                }

            }
            if (model.Ownership != null && model.Ownership.Any())
            {
                if (model.Ownership.Any(x => x.UnitId <= 0))
                {
                    return Op.Failed($"شناسه واحد برای مالکیت بدرستی ارسال نشده است ، خطا در ردیف {(model.Ownership.FindIndex(x => x.UnitId <= 0) + 1)} از لیست واحد های مالکیت");
                }
                if (model.Ownership.Select(x => x.UnitId).ToList().Count != model.Ownership.Select(x => x.UnitId).Distinct().ToList().Count)
                {
                    return Op.Failed("ثبت واحد برای مالکیت با شناسه تکراری امکانپذیر نمیباشد");
                }
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var allUnits = (from u in await _context.Units.ToListAsync(cancellationToken: cancellationToken)
                                select u).ToList();
                if (allUnits == null || !allUnits.Any())
                {
                    return Op.Failed("دریافت اطلاعات واحد با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                if (model.Residency != null && model.Residency.Any())
                {
                    for (int i = 0; i < model.Residency.Count; i++)
                    {
                        if (!allUnits.Any(x => x.Id == model.Residency[i].UnitId))
                        {
                            return Op.Failed($"شناسه واحد برای سکونت بدرستی ارسال نشده است ، خطا در ردیف {(i + 1)} از لیست واحد های سکونت", HttpStatusCode.NotFound);
                        }
                    }
                }
                if (model.Ownership != null && model.Ownership.Any())
                {
                    for (int i = 0; i < model.Ownership.Count; i++)
                    {
                        if (!allUnits.Any(x => x.Id == model.Ownership[i].UnitId))
                        {
                            return Op.Failed($"شناسه واحد برای مالکیت بدرستی ارسال نشده است ، خطا در ردیف {(i + 1)} از لیست واحد های مالکیت", HttpStatusCode.NotFound);
                        }
                    }
                }
                if (await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber.Fa2En() && x.IsDelete != true && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با شماره موبایل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) && await _context.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower() == model.Email.Fa2En().ToLower() && x.IsDelete != true && x.Id != model.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("امکان ثبت کاربر با آدرس ایمیل تکراری امکان پذیر نمیباشد", HttpStatusCode.Conflict);
                }
                user.FirstName = model.Name.Trim();
                user.LastName = model.Lastname.Trim();
                user.Email = !string.IsNullOrEmpty(model.Email) && !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Fa2En().Trim() : null;
                user.PhoneNumber = model.PhoneNumber.Fa2En();
                user.NationalID = !string.IsNullOrEmpty(model.NationalID) && !string.IsNullOrWhiteSpace(model.NationalID) ? model.NationalID.Fa2En().Trim() : null;
                user.Address = !string.IsNullOrEmpty(model.Address) && !string.IsNullOrWhiteSpace(model.Address) ? model.Address.Trim() : null;
                user.Age = model.Age;
                user.Gender = model.Gender;
                user.ModifyDate = Op.OperationDate;
                user.ModifyBy = adminId.ToString();

                _context.Entry<UserModel>(user).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);

                if (model.IsHiddenPhoneNumber != null && model.IsHiddenPhoneNumber == true)
                {
                    var config = await _context.UserLookUp.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Key == "IsHiddenPhoneNumber", cancellationToken: cancellationToken);
                    if (config == null)
                    {
                        await _context.UserLookUp.AddAsync(new UserLookUpModel
                        {
                            BooleanValue = true,
                            Key = "IsHiddenPhoneNumber",
                            UserId = user.Id
                        }, cancellationToken);
                    }
                    else
                    {
                        config.BooleanValue = true;
                        _context.Entry<UserLookUpModel>(config).State = EntityState.Modified;
                    }
                }
                _context.Residents.RemoveRange(await _context.Residents.Include(x => x.User).Where(x => x.User.Id == user.Id).ToListAsync(cancellationToken: cancellationToken));
                _context.Owners.RemoveRange(await _context.Owners.Include(x => x.User).Where(x => x.User.Id == user.Id).ToListAsync(cancellationToken: cancellationToken));

                if (model.Residency != null && model.Residency.Any())
                {
                    await _context.Residents.AddRangeAsync(model.Residency.Select(x => new ResidentModel
                    {
                        FromDate = Op.OperationDate,
                        ToDate = Op.OperationDate,
                        IsHead = x.IsHead,
                        Unit = allUnits.First(y => y.Id == x.UnitId),
                        User = user,
                        Renting = x.Renting,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                    }).ToList(), cancellationToken);

                }
                if (model.Ownership != null && model.Ownership.Any())
                {
                    await _context.Owners.AddRangeAsync(model.Ownership.Select(x => new OwnerModel
                    {
                        FromDate = Op.OperationDate,
                        ToDate = Op.OperationDate,
                        Unit = allUnits.First(y => y.Id == x.UnitId),
                        User = user,
                        IsDelete = false,
                        CreatedDate = Op.OperationDate,
                        CreatedBy = adminId.ToString(),
                        ModifyDate = Op.OperationDate,
                        ModifyBy = adminId.ToString(),
                    }).ToList(), cancellationToken);

                }
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی کاربر با موفقیت انجام شد", new Response_CreateOrModifiedUserIdDomainDTO
                {
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.Id,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> DeleteCoreUser(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteCoreUser");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.UserId <= 0)
            {
                return Op.Failed("شناسه کاربر بدرستی ارسال نشده است");
            }
            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == adminId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken: cancellationToken);
                if (user == null)
                {
                    return Op.Failed("شناسه کاربر بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                user.IsDelete = true;
                user.ModifyBy = adminId.ToString();
                user.ModifyDate = Op.OperationDate;
                _context.Entry<UserModel>(user).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف کاربر با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف کاربر با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<Response_GetUserDomainDTO>> GetCoreUsers(Filter_GetUserDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetUserDomainDTO> Op = new("GetCoreUsers");
            if (filter != null)
            {
                if (filter.UserId != null && filter.UserId <= 0)
                {
                    return Op.Failed("شناسه کاربر بدرستی وارد نشده است");
                }
                if (filter.UnitId != null && filter.UnitId <= 0)
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است");
                }
                if (!string.IsNullOrEmpty(filter.PhoneNumber) && !string.IsNullOrWhiteSpace(filter.PhoneNumber) && !Regex.IsMatch(filter.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase))
                {
                    return Op.Failed("شماره موبایل مالک یا ساکن بدرستی وارد نشده است");
                }
            }
            try
            {
                if (filter != null && filter.UserId != null && !await _context.Users.AnyAsync(x => x.Id == filter.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه کاربر بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                if (filter != null && filter.UnitId != null && !await _context.Units.AnyAsync(x => x.Id == filter.UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                var allOwners = await _context.Owners.Include(x => x.User).Include(x => x.Unit).ToListAsync(cancellationToken: cancellationToken);
                if (allOwners == null || !allOwners.Any())
                {
                    return Op.Failed("دریافت اطلاعات مالک ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                var allResidents = await _context.Residents.Include(x => x.User).Include(x => x.Unit).ToListAsync(cancellationToken: cancellationToken);
                if (allResidents == null || !allResidents.Any())
                {
                    return Op.Failed("دریافت اطلاعات ساکن ها با مشکل مواجه شده است", HttpStatusCode.NotFound);
                }
                List<int> basedOnUnitFilteredUserIDs = new();
                basedOnUnitFilteredUserIDs.AddRange((from o in allOwners
                                                     where filter == null || filter.UnitId == null || o.Unit.Id == filter.UnitId
                                                     select o.User.Id));
                basedOnUnitFilteredUserIDs.AddRange((from r in allResidents
                                                     where filter == null || filter.UnitId == null || r.Unit.Id == filter.UnitId
                                                     select r.User.Id));
                List<int> userIDs = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                     join or in basedOnUnitFilteredUserIDs.Distinct().ToList()
                                     on u.Id equals or
                                     where u.IsDelete != true && (filter == null || ((filter.UserId == null || u.Id == filter.UserId) && (string.IsNullOrEmpty(filter.PhoneNumber) || string.IsNullOrWhiteSpace(filter.PhoneNumber) || u.PhoneNumber.Contains(filter.PhoneNumber.Fa2En()))))
                                     select u.Id).ToList();
                if (userIDs == null || !userIDs.Any())
                {
                    return Op.Succeed("دریافت کاربران با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                var userIDsWithHiddenPhoneNumber = (from confing in await _context.UserLookUp.ToListAsync(cancellationToken: cancellationToken)
                                                    where confing.Key.ToLower() == "IsHiddenPhoneNumber".ToLower() && confing.BooleanValue == true
                                                    select confing.UserId).ToList();
                List<Middle_GetUserDataDomainDTO> result = (from u in await _context.Users.ToListAsync(cancellationToken: cancellationToken)
                                                            join id in userIDs
                                                            on u.Id equals id
                                                            select new Middle_GetUserDataDomainDTO
                                                            {
                                                                Address = u.Address,
                                                                Age = u.Age,
                                                                Email = u.Email,
                                                                FirstName = u.FirstName,
                                                                Gender = u.Gender,
                                                                LastName = u.LastName,
                                                                NationalID = u.NationalID,
                                                                PhoneNumber = userIDsWithHiddenPhoneNumber.Any(id => id == u.Id) ? $"{u.PhoneNumber[..2]} ******* {u.PhoneNumber.Substring(9, 2)}" : u.PhoneNumber,
                                                                IsHiddenPhoneNumber = userIDsWithHiddenPhoneNumber.Any(id => id == u.Id),
                                                                RealPhoneNumber = u.PhoneNumber,
                                                                UserId = u.Id,
                                                                Ownerships = allOwners.Any(x => x.User.Id == u.Id) ? allOwners.Where(x => x.User.Id == u.Id).Select(x => new Middle_GetOwnerships_GetUserDataDomainDTO
                                                                {
                                                                    Block = x.Unit.Block,
                                                                    ComplexId = x.Unit.ComplexId,
                                                                    Floor = x.Unit.Floor,
                                                                    UnitId = x.Unit.Id,
                                                                    UnitName = x.Unit.Name,
                                                                }).ToList() : null,
                                                                Residencies = allResidents.Any(x => x.User.Id == u.Id) ? allResidents.Where(x => x.User.Id == u.Id).Select(x => new Middle_GetResidencies_GetUserDataDomainDTO
                                                                {
                                                                    Block = x.Unit.Block,
                                                                    ComplexId = x.Unit.ComplexId,
                                                                    Floor = x.Unit.Floor,
                                                                    UnitId = x.Unit.Id,
                                                                    UnitName = x.Unit.Name,
                                                                    IsHead = x.IsHead,
                                                                    Renting = x.Renting
                                                                }).ToList() : null,
                                                            }).Skip((Convert.ToInt32(filter == null ? 1 : filter.PageNumber) - 1) * Convert.ToInt32(filter == null ? 20 : filter.PageSize)).Take(Convert.ToInt32(filter == null ? 20 : filter.PageSize)).ToList();
                return Op.Succeed("دریافت اطلاعات کاربران با موفقیت انجام شد", new Response_GetUserDomainDTO
                {
                    TotalCount = userIDs.Count,
                    Persons = result
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اطلاعات کاربران با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
