using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.UserModel
{
    public class LoginResModel
    {
        public string userId { get; set; } = null!;
        public string name { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string campusId { get; set; }
        public int phone { get; set; }
        public int role { get; set; }
        public int balance { get; set; }
        public int status { get; set; }
        public DateTime? createdDate { get; set; }
        public string? createdBy { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiredTime { get; set; }

    }
}
