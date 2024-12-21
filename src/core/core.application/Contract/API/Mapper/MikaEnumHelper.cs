using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.Mapper
{
    public static class MikaEnumHelper
    {
        public static string GetDescription(this DirectionType value) 
        { 
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[]; 
            if (attributes != null && attributes.Length > 0) { 
                return attributes[0].Description;
            }
            return value.ToString();
        }
    }
}
