using core.application.Contract.API.DTO.Complex;
using core.domain.entity.enums;
using core.domain.entity.structureModels;
using System.Data;

namespace core.application.Contract.API.Mapper
{
    public static class ComplexMapper
    {
        public static ComplexModel ConvertUpdateRequestToComplexModel(this ComplexUpdateRequestDTO value)
        {
            return new ComplexModel()
            {
                Id = value.Id,
                Address = value.Address,
                Description = value.Description,
                Directions = value.Directions.Aggregate((x, y) => x | y),
                Positions = value.Positions.Aggregate((x, y) => x | y),
                Usages = value.Usages.Aggregate((x, y) => x | y),
                Title = value.Title
            };
        }
        public static ComplexGetResponseDTO ConvertComplexModelToGetComplexResponseDTO(this ComplexModel value)
        {
            return new ComplexGetResponseDTO()
            {
                Address = value.Address,
                Description = value.Description,
                Directions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Directions.HasFlag(e))
                                    .ToList(),
                Positions = Enum.GetValues(typeof(DirectionType))
                                    .Cast<DirectionType>()
                                    .Where(e => value.Positions.HasFlag(e))
                                    .ToList(),
                Usages = Enum.GetValues(typeof(ComplexUsageType))
                                    .Cast<ComplexUsageType>()
                                    .Where(e => value.Usages.HasFlag(e))
                                    .ToList(),
                Title = value.Title,
            };
        }

        public static ComplexModel ConvertCreateRequestToComplexModel(this ComplextCreateRequestDTO value)
        {
            return new ComplexModel()
            {
                Address = value.Address,
                Description = value.Description,
                Directions = value.Directions.Aggregate((x, y) => x | y),
                Positions = value.Positions.Aggregate((x, y) => x | y),
                Usages = value.Usages.Aggregate((x, y) => x | y),
                Title = value.Title
            };
        }
    }
}

