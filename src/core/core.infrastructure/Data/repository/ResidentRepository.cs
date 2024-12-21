using core.application.Contract.API.DTO.Party.Resident;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.partyModels;
using core.infrastructure.Data.persist;
using core.infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace core.infrastructure.Data.repository;

public class ResidentRepository : IResidentRepository
{
    private readonly EnjoyLifeContext _context;

    public ResidentRepository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public async Task<int> CreateResidentAsync(ResidentCreateRequest residentCreateRequest)
    {
        try
        {
            var residentModel = residentCreateRequest.ConvertResidentCreateRequestToModel();
            _context.Units.Attach(residentModel.Unit);
            _context.Users.Attach(residentModel.User);
            _context.Residents.Update(residentModel);
            return await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {

            throw new InfrastureException($"when resident CreateResidentAsync- {JsonConvert.SerializeObject(residentCreateRequest)}- this error happen- {e.Message}");
        }
    }

    public List<ResidentModel> GetResidents(ResidentGetRequestFilter filter)
    {
        try
        {
            var tempquery = _context.Residents
                .Include(r => r.Unit)
                .Include(r => r.User)
    .AsQueryable();

            if (filter.UserId != null) { tempquery = tempquery.Where(x => x.User.Id == filter.UserId); }

            if (filter.UnitId != null) { tempquery = tempquery.Where(x => x.Unit.Id == filter.UnitId); }
            if (filter.Renting != null) { tempquery = tempquery.Where(x => x.Renting == filter.Renting); }
            if (filter.Head != null) { tempquery = tempquery.Where(x => x.IsHead == filter.Head); }


            return tempquery.ToList();

        }
        catch (Exception e)
        {

            throw new InfrastureException($"when resident GetResidents- {JsonConvert.SerializeObject(filter)} - this error happen- {e.Message}");
        }
    }
    public async Task<int?> GetUnitIdByUser(int userId)
    {
        if (userId <= 0)
        {
            return null;
        }
        try
        {
            var residentsQuery = _context.Residents
                .Include(r => r.Unit)
                .Include(r => r.User)
                .Where(x => x.User.Id == userId);

            return (from r in await residentsQuery.ToListAsync()
                    select r.Unit).FirstOrDefault()?.Id;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<int> UpdateResidentAsync(ResidentUpdateRequest residentCreateRequest)
    {
        try
        {
            var residentModel = residentCreateRequest.ConvertResidentUpdateRequestToModel();
            _context.Units.Attach(residentModel.Unit);
            _context.Users.Attach(residentModel.User);
            _context.Residents.Update(residentModel);
            return await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {

            throw new InfrastureException($"when owner UpdateResidentAsync- {JsonConvert.SerializeObject(residentCreateRequest)}- this error happen- {e.Message}");
        }
    }


}
