using core.application.Contract.API.DTO.Structor.Unit;
using core.application.Framework;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;

namespace core.application.Contract.Infrastructure;

public interface IUnitRepository
{
    Task<UnitModel> AddAsync(UnitModel unitModel);
    Task<IEnumerable<UnitModel>> GetAllAsync(UnitRequestDTO filter);
    Task<IEnumerable<UnitModel>> GetAllAsync();
    Task<ResidentModel> IsUnitHeadAsync(int unitId, int userId);
    Task<List<UnitModel>> GetUserUnitsAsync(int userId, bool IsHead = true);
    Task<IEnumerable<UnitModel>> GetMyResidentalUnitsAsync(int userId);
    Task<IEnumerable<UnitModel>> GetAllUsersInUnitAsync(int userId);
    Task<UnitModel> GetByIdAsync(int id);
    Task<int> UpdateAsync(UnitModel unit);
    Task DeleteParking(int unitID);
    Task<Parking> AddParking(AddParkingDTO parkingDTO);
    Task DeleteStorageLot(int storageId);
    Task<StorageLot> AddStorageLot(AddStorageLotDTO storageLotDTO);
    Task<OperationResult<Response_GetUnitOwnersResidentsDomainDTO>> GetUnitOwnersResidents(Filter_GetUnitOwnersResidentsDomainDTO? filter, CancellationToken cancellationToken = default);
}
