using core.application.contract.infrastructure;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.infrastructure.Data.repository
{
    public class ParkingRepository : IParkingRepository
    {
        private EnjoyLifeContext _context;

        public ParkingRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParkingModel>> GetAllAsync()
        {
            return await _context.Parkings.ToListAsync();
        }

        public async Task<ParkingModel> GetByIdAsync(int id)
        {
            return await _context.Parkings.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(ParkingModel parking)
        {
            await _context.Parkings.AddAsync(parking);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(ParkingModel parking)
        {
            if (await _context.Parkings.AnyAsync(x => x.Id == parking.Id))
            {
                _context.Parkings.Update(parking);
                return await _context.SaveChangesAsync();
            }
            else return -1;
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                var parking = await _context.Parkings.FindAsync(id);
                if (parking != null)
                {
                    _context.Parkings.Remove(parking);
                    return await _context.SaveChangesAsync();
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
    }
}
