using core.domain.entity.financialModels;
using core.domain.entity.structureModels;

namespace core.application.Contract.Infrastructure
{
    public interface ITransactionRepository
    {       
        TransactionRequestModel GetTransactionByAuthorityAsync(string Authority);
        TransactionRequestModel AddRequest(TransactionRequestModel transactionRequestModel);
        TransactionResponseModel AddResponse(TransactionResponseModel transactionResponseModel);
    }
}
