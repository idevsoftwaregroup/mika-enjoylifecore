using core.domain.entity.financialModels.valueObjects;
using core.domain.entity.structureModels;

namespace core.domain.entity.financialModels;

public class TransactionDetail
{
    public PaymentModel Payment { get; set; }
    public TransactionRequestModel? TransactionRequest { get; set; }
    public TransactionResponseModel? TransactionResponse { get; set; }
}
