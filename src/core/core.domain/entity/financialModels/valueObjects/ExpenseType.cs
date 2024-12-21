using System.ComponentModel;

namespace core.domain.entity.financialModels.valueObjects;

public enum ExpenseType
{
    [Description("انجوی لایف")]
    EnjoyLife = 1,
    [Description("احتیاطی")]
    Escrow = 2,
    [Description("جاری")]
    Routin = 3

}
