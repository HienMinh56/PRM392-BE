using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.StoreModel
{
    public class StoreViewModel
    {
        public string AreaId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }

        public string OpenTime { get; set; }

        public string CloseTime { get; set; }

    }
}
