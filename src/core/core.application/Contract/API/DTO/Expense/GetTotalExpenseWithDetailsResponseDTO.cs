namespace core.application.Contract.API.DTO.Expense;

public class GetTotalExpenseWithDetailsResponseDTO
{
    public List<GetExpenseResponseDTO> expenses { get; set; }
    public decimal totalenjoyLife { get; set; }
    public decimal totalEscrow { get; set; }
    public decimal totalRoutin { get; set; }
}
