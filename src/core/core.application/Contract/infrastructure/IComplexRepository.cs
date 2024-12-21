using core.domain.entity.structureModels;

namespace core.application.Contract.Infrastructure;

public interface IComplexRepository
{
    Task<IEnumerable<ComplexModel>> GetAllAsync();
    Task<ComplexModel> GetAsync(int id);
    Task<int> AddAsync(ComplexModel complexModel);
    Task<int> UpdateAsync(ComplexModel complexModel);
    Task<int> DeleteAsync(Guid id);
}
