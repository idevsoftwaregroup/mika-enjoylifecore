using core.application.Contract.API.DTO.Concierge;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Services
{
    public class ConciergeService : IConciergeService
    {
        public readonly IConciergeRepository _conciergeRepo;

        public ConciergeService(IConciergeRepository conciergeRepo)
        {
            _conciergeRepo = conciergeRepo;
        }

        public async Task<List<ConciergeResponseDTO>> GetConcierge(CancellationToken cancellationToken)
        {
            // دریافت داده‌های ConciergeModel از مخزن
            var conciergeModels = await _conciergeRepo.GetConciergeAsync(cancellationToken);

            // تبدیل ConciergeModel به ConciergeResponseDTO
            var conciergeDTOs = conciergeModels.Select(model => new ConciergeResponseDTO
            {
                // فرض می‌کنیم ConciergeResponseDTO دارای فیلدهایی مشابه ConciergeModel است
                // فیلدهای مورد نیاز خود را از ConciergeModel به ConciergeResponseDTO نگاشت کنید
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                SupportDescription = model.SupportDescription,
                SupportPhone = model.SupportPhone,
                SupportTitle = model.SupportTitle,
                // بقیه فیلدها را نیز به همین شکل نگاشت کنید
            }).ToList();

            return conciergeDTOs;
        }

    }
}
