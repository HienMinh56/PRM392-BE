using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.TokenViewModel
{
    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
