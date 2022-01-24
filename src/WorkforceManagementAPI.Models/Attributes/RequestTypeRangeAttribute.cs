using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;

namespace WorkforceManagementAPI.Models.Attributes
{
    class RequestTypeRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int num = (int)value;
            int maxRange = Enum.GetValues(typeof(RequestType)).Length;

            return num >= 0 && num < maxRange;
        }
    }
}
