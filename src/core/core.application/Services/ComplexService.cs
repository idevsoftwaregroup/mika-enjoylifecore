using core.application.Contract.API.DTO.Complex;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.structureModels;

namespace core.application.Services
{
    public class ComplexService : IComplexService
    {
        public IComplexRepository _complexRepository { get; set; }


        public ComplexService(IComplexRepository complexRepository)
        {
            _complexRepository = complexRepository;
        }

        public async Task<ComplexGetResponseDTO> GetComplexById(int id)
        {
            ComplexModel complexModel = await _complexRepository.GetAsync(id);
            if (complexModel == null)
                return null;
            var dto = complexModel.ConvertComplexModelToGetComplexResponseDTO();
            return dto;
        }

        public async Task<IEnumerable<ComplexGetResponseDTO>> GetAllComplexesAsync()
        {
            var result = await _complexRepository.GetAllAsync();
            return result.Select(x => x.ConvertComplexModelToGetComplexResponseDTO()).ToList();
        }

        public async Task<int> CreateComplex(ComplextCreateRequestDTO complexCreateDTO)
        {
            ComplexModel complexModel = complexCreateDTO.ConvertCreateRequestToComplexModel();
            int result = await _complexRepository.AddAsync(complexModel);
            return complexModel.Id;
        }

        public async Task<bool> UpdateComplex(ComplexUpdateRequestDTO complexUpdateDTO) //this part probably should be simplified by giving the dto to our infrastructure or sth else i need time for this
        {
            ComplexModel complexModel = complexUpdateDTO.ConvertUpdateRequestToComplexModel();
            return await _complexRepository.UpdateAsync(complexModel) > 0;
        }
    }
}
