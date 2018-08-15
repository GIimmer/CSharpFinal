using System.ComponentModel.DataAnnotations;
namespace CSharpFinal.Models
{
    public class RegisterValidator
    {
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Compare("Password")]
        public string ConfirmPass { get; set; }
    }
}