using core.domain.entity.financialModels.valueObjects;
using core.domain.entity.structureModels;

namespace core.domain.entity.financialModels;

public class PaymentModel
{
    // we take long id here beacuse online payment system need that
    public long Id { get; set; }
    public UserModel createBy { get; set; }
    public List<ExpensesModel> expenses { get; set; }
    public DateTime createDate { get; set; }
    public PaymentType paymentType { get; set; }
    public DateTime paymentDate { get; set; }
    public DateTime lastUpdateDate { get; set; }
    public PaymentStateType paymentState { get; set; }
    public string bankVoucherId { get; set; }
    public string bankReciveImagePath { get; set; }
    public string Description { get; set; }
    public TransactionRequestModel? transactionRequest { get; set; }
    public TransactionResponseModel? TransactionResponse { get; set; }
    public AccountModel? Account { get; set; }

}
