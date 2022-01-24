using System;
using System.ComponentModel.DataAnnotations;

namespace WorkforceManagementAPI.Models.Attributes
{
    public class MinDateTodayAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var insertedDate = (DateTime)value;

            return insertedDate >= DateTime.UtcNow;
        }
    }
}