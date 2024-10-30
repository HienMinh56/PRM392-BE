using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class EditUserViewModel
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!; // Added ConfirmPassword field
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
    }
}