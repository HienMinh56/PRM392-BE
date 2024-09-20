using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.StoreModel
{
    public class GetStoreViewModel
    {
        public string StoreId { get; set; } = null!;
        public string AreaId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Status { get; set; }
        public string? Phone { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string? AreaName { get; set; }
        public List<string>? Session { get; set;}
    }
}
