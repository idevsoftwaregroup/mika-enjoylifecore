using System.ComponentModel;

namespace core.domain.entity.financialModels.valueObjects;

public enum PaymentType
{
    [Description("پرداخت انلاین")]
    Online = 1,
    [Description("پرداخت فیش")]
    BankVoucher = 2
}
