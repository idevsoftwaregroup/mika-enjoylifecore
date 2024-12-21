using core.application.Contract.API.DTO.Structor.Unit;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs;
using core.domain.entity.structureModels;

namespace core.application.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }
        public async Task<IEnumerable<UnitResponseDTO>> getAllunit(UnitRequestDTO filter)
        {
            var result = _unitRepository
                    .GetAllAsync(filter)
                    .Result.Select(x => x.UnitModelToResponseDto())
                    .ToList();
            return result;
        }
        public async Task<IEnumerable<UnitUserResponseDTO>> GetUserUnits(int userID, bool IsHead = true)
        {
            var result = _unitRepository
        .GetUserUnitsAsync(userID, IsHead)
        .Result.Select(x => x.UnitUserModelToResponseDto())
        .ToList();
            return result;
        }


        public async Task<IEnumerable<UnitUserResponseDTO>> GetMyResidentalUnits(int userID)
        {
            var result = _unitRepository.GetMyResidentalUnitsAsync(userID).Result
                .Select(x => x.UnitUserModelToResponseWithoutRelationsDto()).ToList();
            return result;
        }

        public async Task<IEnumerable<UnitUserResponseDTO>> GetAllUsersInUnit(int userID)
        {
            var result = await _unitRepository.GetAllUsersInUnitAsync(userID);
            if (result == null || !result.Any())
            {
                return new List<UnitUserResponseDTO>();
            }
            return result.Select(x => x.UnitUserModelToResponseDto());
        }

        public async Task<UnitResponseDTO> GetUnitById(int id)
        {
            var unitModel = _unitRepository
                    .GetByIdAsync(id)
                    .Result
                    .UnitModelToResponseDto();
            return unitModel;
        }
        public async Task<FrontGetUnitDTO> GetUnitByIdFront(int id)
        {
            var unitModel = _unitRepository
                    .GetByIdAsync(id)
                    .Result
                    .UnitModelToResponseDtoFront();
            return unitModel;
        }
        public async Task<int> CreateUnit(UnitCreateRequestDTO unitCreateDTO)
        {
            var unitModel = await _unitRepository
                .AddAsync(unitCreateDTO.UnitCreateRequestDTOToModel());
            return unitModel.Id;
        }
        public async Task<bool> UpdateUnit(UnitUpdateRequestDTO unitUpdateDTO)
        {
            UnitModel unitrModel = unitUpdateDTO.UnitUpdateRequestDTOToModel();
            return await _unitRepository.UpdateAsync(unitrModel) > 0;
        }

        public async Task DeleteParking(int parkingId)
        {
            await _unitRepository.DeleteParking(parkingId);
        }

        public Task<Parking> AddParking(AddParkingDTO parkingDTO)
        {
            return _unitRepository.AddParking(parkingDTO);
        }

        public async Task DeleteStorageLot(int storageId)
        {
            await _unitRepository.DeleteStorageLot(storageId);
        }

        public Task<StorageLot> AddStorageLot(AddStorageLotDTO storageLotDTO)
        {
            return _unitRepository.AddStorageLot(storageLotDTO);
        }

        public async Task<OperationResult<Response_GetUnitOwnersResidentsDomainDTO>> GetUnitOwnersResidents(Filter_GetUnitOwnersResidentsDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            return await _unitRepository.GetUnitOwnersResidents(filter, cancellationToken);
        }
    }
}
