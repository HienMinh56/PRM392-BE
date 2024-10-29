using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class DeleteUserReqModel
    {
        [Required(ErrorMessage = "UserId is required")]
        [Display(Name = "User Id")]
        public required string UserId { get; set; }
    }
}
