using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.StoreModel
{
    public class DeleteStoreReqModel
    {
        [Required(ErrorMessage = "StoreId is required")]
        [Display(Name = "Store Id")]
        public required string StoreId { get; set; }

    }
}
