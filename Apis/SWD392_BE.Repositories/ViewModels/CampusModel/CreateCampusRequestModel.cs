﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.ViewModels.CampusModel
{
    public class CreateCampusRequestModel
    {
        public string CampusId { get; set; }

        public string AreaId { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }
    }
}
