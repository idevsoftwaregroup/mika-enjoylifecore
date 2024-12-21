using core.application.Contract.API.DTO.Complex;

namespace core.application.Contract.API.Interfaces;

public interface IComplexService
{
    Task<int> CreateComplex(ComplextCreateRequestDTO complexCreateDTO);
    Task<IEnumerable<ComplexGetResponseDTO>> GetAllComplexesAsync();
    Task<ComplexGetResponseDTO> GetComplexById(int id);
    Task<bool> UpdateComplex(ComplexUpdateRequestDTO complexUpdateDTO);
}
