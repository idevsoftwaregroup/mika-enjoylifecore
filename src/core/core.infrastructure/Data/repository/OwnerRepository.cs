using core.application.Contract.API.DTO.Party.Owner;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.partyModels;
using core.infrastructure.Data.persist;
using core.infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace core.infrastructure.Data.repository;

public class OwnerRepository : IOwnerRepository
{
    private EnjoyLifeContext _context;


    public OwnerRepository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(OwnerCreateRequest ownerCreateDTO)
    {
        try
        {
            var ownerModel = ownerCreateDTO.ConvertCreateRequestToModel();
            _context.Units.Attach(ownerModel.Unit);
            _context.Users.Attach(ownerModel.User);
            _context.Add(ownerModel);
            await _context.SaveChangesAsync();
            return ownerModel.Id;
        }
        catch (Exception e)
        {

            throw new InfrastureException($"when owner AddAsync create -{JsonConvert.SerializeObject(ownerCreateDTO)}- this error happen -{e.Message}");
        }
    }

    public List<OwnerModel> GetOwners(OwnerGetRequestFilter filter)
    {
        try
        {
            var tempquery = _context.Owners
                .Include(r => r.Unit)
                .Include(r => r.User)
                .AsQueryable();

            if (filter.UserId != null) { tempquery = tempquery.Where(x => x.User.Id == filter.UserId); }

            if (filter.UnitId != null) { tempquery = tempquery.Where(x => x.Unit.Id == filter.UnitId); }


            return tempquery.ToList();
        }
        catch (Exception e)
        {

            throw new InfrastureException($"when GetOwners GetAllAsync- {JsonConvert.SerializeObject(filter)}- this error happen- {e.Message}");
        }
    }

    public async Task<int> UpdateAsync(OwnerUpdateRequest ownerUpdateRequest)
    {
        try
        {
            var ownerModel = ownerUpdateRequest.ConvertOwnerModelToGetResponse();
            _context.Units.Attach(ownerModel.Unit);
            _context.Users.Attach(ownerModel.User);
            _context.Owners.Update(ownerModel);
            return await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {

            throw new InfrastureException($"when owner UpdateAsync- {JsonConvert.SerializeObject(ownerUpdateRequest)}- this error happen- {e.Message}");
        }
    }
}
