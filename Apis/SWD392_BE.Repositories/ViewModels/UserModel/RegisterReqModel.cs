using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class RegisterReqModel
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!; // Added ConfirmPassword field
        public string Email { get; set; } = null!;
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "The Name field cannot contain special characters.")]
        public string Name { get; set; } = null!;
        public string CampusId { get; set; } = null!;
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must have 10 numbers.")]
        public string? Phone { get; set; }
        public int Role {  get; set; }
    }
}
