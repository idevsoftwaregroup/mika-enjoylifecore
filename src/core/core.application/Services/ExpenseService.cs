using core.application.Contract.API.DTO.Expense;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.ExpenseDTOs;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Services
{
    public class ExpenseService : IExpenseService
    {
        IExpenseRepository _expenseRepository;
        IUnitRepository _unitRepository;
        IUserRepository _userRepository;

        public ExpenseService(IExpenseRepository finacialRepository,
            IUnitRepository unitRepository,
            IUserRepository residentRepository)
        {
            _expenseRepository = finacialRepository;
            _unitRepository = unitRepository;
            _userRepository = residentRepository;
        }

        public long CreateExpense(CreateExpenseDTO expenseValue)
        {
            ExpensesModel tempExpense = new();
            tempExpense.User = _userRepository.GetUserAsync(expenseValue.UserId).Result;
            if (expenseValue.UnitID != null && expenseValue.UnitID > 0 || expenseValue.UnitID > 0)
            {
                tempExpense.UnitModelId = expenseValue.UnitID;
                tempExpense.Unit = _unitRepository.GetByIdAsync((int)expenseValue.UnitID).Result;
            }
            tempExpense.Title = expenseValue.Title;
            tempExpense.Type = expenseValue.ExpenseType;
            tempExpense.IssueDate = DateTime.Now;
            tempExpense.Amount = expenseValue.Amount;
            tempExpense.DueDate = expenseValue.DueDate;
            tempExpense.RegisterNO = expenseValue.RegisterNO;
            tempExpense.Description = expenseValue.Description;
            tempExpense.CreateDate = DateTime.Now;
            var expenseId = _expenseRepository.CreateExpense(tempExpense);

            if (expenseId == 0)
            {
                throw new Exception("Faild to Create expense");
            }

            return expenseId;
        }
        public GetExpenseResponseDTO GetExpense(long expenseId)
        {
            var resulttemp = _expenseRepository.getExpense(expenseId);
            if (resulttemp is null)
            {
                throw new Exception($"expense with id {expenseId} not found");
            }
            return resulttemp.ExpenseModeltoGetExpenseResponseDTO();
        }
        public List<GetExpenseResponseDTO> GetExpenseByFilter(GetExpenseRequestDTO getExpenseRequestDTO)
        {
            var tempresult = _expenseRepository.getListExpensesFullFilter(getExpenseRequestDTO);

            if (tempresult is null || tempresult.Count() == 0)
            {
                return null;
            }
            var result = tempresult.Select(x => x.ExpenseModeltoGetExpenseResponseDTO()).ToList();

            return result;
        }

        public (List<GetExpenseResponseDTO> Expenses, int TotalCount) GetExpensesByAdmin(GetExpenseFilterRequestDTO getExpenseFilterRequestDTO)
        {
            var (expenses, totalCount) = _expenseRepository.GetExpensesByAdmin(getExpenseFilterRequestDTO);

            if (expenses is null || expenses.Count == 0)
            {
                return (null, 0);
            }

            var result = expenses.Select(x => x.ExpenseModeltoGetExpenseResponseDTO()).ToList();

            return (result, totalCount);
        }


        public async Task<List<GetExpenseResponseDTO>> GetExpenseByFilterUnits(GetExpenseRequestDTO getExpenseRequestDTO)
        {
            var unitIdsTask = await _unitRepository.GetUserUnitsAsync((int)getExpenseRequestDTO.UserId);
            var unitIds = unitIdsTask.Select(unit => unit.Id).ToList();

            GetExpenseByUnitsRequestDTO getExpenseByUnitsRequestDTO = new()
            {
                ExpenseType = getExpenseRequestDTO.ExpenseType,
                UserId = getExpenseRequestDTO.UserId,
                UnitIds = unitIds
            };
            var data = await _expenseRepository.getListExpensesFullFilterByUnits(getExpenseByUnitsRequestDTO);

            if (data == null || !data.Any())
            {
                return new List<GetExpenseResponseDTO>();
            }
            var result = data.Select(x => x.ExpenseModeltoGetExpenseResponseDTO()).ToList();
            return result;
        }
        public GetTotalExpenseResponseDTO GetTotalExpense(int userId)
        {
            var totalexpense = _expenseRepository.getExpenseSumFilter(new() { UserId = userId });

            return totalexpense;
        }
        public GetTotalExpenseResponseDTO GetTotalExpenseUnits(int userId)
        {
            var unitIdsTask = _unitRepository.GetUserUnitsAsync(userId);
            var unitIds = unitIdsTask.Result.Select(unit => unit.Id).ToList();

            GetExpenseByUnitsRequestDTO getExpenseByUnitsRequestDTO = new GetExpenseByUnitsRequestDTO()
            {
                UserId = userId,
                UnitIds = unitIds
            };
            var totalexpense = _expenseRepository.getExpenseSumFilterByUnits(getExpenseByUnitsRequestDTO);

            return totalexpense;
        }
        public GetTotalExpenseWithDetailsResponseDTO GetTotalExpenseWithDetails(GetExpenseRequestDTO filter)
        {
            var Allexpense = _expenseRepository.getListExpensesFullFilter(filter);


            var Fnalresult = Allexpense.GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .ToList();
            GetTotalExpenseWithDetailsResponseDTO response = new();
            response.expenses = Allexpense
                .Select(x => x.ExpenseModeltoGetExpenseResponseDTO())
                .ToList();
            foreach (var item in Fnalresult)
            {
                switch (item.Type)
                {
                    case ExpenseType.EnjoyLife:
                        response.totalenjoyLife = item.TotalAmount;
                        break;
                    case ExpenseType.Escrow:
                        response.totalEscrow = item.TotalAmount;
                        break;
                    case ExpenseType.Routin:
                        response.totalRoutin = item.TotalAmount;
                        break;
                }
            }
            return response;
        }

        public async Task<OperationResult<object>> CreateExpenses(int adminId, List<Request_CreateExpenseDomainDTO>? model, CancellationToken cancellation = default)
        {
            return await _expenseRepository.CreateExpenses(adminId, model, cancellation);
        }
    }
}
