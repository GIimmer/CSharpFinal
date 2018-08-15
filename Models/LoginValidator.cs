using System.ComponentModel.DataAnnotations;
namespace CSharpFinal.Models
{
    public class LoginValidator
    {
        [Required]
        public string LoginUserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}