using core.application.Contract.Infrastructure;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using core.infrastructure.Data.repository.exceptions;
using Microsoft.EntityFrameworkCore;

namespace core.infrastructure.Data.repository;

public class ComplexRepository : IComplexRepository
{
    private EnjoyLifeContext _context;

    public ComplexRepository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public async Task<ComplexModel?> GetAsync(int id)
    {
        return await _context.Complexes.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<ComplexModel>> GetAllAsync()
    {
        return await _context.Complexes.ToListAsync();
    }

    public async Task<int> AddAsync(ComplexModel complex)
    {
        if (_context.Complexes.Any(c => c.Title == complex.Title))
            throw new ComplexCreateException("provided title already exists");

        await _context.Complexes.AddAsync(complex);
        return await SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(ComplexModel complex)
    {
        if (await _context.Complexes.AnyAsync(x => x.Id == complex.Id))
        {
            if (_context.Complexes.Any(c => c.Title == complex.Title && c.Id != complex.Id))
                throw new ComplexUpdateException($"another complex with the name {complex.Title} already exists");

            complex.CreatedDate = await _context.Complexes
                .Where(c => c.Id == complex.Id)
                .Select(c => c.CreatedDate)
                .FirstOrDefaultAsync(); // not good i think

            _context.Complexes.Update(complex);

            return await SaveChangesAsync();
        }
        else return -1;
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        try
        {
            var complex = await _context.Complexes.FindAsync(id);
            if (complex != null)
            {
                _context.Complexes.Remove(complex);
                return await SaveChangesAsync();
            }
            else
            {
                return -1;
            }
        }
        catch (Exception ex)
        {
            return -2;
        }
    }

    private async Task<int> SaveChangesAsync()
    {
        var now = DateTime.Now;

        foreach (var entry in _context.ChangeTracker.Entries<ComplexModel>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.ModifyDate = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifyDate = now;
            }
        }

        return await _context.SaveChangesAsync();
    }


}
