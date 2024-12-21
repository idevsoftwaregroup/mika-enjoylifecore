using core.application.Contract.API.DTO.Expense;
using core.application.Framework;
using core.domain.DomainModelDTOs.ExpenseDTOs;
using core.domain.entity.financialModels;

namespace core.application.Contract.Infrastructure;

public interface IExpenseRepository
{
    long CreateExpense(ExpensesModel expenseValue);
    ExpensesModel getExpense(long expenseId);
    List<ExpensesModel> getExpenses(List<long> expenseId);
    List<ExpensesModel> getExpenseResponseByUser(int residientId);
    List<ExpensesModel> getExpenseResponseByUnit(int unitId);
    GetTotalExpenseResponseDTO getExpenseSumFilter(GetExpenseRequestDTO filter);
    public List<ExpensesModel> getListExpensesFullFilter(GetExpenseRequestDTO filter);
    Task<List<ExpensesModel>> getListExpensesFullFilterByUnits(GetExpenseByUnitsRequestDTO filter);
    GetTotalExpenseResponseDTO getExpenseSumFilterByUnits(GetExpenseByUnitsRequestDTO filter);
    (List<ExpensesModel> Expenses, int TotalCount) GetExpensesByAdmin(GetExpenseFilterRequestDTO filter);
    Task<OperationResult<object>> CreateExpenses(int adminId, List<Request_CreateExpenseDomainDTO>? model, CancellationToken cancellation = default);
}
