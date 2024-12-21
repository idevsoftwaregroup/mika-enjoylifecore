using core.application.Contract.API.DTO.Payment;
using core.application.Contract.Infrastructure;
using core.domain.displayEntities.financialModels;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace core.infrastructure.Data.repository;

public class PaymentRepository : IPaymentRepository
{
    private EnjoyLifeContext _context;

    public PaymentRepository(EnjoyLifeContext context)
    {
        _context = context;
    }
    public PaymentModel getPayment(long paymentId, int userId = 0)
    {
        return _context.Payments.Where(x => x.Id == paymentId && (userId == 0 || x.createBy.Id == userId))
            .Include(r => r.createBy)
            .Include(r => r.expenses)
            .Include(tr => tr.transactionRequest)
            .FirstOrDefault();
    }

    public PaymentModel getPaymentAdmin(long paymentId)
    {
        return _context.Payments.Where(x => x.Id == paymentId)
            .Include(r => r.createBy)
            .Include(r => r.expenses).ThenInclude(u => u.Unit)
            .Include(tr => tr.transactionRequest)
            .FirstOrDefault();
    }

    public PaymentModel getPaymentByRequest(int transactionRequestId)
    {
        return _context.Payments.Where(x => x.transactionRequest.Id == transactionRequestId)
            .Include(r => r.createBy)
            .Include(r => r.expenses)
            .Include(r => r.Account)
            .FirstOrDefault();
    }
    public PaymentModel getPaymentDetail(long paymentId)
    {

        return _context.Payments
            .Include(p => p.transactionRequest)
            .Include(p => p.TransactionResponse)
            .Where(p => p.Id == paymentId)
            .FirstOrDefault();
    }
    public List<PaymentModel> getPayments(List<long> paymentsId)
    {
        return _context.Payments
            .Where(x => paymentsId.Contains(x.Id))
            .Include(r => r.createBy)
            .Include(r => r.expenses)
            .ToList();
    }
    public List<PaymentModel> getPaymentsCreatedByResident(int residentID)
    {
        return _context.Payments
            .Where(x => x.createBy.Id == residentID)
            .Include(r => r.createBy)
            .Include(r => r.expenses)
            .ToList();
    }
    public List<PaymentModel> getpaymentsByUnit(int unitId)
    {

        return _context.Payments
            .Where(x => x.expenses.Any(j => j.Unit.Id == unitId))
            .Include(r => r.createBy)
            .Include(r => r.expenses)
            .ToList();
    }
    public bool updatePayment(PaymentModel paymentDetial)
    {
        _context.Attach(paymentDetial);
        var result = _context.SaveChanges();
        return result > 0;
    }
    public long createPayment(PaymentModel paymentDetial)
    {
        _context.Payments.Add(paymentDetial);
        _context.SaveChanges();
        return paymentDetial.Id;
    }

    public List<PaymentModel> getPaymentsCreatedByUser(int userId)
    {
        return _context.Payments.Where(x => x.createBy.Id == userId && x.paymentState != PaymentStateType.created
        && x.paymentState != PaymentStateType.inBankGate)
        .Include(r => r.createBy)
        .Include(x => x.expenses).OrderByDescending(o => o.Id)
        .ToList();
    }

    public async Task<(List<PaymentModelDisplayDTO> Payments, int TotalCount)> GetPaymentsForAdminAsync(GetAllPaymentsDTO dto)
    {
        int totalCount = (from payment in await _context.Payments.Include(r => r.createBy).Include(x => x.expenses).ThenInclude(u => u.Unit).OrderByDescending(o => o.Id).ToListAsync()
                          where
                          (dto.Id == null || payment.Id.ToString().StartsWith(dto.Id.ToString())) &&
                          (dto.Amount == null || payment.expenses.Sum(x => x.Amount).ToString().StartsWith(dto.Amount.ToString())) &&
                          (dto.PaymentStatus == null || payment.paymentState == dto.PaymentStatus) &&
                          (dto.CreateBy == null || payment.createBy.Id == dto.CreateBy) &&
                          (dto.UnitId == null || payment.expenses.Any(x => x.UnitModelId == dto.UnitId)) &&
                          (payment.paymentState != PaymentStateType.created) &&
                          (payment.paymentState != PaymentStateType.inBankGate) &&
                          (dto.StartDate == null || payment.createDate >= dto.StartDate) &&
                          (dto.EndDate == null || payment.createDate <= dto.EndDate)
                          select payment.Id
                              ).ToList().Count;
        if (totalCount == 0)
        {
            return (new List<PaymentModelDisplayDTO>(), 0);
        }
        return ((from payment in await _context.Payments.Include(r => r.createBy).Include(x => x.expenses).ThenInclude(u => u.Unit).OrderByDescending(o => o.Id).ToListAsync()
                 where
               (dto.Id == null || payment.Id.ToString().StartsWith(dto.Id.ToString())) &&
               (dto.Amount == null || payment.expenses.Sum(x => x.Amount).ToString().StartsWith(dto.Amount.ToString())) &&
               (dto.PaymentStatus == null || payment.paymentState == dto.PaymentStatus) &&
               (dto.CreateBy == null || payment.createBy.Id == dto.CreateBy) &&
               (dto.UnitId == null || payment.expenses.Any(x => x.UnitModelId == dto.UnitId)) &&
               (payment.paymentState != PaymentStateType.created) &&
               (payment.paymentState != PaymentStateType.inBankGate) &&
               (dto.StartDate == null || payment.createDate >= dto.StartDate) &&
               (dto.EndDate == null || payment.createDate <= dto.EndDate)
                 select new PaymentModelDisplayDTO
                 {
                     Id = payment.Id,
                     Name = payment.createBy.FirstName + " " + payment.createBy.LastName,
                     PayDate = payment.paymentDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                     PayNumber = payment.Id.ToString(),
                     Status = payment.Description.ToString(),
                     TransactionStatus = (int)payment.paymentState,
                     TotalCost = payment.expenses.Sum(x => x.Amount),
                     PayType = payment.paymentType.ToString(),
                     Title = payment.expenses[0].Title,
                     UnitName = payment.expenses[0].Unit.Name,
                     bankReciveImagePath = payment.bankReciveImagePath,
                     bankVoucherId = payment.bankVoucherId,
                     createDate = payment.createDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                     PaymentBy = payment.createBy.FirstName + " " + payment.createBy.LastName,
                     paymentDate = payment.paymentDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                     paymentTime = payment.paymentDate.ToString("HH:mm", new CultureInfo("fa-IR")),
                     paymentType = payment.paymentType,
                     state = payment.paymentState,
                     totalAmount = payment.expenses.Sum(x => x.Amount),
                     expenses = payment.expenses.Select(x => new GetExpenseResponseDisplayDTO
                     {
                         Amount = x.Amount,
                         Description = x.Description,
                         DueDate = x.DueDate?.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                         Id = x.Id,
                         IssueDateTime = x.IssueDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
                         RegisterNO = x.RegisterNO,
                         Title = x.Title,
                         Type = x.Type,
                         UnitId = x.UnitModelId,
                         UnitName = x.Unit.Name,
                         UserID = payment.createBy.Id

                     }).ToList()

                 }).Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize).ToList(), totalCount);
    }

    public async Task<List<PaymentModel>> GetNotApprovedPayments(bool? hasVoucher = null, bool? hasImage = null)
    {
        var query = _context.Payments.Where(p => p.paymentState == PaymentStateType.needApproval);
        //if (hasVoucher is not null)
        //{
        //    query = (bool)hasVoucher ? query.Where(p => p.bankVoucherId != null && p.bankVoucherId != "") : query.Where(p => p.bankVoucherId == null || p.bankVoucherId == "");
        //}
        //if (hasImage is not null)
        //{
        //    query = (bool)hasImage ? query.Where(p => p.bankReciveImagePath != null && p.bankReciveImagePath != "") : query.Where(p => p.bankReciveImagePath == null || p.bankReciveImagePath == "");
        //}
        return await query.Include(r => r.createBy)
            .Include(r => r.expenses)
            .ToListAsync();
    }
}
