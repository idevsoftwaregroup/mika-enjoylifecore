using core.application.Contract.API.DTO.Structor.Unit;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs;
using core.domain.entity.structureModels;
using core.application.Framework;

namespace core.application.Contract.API.Interfaces;

public interface IUnitService
{
    Task<int> CreateUnit(UnitCreateRequestDTO unitCreateDTO);
    Task<IEnumerable<UnitResponseDTO>> getAllunit(UnitRequestDTO filter);
    Task<UnitResponseDTO> GetUnitById(int id);
    Task<FrontGetUnitDTO> GetUnitByIdFront(int id);
    Task<IEnumerable<UnitUserResponseDTO>> GetUserUnits(int userID, bool IsHead = true);
    Task<IEnumerable<UnitUserResponseDTO>> GetMyResidentalUnits(int userID);
    Task<IEnumerable<UnitUserResponseDTO>> GetAllUsersInUnit(int userID);
    Task<bool> UpdateUnit(UnitUpdateRequestDTO unitUpdateDTO);
    Task DeleteParking(int parkingId);
    Task<Parking> AddParking(AddParkingDTO parkingDTO);
    Task DeleteStorageLot(int storageId);
    Task<StorageLot> AddStorageLot(AddStorageLotDTO storageLotDTO);
    Task<OperationResult<Response_GetUnitOwnersResidentsDomainDTO>> GetUnitOwnersResidents(Filter_GetUnitOwnersResidentsDomainDTO? filter, CancellationToken cancellationToken = default);
}
