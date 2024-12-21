using core.application.Contract.API.DTO.Party.Manager;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.partyModels;
using core.infrastructure.Data.persist;
using core.infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace core.infrastructure.Data.repository
{
    public class ManagerRepository : IManagerRepository
    {
        private EnjoyLifeContext _context;


        public ManagerRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public async Task<int> AddManagerAsync(ManagerCreateRequest managerCreateRequest)
        {
            try
            {
                var managerModel = managerCreateRequest.CovnertManagerCreateRequestToModel();
                _context.Complexes.Attach(managerModel.Complex);
                _context.Users.Attach(managerModel.User);
                _context.Managers.Add(managerModel);
                await _context.SaveChangesAsync();
                return managerModel.Id;
            }
            catch (Exception e)
            {
                throw new InfrastureException($"when  AddManagerAsync- {JsonConvert.SerializeObject(managerCreateRequest)}- this error happen- {e.Message}");
            }
        }

        public List<ManagerModel> GetComplexManagers(int complexId)
        {
            try
            {
                return _context.Managers.Where(x => x.Complex.Id == complexId)
                    .Include(r => r.Complex)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new InfrastureException($"when GetComplexManagers- {JsonConvert.SerializeObject(new { id = complexId })}- this error happen- {e.Message}");
            }
        }

        public ManagerModel GetManager(int managerId)
        {
            try
            {
                return _context.Managers.Where(x => x.Id == managerId)
                    .Include(r => r.Complex)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new InfrastureException($"when manger GetManager- {JsonConvert.SerializeObject(new { id = managerId })}- this error happen- {e.Message}");
            }
        }

        public async Task<int> UpdateManagerAsync(ManagerUpdateRequest managerUpdateRequest)
        {
            try
            {
                var managerModel = managerUpdateRequest.CovnertManagerUpdateRequestToModel();
                _context.Complexes.Attach(managerModel.Complex);
                _context.Users.Attach(managerModel.User);
                _context.Managers.Update(managerModel);
                return await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new InfrastureException($"when manger UpdateManagerAsync- {JsonConvert.SerializeObject(managerUpdateRequest)}- this error happen- {e.Message}");
            }
        }
    }
}
