using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.TransactionModel
{
    public class TransactionUserViewModel
    {
        public string TransationId { get; set; }
        public string UserId { get; set; }
        public int Amount { get; set; }
        public int Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int Status { get; set; }
        public TimeSpan? CreatTime { get; set; }
        public UserViewModel User { get; set; }

    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
