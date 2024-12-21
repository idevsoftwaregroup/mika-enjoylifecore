using core.application.Contract.API.DTO.Payment;
using core.domain.displayEntities.financialModels;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.Infrastructure
{
    public interface IPaymentRepository
    {
        PaymentModel getPayment(long paymentId, int userId = 0);
        PaymentModel getPaymentAdmin(long paymentId);
        PaymentModel getPaymentByRequest(int transactionRequestId);
        PaymentModel getPaymentDetail(long paymentId);
        long createPayment(PaymentModel paymentDetial);
        List<PaymentModel> getPayments(List<long> paymentsId);
        List<PaymentModel> getPaymentsCreatedByResident(int residentID);
        List<PaymentModel> getpaymentsByUnit(int unitId);
        bool updatePayment(PaymentModel paymentDetial);
        List<PaymentModel> getPaymentsCreatedByUser(int userId);       
        Task<List<PaymentModel>> GetNotApprovedPayments(bool? hasVoucher = null, bool? hasImage = null);        
        Task<(List<PaymentModelDisplayDTO> Payments, int TotalCount)> GetPaymentsForAdminAsync(GetAllPaymentsDTO dto);
    }
}
