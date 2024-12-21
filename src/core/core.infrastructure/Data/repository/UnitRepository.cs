using core.application.Contract.API.DTO.Structor.Unit;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs;
using core.domain.entity.enums;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;

namespace core.infrastructure.Data.repository
{
    public class UnitRepository : IUnitRepository
    {
        private EnjoyLifeContext _context;

        public UnitRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public async Task<List<UnitModel>> GetUserUnitsAsync(int userId, bool IsHead = true)
        {
            var unitIDs = new List<int>();
            unitIDs.AddRange((from owner in await _context.Owners.Include(x => x.User).Include(x => x.Unit).ToListAsync()
                              where owner.User.Id == userId
                              select owner.Unit.Id).ToList());
            unitIDs.AddRange((from resident in await _context.Residents.Include(x => x.User).Include(x => x.Unit).ToListAsync()
                              where resident.User.Id == userId
                              select resident.Unit.Id).ToList());
            return (from unit in await _context.Units.Include(z => z.Residents).ThenInclude(r => r.User).Include(p => p.Parkings).Include(s => s.StorageLots).ToListAsync()
                    join id in unitIDs.Distinct().ToList()
                    on unit.Id equals id
                    select unit).ToList();
        }
        public async Task<IEnumerable<UnitModel>> GetMyResidentalUnitsAsync(int userId)
        {
            var result = _context.Units
                 .Where(x => (x.Residents.Any(j => j.User.Id == userId)))
                 .AsQueryable();
            return result;
        }

        public async Task<IEnumerable<UnitModel>> GetAllUsersInUnitAsync(int userId)
        {
            try
            {
                if (!await _context.Residents.Include(x => x.User).AnyAsync(x => x.User.Id == userId) && !await _context.Owners.Include(x => x.User).AnyAsync(x => x.User.Id == userId))
                {
                    return new List<UnitModel>();
                }
                var unitIDs = new List<int>();

                //inOwner
                unitIDs.AddRange((from owner in await _context.Owners.Include(x => x.Unit).Include(x => x.User).ToListAsync()
                                  join unit in await _context.Units.ToListAsync()
                                  on owner.Unit.Id equals unit.Id
                                  where owner.User.Id == userId
                                  select unit.Id).ToList());
                //inResident
                unitIDs.AddRange((from resident in await _context.Residents.Include(x => x.Unit).Include(x => x.User).ToListAsync()
                                  join unit in await _context.Units.ToListAsync()
                                  on resident.Unit.Id equals unit.Id
                                  where resident.User.Id == userId
                                  select unit.Id).ToList());
                if (unitIDs == null || !unitIDs.Any())
                {
                    return new List<UnitModel>();
                }
                return (from unit in await _context.Units.Include(x => x.Residents).ThenInclude(x => x.User).Include(x => x.Owners).ThenInclude(x => x.User).ToListAsync()
                        join id in unitIDs.Distinct().ToList()
                        on unit.Id equals id
                        select unit).ToList();
            }
            catch (Exception)
            {
                return new List<UnitModel>();
            }

            //var result = await _context.Units
            //    .Where(x =>
            //        x.Owners.Any(j => j.User.Id == userId && !x.Residents.Any(r => r.Unit.Id == x.Id && j.Id == r.User.Id)) ||  // && !x.Residents.Any(r=>r.Unit.Id==x.Id && j.Id ==r.User.Id)
            //        x.Residents.Any(j => j.User.Id == userId && j.IsHead)
            //    )
            //    .Include(z => z.Residents.Where(a => a.IsHead || a.User.Id==userId)).ThenInclude(r => r.User)
            //    .Include(o => o.Owners).ThenInclude(r => r.User)
            //    .ToListAsync();

            //// Now, let's find units where the specified user is a resident but not a head
            //var additionalUnits = await _context.Units
            //    .Where(x =>
            //        x.Residents.Any(j => j.User.Id == userId && !j.IsHead) &&  // Non-head residents can see their units
            //        !x.Owners.Any(j => j.User.Id == userId)                     // Exclude units already included for owners
            //    )
            //    .Include(z => z.Residents).ThenInclude(r => r.User)
            //    .Include(o => o.Owners).ThenInclude(r => r.User)
            //    .ToListAsync();

            //// Combine the results
            //result.AddRange(additionalUnits);

            //return result;
        }



        public async Task<IEnumerable<UnitModel>> GetAllAsync(UnitRequestDTO filter)
        {
            var tempquery = _context.Units.AsQueryable();
            if (filter.ComplexId != null) { tempquery = tempquery.Where(x => x.ComplexId == filter.ComplexId); }
            if (filter.Floor != null) { tempquery = tempquery.Where(x => x.Floor == filter.Floor); }
            if (filter.Block != null) { tempquery = tempquery.Where(x => x.Block == filter.Block); }
            return tempquery.Include(x => x.Parkings).Include(x => x.StorageLots).ToList();

        }

        public async Task<IEnumerable<UnitModel>> GetAllAsync()
        {
            return await _context.Units.ToListAsync();
        }

        public async Task<UnitModel> GetByIdAsync(int id)
        {
            return _context.Units.Where(x => x.Id == id).Include(x => x.Parkings).Include(x => x.StorageLots).FirstOrDefault();
        }
        public async Task<ResidentModel> IsUnitHeadAsync(int unitId, int userId)
        {
            //return _context.Units.Any(x => x.Id == unitId && x.Residents.Any(r=>r.User.Id==userId && r.IsHead==true));
            return await _context.Residents.Where(x => x.Unit.Id == unitId && x.User.Id == userId && x.IsHead == true).Include(u => u.User).Include(h => h.Unit).FirstOrDefaultAsync();
        }

        public async Task<UnitModel> AddAsync(UnitModel unitModel)
        {

            if (!_context.Complexes.Any(x => x.Id == unitModel.ComplexId))
            {
                throw new Exception("Complex doesnt exist");
            }

            await _context.Units.AddAsync(unitModel);
            await _context.SaveChangesAsync();
            return unitModel;
        }

        public async Task<int> UpdateAsync(UnitModel unit)
        {

            _context.Units.Update(unit);
            return await _context.SaveChangesAsync();

        }

        public async Task DeleteParking(int parkingId)
        {
            var parking = await _context.Parkings.SingleOrDefaultAsync(p => p.Id == parkingId) ?? throw new Exception("parking not found");
            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();
        }

        public async Task<Parking> AddParking(AddParkingDTO parkingDTO)
        {
            var unit = await _context.Units.Where(u => u.Id == parkingDTO.UnitId).SingleOrDefaultAsync() ?? throw new Exception("unit not found");
            Parking p = new Parking() { Name = parkingDTO.ParkingName };
            unit.Parkings.Add(p);
            _context.Update(unit);
            await _context.SaveChangesAsync();
            return p;
        }
        public async Task DeleteStorageLot(int storageId)
        {
            var storage = await _context.StorageLots.SingleOrDefaultAsync(p => p.Id == storageId) ?? throw new Exception("storage lot not found");
            _context.StorageLots.Remove(storage);
            await _context.SaveChangesAsync();
        }

        public async Task<StorageLot> AddStorageLot(AddStorageLotDTO storageLotDTO)
        {
            var unit = await _context.Units.Where(u => u.Id == storageLotDTO.UnitId).SingleOrDefaultAsync() ?? throw new Exception("unit not found");
            StorageLot p = new StorageLot() { Name = storageLotDTO.StorageLotName };
            unit.StorageLots.Add(p);
            _context.Update(unit);
            await _context.SaveChangesAsync();
            return p;
        }

        public async Task<OperationResult<Response_GetUnitOwnersResidentsDomainDTO>> GetUnitOwnersResidents(Filter_GetUnitOwnersResidentsDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Response_GetUnitOwnersResidentsDomainDTO> Op = new("GetUnitOwnersResidents");
            if (filter != null)
            {
                if (filter.UnitId != null && filter.UnitId <= 0)
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است");
                }
                if (filter.UserId != null && filter.UserId <= 0)
                {
                    return Op.Failed("شناسه کاربر بدرستی وارد نشده است");
                }
                if (!string.IsNullOrEmpty(filter.PhoneNumber) && !string.IsNullOrWhiteSpace(filter.PhoneNumber) && !Regex.IsMatch(filter.PhoneNumber, "^[0-9]*$", RegexOptions.IgnoreCase))
                {
                    return Op.Failed("شماره موبایل مالک یا ساکن بدرستی وارد نشده است");
                }
                if (filter.Gender != null && !new List<int> {
                    (int)GenderType.FAMALE ,
                    (int)GenderType.MALE ,
                    (int)GenderType.ALL }.ToList().Any(x => x == (int)filter.Gender))
                {
                    return Op.Failed("جنسیت مالک یا ساکن بدرستی وارد نشده است");
                }
            }
            try
            {
                if (filter != null && filter.UnitId != null && !await _context.Units.AnyAsync(x => x.Id == filter.UnitId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه واحد بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }
                if (filter != null && filter.UserId != null && !await _context.Users.AnyAsync(x => x.Id == filter.UserId, cancellationToken: cancellationToken))
                {
                    return Op.Failed("شناسه کاربر بدرستی وارد نشده است", HttpStatusCode.NotFound);
                }

                var units = (from unit in await _context.Units.Include(x => x.Owners).ThenInclude(x => x.User).Include(x => x.Residents).ThenInclude(x => x.User).ToListAsync(cancellationToken: cancellationToken)
                             where filter == null ||
                             (
                                (filter.UnitId == null || unit.Id == filter.UnitId) &&
                                (string.IsNullOrEmpty(filter.UnitName) || string.IsNullOrWhiteSpace(filter.UnitName) || unit.Name.ToLower().Contains(filter.UnitName.Trim().ToLower())) &&
                                (filter.Floor == null || unit.Floor == filter.Floor)
                             )
                             orderby unit.Floor
                             select unit).ToList();
                if (units == null || !units.Any())
                {
                    return Op.Succeed("دریافت لیست ساکنین و مالکین با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }

                var UserIDsWithHiddenPhoneNumber = (from confing in await _context.UserLookUp.ToListAsync(cancellationToken: cancellationToken)
                                                    where confing.Key.ToLower() == "IsHiddenPhoneNumber".ToLower() && confing.BooleanValue == true
                                                    select confing.UserId).ToList();

                var persons = new List<GetUnitOwnersResidentsDomainDTO>();

                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].Owners != null && units[i].Owners.Any())
                    {
                        persons.AddRange(units[i].Owners.Where(x =>
                                (x.User.IsDelete != true) && (filter == null || (
                                        (filter.UserId == null || x.User.Id == filter.UserId) &&
                                        (string.IsNullOrEmpty(filter.FullName) || string.IsNullOrWhiteSpace(filter.FullName) || $"{x.User.FirstName} {x.User.LastName}".ToLower().Contains(filter.FullName.Trim().ToLower())) &&
                                        (string.IsNullOrEmpty(filter.PhoneNumber) || string.IsNullOrWhiteSpace(filter.PhoneNumber) || x.User.PhoneNumber.ToLower().StartsWith(filter.PhoneNumber.Trim().ToLower())) &&
                                        (filter.Gender == null || x.User.Gender != null && (int)x.User.Gender == (int)filter.Gender)
                                ))
                            ).Select(x => new GetUnitOwnersResidentsDomainDTO
                            {
                                Address = x.User.Address,
                                Age = x.User.Age,
                                Block = units[i].Block,
                                ComplexId = units[i].ComplexId,
                                Email = x.User.Email,
                                FirstName = x.User.FirstName,
                                Floor = units[i].Floor,
                                FromDate = x.FromDate,
                                Gender = x.User.Gender,
                                IsHead = null,
                                IsOwner = true,
                                LastName = x.User.LastName,
                                NationalID = x.User.NationalID,
                                PhoneNumber = UserIDsWithHiddenPhoneNumber.Any(id => id == x.User.Id) ? $"{x.User.PhoneNumber[..2]} xxxxxxx {x.User.PhoneNumber.Substring(9, 2)}" : x.User.PhoneNumber,
                                Renting = null,
                                ToDate = x.ToDate,
                                UnitId = units[i].Id,
                                UnitName = units[i].Name,
                                UserId = x.User.Id,
                                IsHiddenPhoneNumber = UserIDsWithHiddenPhoneNumber.Any(id => id == x.User.Id)
                            }));
                    }
                    if (units[i].Residents != null && units[i].Residents.Any())
                    {
                        persons.AddRange(units[i].Residents.Where(x =>
                            ((x.User.IsDelete != true) &&
                            (filter == null || (
                                    (filter.UserId == null || x.User.Id == filter.UserId) &&
                                    (string.IsNullOrEmpty(filter.FullName) || string.IsNullOrWhiteSpace(filter.FullName) || $"{x.User.FirstName} {x.User.LastName}".ToLower().Contains(filter.FullName.Trim().ToLower())) &&
                                    (string.IsNullOrEmpty(filter.PhoneNumber) || string.IsNullOrWhiteSpace(filter.PhoneNumber) || x.User.PhoneNumber.ToLower().StartsWith(filter.PhoneNumber.Trim().ToLower())) &&
                                    (filter.Gender == null || x.User.Gender != null && (int)x.User.Gender == (int)filter.Gender) &&
                                    (filter.IsHeadResident == null || x.IsHead == filter.IsHeadResident) &&
                                    (filter.RentingResident == null || x.Renting == filter.RentingResident))
                            ))
                            ).Select(x => new GetUnitOwnersResidentsDomainDTO
                            {
                                Address = x.User.Address,
                                Age = x.User.Age,
                                Block = units[i].Block,
                                ComplexId = units[i].ComplexId,
                                Email = x.User.Email,
                                FirstName = x.User.FirstName,
                                Floor = units[i].Floor,
                                FromDate = x.FromDate,
                                Gender = x.User.Gender,
                                IsHead = x.IsHead,
                                IsOwner = false,
                                LastName = x.User.LastName,
                                NationalID = x.User.NationalID,
                                PhoneNumber = UserIDsWithHiddenPhoneNumber.Any(id => id == x.User.Id) ? $"{x.User.PhoneNumber[..2]} xxxxxxx {x.User.PhoneNumber.Substring(9, 2)}" : x.User.PhoneNumber,
                                Renting = x.Renting,
                                ToDate = x.ToDate,
                                UnitId = units[i].Id,
                                UnitName = units[i].Name,
                                UserId = x.User.Id,
                                IsHiddenPhoneNumber = UserIDsWithHiddenPhoneNumber.Any(id => id == x.User.Id)
                            }));
                    }
                }
                if (persons.Count == 0)
                {
                    return Op.Succeed("دریافت لیست ساکنین و مالکین با موفقیت انجام شد ، اطلاعاتی جهت نمایش وجود ندارد", HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت لیست ساکنین و مالکین با موفقیت انجام شد", new Response_GetUnitOwnersResidentsDomainDTO
                {
                    TotalCount = persons.Count,
                    Persons = persons.OrderBy(x => x.UnitId).ThenByDescending(x => x.IsOwner).ThenByDescending(x => x.IsHead).Skip((Convert.ToInt32(filter == null ? 1 : filter.PageNumber) - 1) * Convert.ToInt32(filter == null ? 20 : filter.PageSize)).Take(Convert.ToInt32(filter == null ? 20 : filter.PageSize)).ToList(),
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت لیست ساکنین و مالکین با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
