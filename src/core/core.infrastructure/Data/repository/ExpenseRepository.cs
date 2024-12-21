using core.application.Contract.API.DTO.Expense;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.application.Helper;
using core.domain.DomainModelDTOs.ExpenseDTOs;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace core.infrastructure.Data.repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly EnjoyLifeContext _context;

        public ExpenseRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        public long CreateExpense(ExpensesModel expenseValue)
        {

            _context.Expenses.Add(expenseValue);
            _context.SaveChanges();
            return expenseValue.Id;
        }


        public ExpensesModel getExpense(long expenseId)
        {
            return _context.Expenses
                .Where(x => x.Id == expenseId)
                .Include(r => r.Unit)
                .Include(r => r.User)
                .FirstOrDefault();
        }

        public List<ExpensesModel> getExpenseResponseByUser(int userId)
        {
            return _context.Expenses.Where(x => x.User.Id == userId)
                .Include(r => r.Unit)
                .Include(r => r.User)
                .ToList();
        }

        public List<ExpensesModel> getExpenseResponseByUnit(int unitId)
        {
            return _context.Expenses.Where(x => x.Unit.Id == unitId)
                .Include(r => r.Unit)
                .Include(r => r.User)
                .ToList();
        }

        public List<ExpensesModel> getExpenses(List<long> expenseId)
        {
            return _context.Expenses.Where(x => expenseId.Contains(x.Id))
                .Include(r => r.Unit)
                .Include(r => r.User)
                .ToList();
        }


        public GetTotalExpenseResponseDTO getExpenseSumFilter(GetExpenseRequestDTO filter)
        {
            var thresholdDateTime = DateTime.Now - TimeSpan.FromMinutes(10);
            var tempquery = _context.Expenses
                                .Where(e =>

                //e.IsPaid != true && 
                !e.Payments.Any() ||
                !e.Payments.Any(p =>
                    p.paymentState == PaymentStateType.success ||
                    p.paymentState == PaymentStateType.needApproval ||
                    p.paymentState == PaymentStateType.finalize ||
                    (p.paymentState == PaymentStateType.inBankGate && p.lastUpdateDate > thresholdDateTime && p.createBy.Id != filter.UserId)
                    ))
                .Include(r => r.User).Include(r => r.Unit).AsQueryable();

            if (filter.UserId != null) { tempquery = tempquery.Where(x => x.User.Id == filter.UserId); }

            if (filter.UnitId != null) { tempquery = tempquery.Where(x => x.UnitModelId == filter.UnitId); }

            if (filter.ExpenseType != null) { tempquery = tempquery.Where(x => x.Type == filter.ExpenseType); }


            var Fnalresult = tempquery.GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, TotalAmount = g.Sum(e => e.Amount) }).ToList();

            GetTotalExpenseResponseDTO response = new();
            foreach (var item in Fnalresult)
            {
                switch (item.Type)
                {
                    case ExpenseType.EnjoyLife:
                        response.TotalenjoyLife = item.TotalAmount;
                        break;
                    case ExpenseType.Escrow:
                        response.TotalEscrow = item.TotalAmount;
                        break;
                    case ExpenseType.Routin:
                        response.TotalRoutin = item.TotalAmount;
                        break;
                }
            }

            return response;
        }
        public List<ExpensesModel> getListExpensesFullFilter(GetExpenseRequestDTO filter)
        {
            try
            {
                var thresholdDateTime = DateTime.Now - TimeSpan.FromMinutes(10);
                var tempquery = _context.Expenses
                .Where(e =>

                //e.IsPaid != true && 
                !e.Payments.Any() ||
                !e.Payments.Any(p =>
                    p.paymentState == PaymentStateType.success ||
                    p.paymentState == PaymentStateType.needApproval ||
                    p.paymentState == PaymentStateType.finalize ||
                    (p.paymentState == PaymentStateType.inBankGate && p.lastUpdateDate > thresholdDateTime && p.createBy.Id != filter.UserId)
                ))
                .Include(r => r.User).Include(r => r.Unit).AsQueryable();

                if (filter.UserId != null) { tempquery = tempquery.Where(x => x.User.Id == filter.UserId); }

                if (filter.UnitId != null) { tempquery = tempquery.Where(x => x.UnitModelId == filter.UnitId); }

                if (filter.ExpenseType != null) { tempquery = tempquery.Where(x => x.Type == filter.ExpenseType); }

                var expenseDetials = tempquery.ToList();
                return expenseDetials;
            }
            catch (Exception t)
            {
                var ggg = t.Message;
                return null;
            }

        }

        public (List<ExpensesModel> Expenses, int TotalCount) GetExpensesByAdmin(GetExpenseFilterRequestDTO filter)
        {
            try
            {
                var thresholdDateTime = DateTime.Now - TimeSpan.FromMinutes(10);
                var tempquery = _context.Expenses
                    .Where(e => !e.Payments.Any() ||
                                !e.Payments.Any(p =>
                                    p.paymentState == PaymentStateType.success ||
                                    p.paymentState == PaymentStateType.needApproval ||
                                    p.paymentState == PaymentStateType.finalize ||
                                    (p.paymentState == PaymentStateType.inBankGate && p.lastUpdateDate > thresholdDateTime && p.createBy.Id != filter.UserId)
                                ))
                    .Include(r => r.User).Include(r => r.Unit)
                    .OrderByDescending(o => o.CreateDate)
                    .AsQueryable();

                if (filter.UserId != null) { tempquery = tempquery.Where(x => x.User.Id == filter.UserId); }

                if (filter.UnitId != null) { tempquery = tempquery.Where(x => x.UnitModelId == filter.UnitId); }

                if (filter.ExpenseType != null) { tempquery = tempquery.Where(x => x.Type == filter.ExpenseType); }

                int totalCount = tempquery.Count();

                var expenseDetails = tempquery
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                return (expenseDetails, totalCount);
            }
            catch (Exception t)
            {
                var errorMessage = t.Message;
                return (null, 0);
            }
        }



        public GetTotalExpenseResponseDTO getExpenseSumFilterByUnits(GetExpenseByUnitsRequestDTO filter)
        {
            var thresholdDateTime = DateTime.Now - TimeSpan.FromMinutes(10);
            var tempquery = _context.Expenses
                                .Where(e =>

                //e.IsPaid != true && 
                !e.Payments.Any() ||
                !e.Payments.Any(p =>
                    p.paymentState == PaymentStateType.success ||
                    p.paymentState == PaymentStateType.needApproval ||
                    p.paymentState == PaymentStateType.finalize ||
                    (p.paymentState == PaymentStateType.inBankGate && p.lastUpdateDate > thresholdDateTime && p.createBy.Id != filter.UserId)
                    ))
                .Include(r => r.User).Include(r => r.Unit).AsQueryable();

            IQueryable<ExpensesModel>? result = null;
            //IQueryable<ExpensesModel>? UserExpense = null;
            //if (filter.UserId != null)
            //{
            //    UserExpense = tempquery.Where(x => x.User.Id == filter.UserId);
            //}

            IQueryable<ExpensesModel>? UnitExpense = null;
            if (filter.UnitIds != null && filter.UnitIds.Any())
            {
                //UnitExpense = tempquery.Where(x => filter.UnitIds.Contains((int)x.UnitModelId) && filter.UserId==null);
                UnitExpense = tempquery.Where(x => filter.UnitIds.Contains((int)x.UnitModelId));
            }

            //if (UnitExpense != null)
            //{
            //    result = UserExpense.Union(UnitExpense).Distinct();
            //    //return result;
            //}
            //else if (UserExpense != null)
            //{
            //    result = UserExpense.Distinct();
            //}
            else if (UnitExpense != null)
            {
                result = UnitExpense.Distinct();
            }


            var Fnalresult = UnitExpense.GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, TotalAmount = g.Sum(e => e.Amount) }).ToList();

            GetTotalExpenseResponseDTO response = new();
            foreach (var item in Fnalresult)
            {
                switch (item.Type)
                {
                    case ExpenseType.EnjoyLife:
                        response.TotalenjoyLife = item.TotalAmount;
                        break;
                    case ExpenseType.Escrow:
                        response.TotalEscrow = item.TotalAmount;
                        break;
                    case ExpenseType.Routin:
                        response.TotalRoutin = item.TotalAmount;
                        break;
                }
            }

            return response;
        }
        public async Task<List<ExpensesModel>> getListExpensesFullFilterByUnits(GetExpenseByUnitsRequestDTO filter)
        {
            try
            {
                var thresholdDateTime = DateTime.Now - TimeSpan.FromMinutes(10);

                var unitIDs = filter.UnitIds == null || !filter.UnitIds.Any() ? (from unit in await _context.Units.ToListAsync() select unit.Id).ToList() : filter.UnitIds;

                var expenses = (from expense in await _context.Expenses.Include(x => x.User).Include(x => x.Payments).Include(x => x.Unit).ToListAsync()
                                join unitId in unitIDs
                                on expense.Unit.Id equals unitId
                                where
                                (!expense.Payments.Any() ||
                                 !expense.Payments.Any(p =>
                                 p.paymentState == PaymentStateType.success ||
                                 p.paymentState == PaymentStateType.needApproval ||
                                 p.paymentState == PaymentStateType.finalize ||
                                 (p.paymentState == PaymentStateType.inBankGate && p.lastUpdateDate > thresholdDateTime && p.createBy.Id != filter.UserId)
                                ))
                                && (filter.ExpenseType == null || expense.Type == filter.ExpenseType)
                                && (filter.UserId == null || expense.User == null || expense.User.Id == filter.UserId)
                                select expense).ToList();
                if (expenses == null || !expenses.Any())
                {
                    return new List<ExpensesModel>();
                }
                return expenses;
            }
            catch (Exception t)
            {
                var ggg = t.Message;
                return new List<ExpensesModel>();
            }
        }

        public async Task<OperationResult<object>> CreateExpenses(int adminId, List<Request_CreateExpenseDomainDTO>? model, CancellationToken cancellation = default)
        {
            OperationResult<object> Op = new("CreateExpenses");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه کاربر ادمین بدرستی ارسال نشده است");
            }
            if (model == null || !model.Any())
            {
                return Op.Failed("اطلاعاتی برای ثبت ارسال نشده است");
            }
            if (model.GroupBy(x => new
            {
                x.Title,
                x.Amount,
                x.RegisterNO,
                x.Description,
                x.DueDate,
                x.Type,
                x.UnitName,
                x.IsPaid,
            }).Select(x => x.First()).ToList().Count != model.Count)
            {
                return Op.Failed("ثبت ردیف تکراری امکانپذیر نمیباشد");
            }
            if (model.Any(x => x.Amount <= 0))
            {
                return Op.Failed($"مقدار هزینه بدرستی ارسال نشده است ، خطا در ردیف : {model[model.FindIndex(x => x.Amount <= 0)].Row}");
            }
            if (model.Any(x => x.DueDate != null && Convert.ToDateTime(x.DueDate).ResetTime() < Op.OperationDate.ResetTime()))
            {
                return Op.Failed($"مهلت پرداخت بدرستی ارسال نشده است ، خطا در ردیف : {model[model.FindIndex(x => x.DueDate != null && Convert.ToDateTime(x.DueDate).ResetTime() < Op.OperationDate.ResetTime())].Row}");
            }
            if (model.Any(x => x.Type != ExpenseType.EnjoyLife && x.Type != ExpenseType.Escrow && x.Type != ExpenseType.Routin))
            {
                return Op.Failed($"نوع هزینه بدرستی ارسال نشده است ، خطا در ردیف : {model[model.FindIndex(x => x.Type != ExpenseType.EnjoyLife || x.Type != ExpenseType.Escrow || x.Type != ExpenseType.Routin)].Row}");
            }
            if (model.Any(x => string.IsNullOrEmpty(x.UnitName) || string.IsNullOrWhiteSpace(x.UnitName)))
            {
                return Op.Failed($"ارسال نام واحد اجباری است ، خطا در ردیف : {model[model.FindIndex(x => string.IsNullOrEmpty(x.UnitName) || string.IsNullOrWhiteSpace(x.UnitName))].Row}");
            }
            //if (model.Any(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber)))
            //{
            //    return Op.Failed($"ارسال شماره موبایل مربوط به شخص اجباری است ، خطا در ردیف : {model[model.FindIndex(x => string.IsNullOrEmpty(x.PhoneNumber) || string.IsNullOrWhiteSpace(x.PhoneNumber))].Row}");
            //}
            //if (model.Any(x => !Regex.IsMatch(x.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.Fa2En().StartsWith("09")))
            //{
            //    return Op.Failed($"ارسال شماره موبایل مربوط به شخص اجباری است ، خطا در ردیف : {model[model.FindIndex(x => !Regex.IsMatch(x.PhoneNumber.Fa2En(), "^[0-9]*$", RegexOptions.IgnoreCase) || x.PhoneNumber.Length != 11 || !x.PhoneNumber.Fa2En().StartsWith("09"))].Row}");
            //}
            try
            {
                var admin = await _context.Users.FirstOrDefaultAsync(x => x.Id == adminId, cancellationToken: cancellation);
                if (admin == null)
                {
                    return Op.Failed("شناسه کاربر ادمین بدرستی ارسال نشده است", HttpStatusCode.NotFound);
                }
                var existsUnits = (from unit in await _context.Units.ToListAsync(cancellationToken: cancellation) select unit).ToList();
                if (existsUnits == null || !existsUnits.Any())
                {
                    return Op.Failed("دریافت اطلاعات واحد ها با مشکل مواجه شده است");
                }
                if (model.Any(x => !existsUnits.Any(y => y.Name.Trim().ToLower() == x.UnitName.Trim().ToLower())))
                {
                    return Op.Failed($"نام واحد بدرستی ارسال نشده است ، خطا در ردیف : {model[model.FindIndex(x => !existsUnits.Any(y => y.Name.Trim().ToLower() == x.UnitName.Trim().ToLower()))].Row}", HttpStatusCode.NotFound);
                }
                List<ExpensesModel> requestListDTO = new();

                for (int i = 0; i < model.Count; i++)
                {
                    //var user = await _context.Users.FirstOrDefaultAsync(x => x.IsDelete != true, cancellationToken: cancellation);
                    //if (user == null)
                    //{
                    //    return Op.Failed($"شماره موبایل مربوط به شخص بدرستی ارسال نشده است ، خطا در ردیف : {model[i].Row}");
                    //}
                    var unit = existsUnits.FirstOrDefault(x => x.Name.ToLower() == model[i].UnitName.ToLower());
                    if (!await _context.Owners.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.Unit == unit, cancellationToken: cancellation)
                        &&
                        !await _context.Residents.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.Unit == unit, cancellationToken: cancellation))
                    {
                        return Op.Failed($"کاربر با اطلاعات وارد شده با واحد وارد شده در ارتباط نمیباشند ، خطا در ردیف : {model[i].Row}");
                    }
                    if (!await _context.Owners.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.Unit == unit, cancellationToken: cancellation) && !await _context.Residents.Include(x => x.Unit).Include(x => x.User).AnyAsync(x => x.Unit == unit && x.IsHead, cancellationToken: cancellation))
                    {
                        return Op.Failed($"اختصاص هزینه به کاربر تنها به سرپرست ساکنین در واحد امکان پذیر است ، خطا در ردیف : {model[i].Row}");
                    }
                    requestListDTO.Add(new ExpensesModel
                    {
                        Amount = model[i].Amount,
                        CreateDate = Op.OperationDate,
                        Description = string.IsNullOrEmpty(model[i].Description) || string.IsNullOrWhiteSpace(model[i].Description) ? null : model[i].Description.Trim(),
                        DueDate = model[i].DueDate?.ResetTime(),
                        IsPaid = model[i].IsPaid,
                        IssueDate = Op.OperationDate.ResetTime(),
                        RegisterNO = string.IsNullOrEmpty(model[i].RegisterNO) || string.IsNullOrWhiteSpace(model[i].RegisterNO) ? null : model[i].RegisterNO.Trim(),
                        Title = string.IsNullOrEmpty(model[i].Title) || string.IsNullOrWhiteSpace(model[i].Title) ? null : model[i].Title.Trim(),
                        Type = model[i].Type,
                        UnitModelId = existsUnits.FirstOrDefault(y => y.Name.Trim().ToLower() == model[i].UnitName.Trim().ToLower()).Id,
                        Unit = existsUnits.FirstOrDefault(y => y.Name.Trim().ToLower() == model[i].UnitName.Trim().ToLower()),
                    });
                }
                await _context.Expenses.AddRangeAsync(requestListDTO, cancellation);
                await _context.SaveChangesAsync(cancellation);
                return Op.Succeed("ثبت گروهی هزینه ها با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت گروهی هزینه ها با مشکل مواجه شده است", ex.Message, HttpStatusCode.InternalServerError);
            }

        }
    }
}
