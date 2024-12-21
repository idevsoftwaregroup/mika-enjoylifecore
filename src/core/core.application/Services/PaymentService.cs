using core.application.contract.api.DTO.Payment;
using core.application.contract.api.Zarinpal;
using core.application.Contract.API.DTO.Payment;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.infrastructure.Services;
using core.application.Contract.Infrastructure;
using core.domain.displayEntities.financialModels;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace core.application.Services
{
    public class PaymentService : IPaymentService
    {
        private HttpCore _HttpCore;
        IPaymentRepository _paymentRepository;
        IExpenseRepository _expenseRepository;
        IUserRepository _userRepository;
        IAccountRepository _accountRepository;
        ITransactionRepository _transactionRepository;
        IMessagingService _messagingService;

        public PaymentService(IPaymentRepository paymentRepository,
            IUserRepository residentRepository,
            IExpenseRepository expenseRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IMessagingService messagingService)
        {
            _paymentRepository = paymentRepository;
            _userRepository = residentRepository;
            _expenseRepository = expenseRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _messagingService = messagingService;
        }
        public long createPayment(CreatePaymentDTO paymentDetial)
        {
            PaymentModel tempPayment = new();
            tempPayment.expenses = _expenseRepository.getExpenses(paymentDetial.expenses);
            tempPayment.paymentState = PaymentStateType.created;
            tempPayment.paymentDate = DateTime.Now;
            tempPayment.createBy = _userRepository.GetUserAsync(paymentDetial.createBy).Result;
            tempPayment.paymentType = PaymentType.Online;
            tempPayment.Description = "پیش نویس";
            tempPayment.bankVoucherId = "0";
            tempPayment.createDate = DateTime.Now;
            tempPayment.bankReciveImagePath = "";
            var paymentId = _paymentRepository.createPayment(tempPayment);

            if (paymentId < 0)
            {
                throw new Exception("Failed to creat payment");
            }
            return paymentId;
        }

        [Obsolete]
        public string RequestZarinPayment(PaymentZarinRequestDTO transactionDetial)
        {
            #region base info
            var user = _userRepository.GetUserAsync(transactionDetial.RequestBy).Result;
            var payment = _paymentRepository.getPayment(transactionDetial.PaymentId, user.Id);
            var Amount = _expenseRepository.getExpenses(payment.expenses.Select(expense => expense.Id).ToList()).Sum(expense => expense.Amount) / 10;
            //var Amount = 1000;
            var account = _accountRepository.GetAccount(transactionDetial.buildingId, transactionDetial.AccountType);
            #endregion
            #region request object
            TransactionRequestModel request = new TransactionRequestModel();

            request.Description = transactionDetial.Description;
            request.Amount = Amount;
            request.Email = user.Email;
            request.Mobile = user.PhoneNumber;
            #endregion
            #region get Authority
            PaymentZarinGetURL paymentZarinGetURL = new PaymentZarinGetURL();
            paymentZarinGetURL.MerchantID = account.MerchantID;
            paymentZarinGetURL.CallbackURL = transactionDetial.CallbackURL;
            paymentZarinGetURL.Description = transactionDetial.Description;
            paymentZarinGetURL.Amount = request.Amount;
            paymentZarinGetURL.Mobile = request.Mobile;
            paymentZarinGetURL.Email = request.Email;
            #endregion
            #region get zarin url
            URLs url = new URLs(false);
            _HttpCore = new HttpCore();
            _HttpCore.URL = url.GetPaymentRequestURL();
            _HttpCore.Method = Method.POST;
            _HttpCore.Raw = paymentZarinGetURL;
            string response = _HttpCore.Get();
            var tempPayment = JsonConvert.DeserializeObject<PaymentZarinResponseDTO>(response);
            tempPayment.PaymentURL = url.GetPaymenGatewayURL(tempPayment.Authority);
            #endregion

            request.Authority = tempPayment.Authority;
            payment.transactionRequest = request;
            payment.paymentState = PaymentStateType.inBankGate;
            payment.lastUpdateDate = DateTime.Now;
            payment.Account = account;
            _paymentRepository.updatePayment(payment);
            return tempPayment.PaymentURL;
        }


        [Obsolete]
        public PaymentZarinVerifyResponseDTO VerifyZarinPayment(string Authority)
        {
            var transactionRequest = _transactionRepository.GetTransactionByAuthorityAsync(Authority);
            var payment = _paymentRepository.getPaymentByRequest(transactionRequest.Id);
            PaymentZarinVerificationDTO paymentVerification = new PaymentZarinVerificationDTO(payment.Account.MerchantID, transactionRequest.Amount, Authority);

            URLs url = new URLs(false);
            _HttpCore = new HttpCore();
            _HttpCore.URL = url.GetVerificationURL();
            _HttpCore.Method = Method.POST;
            _HttpCore.Raw = paymentVerification;
            string zarinResponse = _HttpCore.Get();
            var verification = JsonConvert.DeserializeObject<PaymentZarinVerifyResponseDTO>(zarinResponse);
            verification.totalAmount = transactionRequest.Amount;
            verification.CreatedBy = payment.createBy.FirstName + " " + payment.createBy.LastName;
            verification.PaymentId = payment.Id;
            UpdateBankPayment(verification);

            #region Message
            if (verification.IsSuccess)
            {
                var totalAmount = payment.expenses.Sum(e => e.Amount);
                _messagingService.SendPaymentOnlineSMS(new Contract.API.DTO.Messaging.PaymentOnlineSMSDTO
                {
                    CreatedDate = DateTime.Now.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
                    AccountType = payment.Account.AccountType switch
                    {
                        1 => "انجوی لایف",
                        2 => "احتیاطی",
                        3 => "جاری",
                        _ => "نامشخص"
                    },
                    CreatedBy = verification.CreatedBy,
                    PaymenyId = payment.Id.ToString(),
                    TotalAmount = string.Format("{0:N0}", totalAmount),
                });
            }
            #endregion
            return verification;
        }
        public bool UpdateBankPayment(PaymentZarinVerifyResponseDTO paymentZarinVerifyResponse)
        {
            bool result = false;
            var payment = _paymentRepository.getPayment(paymentZarinVerifyResponse.PaymentId);

            TransactionResponseModel tempTransactionModel = new TransactionResponseModel();
            tempTransactionModel.Status = paymentZarinVerifyResponse.Status;
            tempTransactionModel.IsSuccess = paymentZarinVerifyResponse.IsSuccess;
            tempTransactionModel.RefID = paymentZarinVerifyResponse.RefID;
            tempTransactionModel.ExtraDetail = "test";
            payment.TransactionResponse = tempTransactionModel;

            if (payment != null && paymentZarinVerifyResponse.IsSuccess)
            {
                payment.paymentState = PaymentStateType.success;
                payment.Description = "تراکنش موفقیت آمیز";
                foreach (var expense in payment.expenses)
                {
                    expense.IsPaid = true;
                }
            }
            else if (payment != null && !paymentZarinVerifyResponse.IsSuccess)
            {
                payment.paymentState = PaymentStateType.faild;
                payment.Description = "تراکنش ناموفق";
            }
            result = _paymentRepository.updatePayment(payment);

            return result;
        }

        public GetPaymentResponseDTO getPayment(long paymentId, int userId)
        {
            var result = _paymentRepository.getPayment(paymentId, userId);
            if (result is null)
            {
                throw new Exception($"Payment by this Id {paymentId} not found");
            }
            return result.PaymentModeltoGetPaymentResponseDTO();
        }

        public GetPaymentResponseDTO getPaymentAdmin(long paymentId)
        {
            var result = _paymentRepository.getPaymentAdmin(paymentId);
            if (result is null)
            {
                throw new Exception($"Payment by this Id {paymentId} not found");
            }
            return result.PaymentModeltoGetPaymentResponseDTO();
        }

        public bool RegisterVoucherPayment(RegisterPaymentVoucherDTO paymentDetial)
        {
            var payment = _paymentRepository.getPayment(paymentDetial.Id);
            if (payment is null)
            {
                throw new Exception($"Payment by this Id {paymentDetial.Id} not found");
            }
            if (string.IsNullOrEmpty(paymentDetial.bankVoucherId) && string.IsNullOrEmpty(paymentDetial.bankReciveImagePath))
            {
                throw new Exception($"Please fill the required fields");
            }
            var account = _accountRepository.GetAccount(paymentDetial.ComplexId, paymentDetial.AccountType);
            payment.paymentType = PaymentType.BankVoucher;
            payment.paymentState = PaymentStateType.needApproval;
            payment.Description = "در حال بررسی";
            payment.bankVoucherId = paymentDetial.bankVoucherId;
            payment.bankReciveImagePath = paymentDetial.bankReciveImagePath;
            payment.Account = account;
            var result = _paymentRepository.updatePayment(payment);

            var totalAmount = payment.expenses.Sum(e => e.Amount);
            _messagingService.SendPaymentOfflineSMS(new Contract.API.DTO.Messaging.PaymentOfflineSMSDTO
            {
                CreatedDate = DateTime.Now.ToString("yy/MM/dd-HH:mm", new CultureInfo("fa-IR")),
                BankVoucherImage = !string.IsNullOrEmpty(paymentDetial.bankReciveImagePath)
                ? "https://app.enjoylife.ir/filestorage/" + paymentDetial.bankReciveImagePath : "", //make dynamic
                BankVoucherId = paymentDetial.bankVoucherId,
                //AccountType = account.AccountType == 1 ? "انجوی لایف" : account.AccountType ==2 ? "احتیاطی" : "جاری",
                AccountType = account.AccountType switch
                {
                    1 => "انجوی لایف",
                    2 => "احتیاطی",
                    3 => "جاری",
                    _ => "نامشخص"
                },
                CreatedBy = payment.createBy.FirstName + " " + payment.createBy.LastName,
                PaymenyId = payment.Id.ToString(),
                TotalAmount = string.Format("{0:N0}", totalAmount),

            });

            return result;
        }



        public GetPaymentDetailResponseDTO getPaymentDetail(long paymentId)
        {

            var result = _paymentRepository.getPaymentDetail(paymentId);
            if (result is null)
            {
                throw new ApplicationException($"payment by this id {paymentId} not found");
            }
            return result.PaymentModeltoGetPaymentDetailResponseDTO();
        }

        public List<GetPaymentsDTO> getPayments(int userId)
        {
            List<PaymentModel> result = _paymentRepository.getPaymentsCreatedByUser(userId);
            if (result is null)
            {
                throw new Exception($"Payment by this Id {userId} not found");
            }
            return result
                .Select(x => x.PaymentModeltoGetPaymentsResponseDTO())
                .ToList();
        }

        public async Task<(List<GetPaymentsDTO> Payments, int TotalCount)> GetPaymentsAdminAsync(GetAllPaymentsDTO dto)
        {
            (List<PaymentModelDisplayDTO> payments, int totalCount) = await _paymentRepository.GetPaymentsForAdminAsync(dto);

            var paymentDTOs = payments
                .Select(x => new GetPaymentsDTO
                {
                    Status = x.Status,
                    UnitName = x.UnitName,
                    TransactionStatus = x.TransactionStatus,
                    Id = x.Id,
                    Name = x.Name,
                    PayDate = x.PayDate,
                    PayNumber = x.PayNumber,
                    PayType = x.PayType,
                    Title = x.Title,
                    TotalCost = x.TotalCost,
                })
                .ToList();

            return (paymentDTOs, totalCount);
        }
        //
        public async Task<(List<GetPaymentResponseDTO> Payments, int TotalCount)> GetExcelPaymentsAsync(GetAllPaymentsDTO dto)
        {
            (List<PaymentModelDisplayDTO> payments, int totalCount) = await _paymentRepository.GetPaymentsForAdminAsync(dto);

            var paymentDTOs = payments
                .Select(x => new GetPaymentResponseDTO
                {
                    bankReciveImagePath = x.bankReciveImagePath,
                    bankVoucherId = x.bankVoucherId,
                    createDate = x.createDate,
                    expenses = x.expenses.Select(y => new Contract.API.DTO.Expense.GetExpenseResponseDTO
                    {
                        Amount = y.Amount,
                        Description = y.Description,
                        DueDate = y.DueDate,
                        Id = y.Id,
                        IssueDateTime = y.IssueDateTime,
                        RegisterNO = y.RegisterNO,
                        Title = y.Title,
                        Type = y.Type,
                        UnitId = y.UnitId,
                        UnitName = y.UnitName,
                        UserID = y.UserID,
                    }).ToList(),
                    state = x.state,
                    Id = x.Id,
                    PaymentBy = x.PaymentBy,
                    paymentDate = x.paymentDate,
                    paymentTime = x.paymentTime,
                    paymentType = x.paymentType,
                    totalAmount = x.totalAmount,
                })
                .ToList();

            return (paymentDTOs == null || !paymentDTOs.Any() ? new List<GetPaymentResponseDTO>() : paymentDTOs, totalCount);
        }

        public async Task<List<GetPaymentResponseDTO>> GetNotApprovedPayments(bool? hasVoucher = null, bool? hasImage = null)
        {
            List<PaymentModel> paymentModels = await _paymentRepository.GetNotApprovedPayments(hasVoucher, hasImage);

            return paymentModels.Select(p => p.PaymentModeltoGetPaymentResponseDTO()).ToList();
        }

        public GetPaymentResponseDTO UpdatePayment(long paymentId, UpdatePaymentRequestDTO requestDTO)
        {
            PaymentModel paymentModel = _paymentRepository.getPayment(paymentId);
            if (paymentModel.paymentState != requestDTO.PaymentState)
            {
                _messagingService.SendPaymentStatusSMS(new Contract.API.DTO.Messaging.PaymentStatusSMSDTO
                {
                    CreatedBy = paymentModel.createBy.FirstName + " " + paymentModel.createBy.LastName,
                    PaymenyId = paymentModel.Id.ToString(),
                    AdminAction = requestDTO.PaymentState switch
                    {
                        PaymentStateType.success => "توسط ادمین مورد تایید قرار گرفت.",
                        PaymentStateType.faild => "توسط ادمین مورد تایید قرار نگرفت و به وضعیت ناموفق تغییر یافت",
                        _ => "نامشخص"
                    },
                });
            }
            paymentModel.paymentState = requestDTO.PaymentState;
            paymentModel.Description = requestDTO.Description;

            _paymentRepository.updatePayment(paymentModel);
            return paymentModel.PaymentModeltoGetPaymentResponseDTO();
        }
    }
}
