using System;
using System.ComponentModel.DataAnnotations;
namespace CSharpFinal.Models
{
    public class AuctionValidator
    {
        [Required]
        [MinLength(3)]
        public string ProductName { get; set; }

        [Required]
        [MinLength(10)]
        public string ProductDesc { get; set; }

        [Required]
        [Range(0.01, 9999999.99)]
        public double CurrentBid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CurrentDate(ErrorMessage="Cannot be a past date")]
        public DateTime EndDate { get; set; }
    }
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }
        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if(dt >= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}