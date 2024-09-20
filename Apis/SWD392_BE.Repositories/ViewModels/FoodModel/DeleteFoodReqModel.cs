using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.FoodModel
{
    public class DeleteFoodReqModel
    {
        [Required(ErrorMessage = "FoodId is required")]
        [Display(Name = "Food Id")]
        public required string FoodId { get; set; }
    }
}
