using core.application.Contract.API.DTO.Expense;
using core.application.Framework;
using core.domain.DomainModelDTOs.ExpenseDTOs;

namespace core.application.Contract.API.Interfaces;

public interface IExpenseService
{
    long CreateExpense(CreateExpenseDTO expenseValue);
    GetExpenseResponseDTO GetExpense(long expenseId);
    List<GetExpenseResponseDTO> GetExpenseByFilter(GetExpenseRequestDTO getExpenseRequestDTO);
    Task<List<GetExpenseResponseDTO>> GetExpenseByFilterUnits(GetExpenseRequestDTO getExpenseRequestDTO);
    GetTotalExpenseResponseDTO GetTotalExpense(int userId);
    GetTotalExpenseResponseDTO GetTotalExpenseUnits(int userId);
    GetTotalExpenseWithDetailsResponseDTO GetTotalExpenseWithDetails(GetExpenseRequestDTO filter);
    (List<GetExpenseResponseDTO> Expenses, int TotalCount) GetExpensesByAdmin(GetExpenseFilterRequestDTO getExpenseFilterRequestDTO);
    Task<OperationResult<object>> CreateExpenses(int adminId, List<Request_CreateExpenseDomainDTO>? model, CancellationToken cancellation = default);
}

