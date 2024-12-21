using common.defination.Utility;
using core.application.Contract.API.DTO.Expense;
using core.application.Contract.API.DTO.Payment;
using core.domain.entity.financialModels;

namespace core.application.Contract.API.Mapper;

public static class FinancialModelMapper
{
    public static GetExpenseResponseDTO ExpenseModeltoGetExpenseResponseDTO(this ExpensesModel value)
    {
        GetExpenseResponseDTO result = new()
        {
            Id = value.Id,
            Title = value.Title,
            Description = value.Description,
            RegisterNO = value.RegisterNO,
            Amount = value.Amount,
            IssueDateTime = value.IssueDate.ConvertGeoToJalaiSimple(),
            DueDate = value.DueDate?.ConvertGeoToJalaiSimple(),
            UserID = value.User?.Id,
            UnitId = value.Unit?.Id, // use ? to check for null
            UnitName=value.Unit?.Name,
            Type = value.Type,
            //PersianIssueDateTime = value.IssueDate.ConvertGeoToJalaiSimple(),
            //PersianDueDateTime = value.DueDate.ConvertGeoToJalaiSimple(),
        };


        return result;
    }

    public static GetPaymentResponseDTO PaymentModeltoGetPaymentResponseDTO(this PaymentModel value)
    {
        GetPaymentResponseDTO result = new()
        {
            Id = value.Id,
            totalAmount = value.expenses.Sum(x => x.Amount), // calculate the total amount from the expenses list
            expenses = value.expenses.Select(x => x.ExpenseModeltoGetExpenseResponseDTO()).ToList(),
            state = value.paymentState,
            createDate = value.createDate.ConvertGeoToJalaiSimple(),
            paymentDate = value.paymentDate.ConvertGeoToJalaiSimple(),
            paymentTime = value.paymentDate.ToString("HH:mm"),
            PaymentBy = value.createBy.FirstName + " " + value.createBy.LastName,
            paymentType = value.paymentType,
            bankVoucherId = value.bankVoucherId,
            bankReciveImagePath = value.bankReciveImagePath,
        };

        return result;
    }    
    public static GetPaymentsDTO PaymentModeltoGetPaymentsResponseDTO(this PaymentModel value)
    {
        GetPaymentsDTO result = new()
        {
            Id = value.Id,
            Name = value.createBy.FirstName + " " + value.createBy.LastName,
            PayDate = value.paymentDate.ConvertGeoToJalaiSimple(),
            PayNumber = value.Id.ToString(),
            Status = value.Description.ToString(),
            TransactionStatus = (int)value.paymentState,
            TotalCost = value.expenses.Sum(x => x.Amount),
            PayType = value.paymentType.ToString()
        };

        return result;
    }

    public static GetPaymentDetailResponseDTO PaymentModeltoGetPaymentDetailResponseDTO(this PaymentModel value)
    {
        GetPaymentDetailResponseDTO result = new()
        {
            totalAmount = value.expenses.Sum(x => x.Amount), // calculate the total amount from the expenses list
            expenses = value.expenses.Select(x => x.ExpenseModeltoGetExpenseResponseDTO()).ToList(),
            state = value.paymentState,
            createDate = value.createDate.ConvertGeoToJalaiSimple(),    //data persian : there are 2 other date fields in the dto seemingly unused and in string type
            paymentDate = value.paymentDate.ConvertGeoToJalaiSimple(),
            paymentType = value.paymentType,
            bankVoucherId = value.bankVoucherId,
           // PayDate=value.transactionRequest.TransactionResponse.CreatedDate,

        };

        return result;
    }
 
}
