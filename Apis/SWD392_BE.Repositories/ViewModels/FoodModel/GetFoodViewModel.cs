using Microsoft.AspNetCore.Http;
using SWD392_BE.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.FoodModel
{
    public class GetFoodViewModel
    {
        public int Id { get; set; }

        public string FoodId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string StoreId { get; set; } = null!;

        public int Price { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Cate { get; set; }

        public string? Image { get; set; }

        public int Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string? DeletedBy { get; set; }
        public int OrderCount { get; set; }
    }
}
