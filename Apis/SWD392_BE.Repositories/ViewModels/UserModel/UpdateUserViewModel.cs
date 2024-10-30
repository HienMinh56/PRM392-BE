using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class UpdateUserViewModel
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? CampusId { get; set; } = null!;

        public string Phone { get; set; }

        public int Role { get; set; }

        public int Balance { get; set; }
    }
}
