using core.application.contract.api.DTO.Payment;
using core.application.Contract.API.DTO.Payment;
using core.domain.displayEntities.financialModels;
using core.domain.entity.financialModels;
using Microsoft.AspNetCore.Mvc;

namespace core.application.Contract.API.Interfaces
{
    public interface IPaymentService
    {
        long createPayment(CreatePaymentDTO paymentDetial);
        string RequestZarinPayment(PaymentZarinRequestDTO transactionDetial);
        PaymentZarinVerifyResponseDTO VerifyZarinPayment(string Authority);
        GetPaymentResponseDTO getPayment(long paymentId, int userId = 0);
        GetPaymentResponseDTO getPaymentAdmin(long paymentId);        
        bool RegisterVoucherPayment(RegisterPaymentVoucherDTO paymentDetial);
        GetPaymentDetailResponseDTO getPaymentDetail(long paymentId);
        List<GetPaymentsDTO> getPayments(int userId);
        Task<List<GetPaymentResponseDTO>> GetNotApprovedPayments(bool? hasVoucher = null, bool? hasImage = null);
        GetPaymentResponseDTO UpdatePayment(long paymentId, UpdatePaymentRequestDTO requestDTO);
        Task<(List<GetPaymentsDTO> Payments, int TotalCount)> GetPaymentsAdminAsync(GetAllPaymentsDTO dto);
        Task<(List<GetPaymentResponseDTO> Payments, int TotalCount)> GetExcelPaymentsAsync(GetAllPaymentsDTO dto);
    }
}
