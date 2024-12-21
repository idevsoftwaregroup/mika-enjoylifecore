using core.application.Contract.Infrastructure;
using core.domain.entity.financialModels;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;

namespace core.infrastructure.Data.repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private EnjoyLifeContext _context;

        public TransactionRepository(EnjoyLifeContext context)
        {
            _context = context;
        }
        public TransactionRequestModel GetTransactionByAuthorityAsync(string Authority)
        {
           var result=  _context.TransactionRequests.Where(t=>t.Authority== Authority).FirstOrDefault();
            return result;
        }
        public  TransactionRequestModel AddRequest(TransactionRequestModel transactionRequestModel)
        {
             _context.TransactionRequests.Add(transactionRequestModel);
             _context.SaveChanges();
            return transactionRequestModel;
        }
        public TransactionResponseModel AddResponse(TransactionResponseModel transactionResponseModel)
        {
            try
            {
                _context.TransactionResponses.Add(transactionResponseModel);
                _context.SaveChanges();
                
            
            }
            catch(Exception ex) {
                var x = ex;
            }
            return transactionResponseModel;
        }
    }
}
