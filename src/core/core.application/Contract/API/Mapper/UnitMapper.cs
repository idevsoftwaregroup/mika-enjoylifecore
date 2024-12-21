using core.application.Contract.API.DTO.Structor.Unit;
using core.domain.entity.enums;
using core.domain.entity.structureModels;

namespace core.application.Contract.API.Mapper;

public static class UnitMapper
{
    public static UnitResponseDTO UnitModelToResponseDto(this UnitModel value)
    {
        return new()
        {
            Directions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),
            Positions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),
            PositionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>().Where(e => value.Directions.HasFlag(e))
                    .Select(e => e.GetDescription())
                    .ToList(),
            UnitUsages = value.UnitUsages,
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Id = value.Id,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            //TotalStorageLots = value.TotalStorageLots,
            //TotalParkingLots=value.TotalParkingLots,
            Parkings = value.Parkings.ToList(),
            StorageLots = value.StorageLots.ToList(),
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl
        };
    }

    public static FrontGetUnitDTO UnitModelToResponseDtoFront(this UnitModel value)
    {
        return new()
        {
            Directions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),
            Positions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),
            PositionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>().Where(e => value.Directions.HasFlag(e))
                    .Select(e => e.GetDescription())
                    .ToList(),
            UnitUsages = value.UnitUsages,
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Id = value.Id,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            //TotalStorageLots = value.TotalStorageLots,
            //TotalParkingLots=value.TotalParkingLots,
            StorageLots = value.StorageLots.Select(s => s.Name).ToList(),
            Parkings = value.Parkings.Select(p => p.Name).ToList(),
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl
        };
    }

    public static UnitUserResponseDTO UnitUserModelToResponseDto(this UnitModel value)
    {
        return new()
        {
            Directions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),

            DirectionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>()
                    .Where(e => value.Directions == DirectionType.NONE || (e != DirectionType.NONE && value.Directions.HasFlag(e)))
                    .Select(e => e.GetDescription())
                    .ToList(),
            Positions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Positions.HasFlag(e))
                                    .ToList(),

            PositionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>()
                    .Where(e => value.Positions == DirectionType.NONE || (e != DirectionType.NONE && value.Positions.HasFlag(e)))
                    .Select(e => e.GetDescription())
                    .ToList(),


            UnitUsages = value.UnitUsages,
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Id = value.Id,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            ResidentalStatus = value.Residents.Any(r => r.Renting == true) ? 1 : 0,
            Residents = value.Residents != null ? value.Residents.Select(x => x.ConvertResidentModelTOResidentGetResponse()).ToList() : null,
            Owners = value.Owners != null ? value.Owners.Select(x => x.ConvertOwnerModelToGetResponse()).ToList() : null,
            Parkings = value.Parkings.Select(p => p.Name).ToList(),
            StorageLots = value.StorageLots.Select(s => s.Name).ToList(),
            //TotalStorageLot =value.TotalStorageLots,
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl
        };
    }

    public static UnitUserResponseDTO UnitUserModelToResponseWithoutRelationsDto(this UnitModel value)
    {
        return new()
        {
            Directions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),

            DirectionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>()
                    .Where(e => value.Directions == DirectionType.NONE || (e != DirectionType.NONE && value.Directions.HasFlag(e)))
                    .Select(e => e.GetDescription())
                    .ToList(),
            Positions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Positions.HasFlag(e))
                                    .ToList(),

            PositionsDescription = Enum.GetValues(typeof(DirectionType))
                    .Cast<DirectionType>()
                    .Where(e => value.Positions == DirectionType.NONE || (e != DirectionType.NONE && value.Positions.HasFlag(e)))
                    .Select(e => e.GetDescription())
                    .ToList(),


            UnitUsages = value.UnitUsages,
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Id = value.Id,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl
        };
    }


    public static UnitModel UnitCreateRequestDTOToModel(this UnitCreateRequestDTO value)
    {
        return new()
        {
            UnitUsages = value.UnitUsages,
            Directions = value.Directions.Aggregate((x, y) => x | y),
            Positions = value.Positions.Aggregate((x, y) => x | y),
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            //TotalStorageLots=value.TotalStorageLot,
            Parkings = value.Parkings.Select(p => new Parking() { Name = p }).ToList(),
            StorageLots = value.StorageLots.Select(s => new StorageLot() { Name = s }).ToList(),
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl,
        };
    }

    public static UnitModel UnitUpdateRequestDTOToModel(this UnitUpdateRequestDTO value)
    {
        return new()
        {
            UnitUsages = value.UnitUsages,
            Directions = value.Directions.Aggregate((x, y) => x | y),
            Positions = value.Positions.Aggregate((x, y) => x | y),
            Id = value.Id,
            Type = value.Type,
            Block = value.Block,
            ComplexId = value.ComplexId,
            Floor = value.Floor,
            Meterage = value.Meterage,
            Name = value.Name,
            UnitElectricityCounterNumber = value.UnitElectricityCounterNumber,
            UnitPLanMapFileUrl = value.UnitPLanMapFileUrl,
        };
    }
}
