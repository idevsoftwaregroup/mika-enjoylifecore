using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.domain.entity.validationAttributes
{
    public class UnitUsageNotNONEAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public UnitUsageNotNONEAttribute()
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            //if (!Enum.IsDefined(_enumType, value))
            //{
            //    return false;
            //}

            var enumValue = (UnitUsageType)value;
            return enumValue != UnitUsageType.NONE;
        }
    }
}
