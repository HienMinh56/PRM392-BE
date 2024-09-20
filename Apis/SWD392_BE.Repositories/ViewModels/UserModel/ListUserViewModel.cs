using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class ListUserViewModel
    {
        public string UserId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Campus { get; set; } = null!;
        public string Area { get; set; } = null!;

        public string Phone { get; set; }

        public int Role { get; set; }

        public int Balance { get; set; }

        public int Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
