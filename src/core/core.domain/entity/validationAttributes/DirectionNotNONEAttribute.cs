using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.domain.entity.validationAttributes
{
    public class DirectionNotNONEAttribute : ValidationAttribute
    {
        private readonly DirectionType _enumType;

        public DirectionNotNONEAttribute()
        {

        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            //if (!Enum.IsDefined(typeof(DirectionType), value))
            //{
            //    return false;
            //}

            var enumValue = (int)value;
            return enumValue != 0;
        }
    }
}
